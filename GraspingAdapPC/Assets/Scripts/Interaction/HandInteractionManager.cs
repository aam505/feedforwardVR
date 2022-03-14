using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static OVRHand;
using static OVRSkeleton;
using HPTK.Models.Avatar;
using HPTK.Controllers.Avatar;
using HPTK.Controllers.Input;
using System;


/**
* Press A to start recording gesture
* Release A to stop recording gesture
* Press S to cycle through the first gesture of all recorded interactions
**/

public class HandInteractionManager : MonoBehaviour
{
    [Space(20)]

    [Header("Recording settings")]

    [Tooltip("Select the handedness of the hand recoding the interaction.")]
    public OVRHand.Hand NewInteractionHandedness;
    [Tooltip("Add each object that move in interaction, both parents and children, and also objects that have scripts that control the moving parts. ")]
    public List<GameObject> MovingParts;
    [Tooltip("If you want to have ghosts, duplicate everything in MovingParts in the scene, add the ghosted material to it, remove the scripts that move objects, and add the parent object reference here.")]
    public GameObject GhostObjectDuplicate;
    [Tooltip("Add the moving parts from the ghost object, same as for MovingParts. Or they will be added at startup. The order between the MovingParts and GhostMovingParts has to be the same.")]
    public List<GameObject> GhostMovingParts = new List<GameObject>();
    [Tooltip("Before recording interaction, add an InteractionId for it to Interaction.cs. The id can be a Helpful enum that captures the type of hand and objects involved in the interactions. Give it a useful name so you can identify it uniquely.")]
    public InteractionId NewInteractionId;

    [Space(20)]
    [Header("Other settings")]

    [Tooltip("Select how much time it takes for the ghost hand or original hand to fly to the area of the preview.")]
    public float InterpolationDurationSet;
    [Tooltip("Select by how much the preview speed should be increased: multiplyer x recording speed.")]
    public float FeedforwardMultiplierSet;

    [Tooltip("Select by how much the rewind speed should be increased: multiplyer x rewind speed. Recommend always speeding the rewinding.")]
    public float RewindMultiplierSet;

    [Tooltip("Place the lens of each hand here. This functionality is currently disabled.")]
    [ReadOnly]
    public GameObject FFWindowLeft, FFWindowRight;//, TrackedWindowLeft, TrackedWindowRight;
    public static float InterpolationDuration, FeedforwardMultiplier, RewindMultiplier;
    public bool LeftHandMeshVisibility;
    public bool RightHandMeshVisibility;

    [Tooltip("Object containing reference to all tracked hands scripts.")]
    public HandContainerFF handContainerLeft, handContainerRight;
    public static Action<int, Interaction, Target, TargetDuplication, Ghost, Visualization, bool, int> OnFeedforward;
    public static Action<HandContainerFF> OnDisableHandTracking, OnDestroyHand;
    public static Action<HandContainerFF, bool> OnFinishFeedforward, OnSetHandVisibility;

    public static int DUPLICATE_HANDS_LAYER = 8, TRACKED_HANDS_LAYER = 9;

    public bool HandStatusLeft, HandStatusRight;
    [SerializeField]
    public List<BoneDisplayModel> HandDisplayBones;

    HandMode CurrentHandMode;

    bool RegisteredHandLeft, RegisteredHandRight;

    [Tooltip("Select an empty object in the scene to calculate all previewing movements relative to. Make sure the object does not move. This is important to keep the movements the same whenever there is a scene change, e.g. redrawing the play area.")]
    public GameObject RelativeTo;
    public float RecognitionThreshold = 0.1f;

    [SerializeField]
    [Tooltip("List of all available interactions.")]
    public List<Interaction> Interactions;

    [Tooltip("This is an empty interaction which is loaded with new recording when the user wants to record a new interactions. This interaction gets a new reference after a recording.")]
    Interaction currentInteraction;

    [Tooltip("Press A to record, and S to stop recording. F to play back recording")]
    public bool Record;

    //public bool Preview;
    bool Recorded;

    [ReadOnly]
    public bool interruptAnimation = false;
     [ReadOnly]
    public bool interruptRecoveryAnimation = false;
     [ReadOnly]
    public bool RecognizeGestureFlag = false;
     [ReadOnly]
    public bool poseCorutineActive = false;
     [ReadOnly]
    public bool recoveryCorutineActive = false;
    [Tooltip("How many gestures are in this recording")]
    int GestureCounter = 0;

    [Tooltip("Left hand pool containing ghosted hands. These hands are invisible but still track the main left hand outside of the feedforward.")]
    protected HandPoolManager LeftHandPool;
    [Tooltip("Right hand pool containing ghosted hands. These hands are invisible but still track the main right hand outside of the feedforward.")]
    protected HandPoolManager RightHandPool;

    private UIManager uiManager;
    //Rigidbody rbVisible, rbTracked;
    GameObject handStartVisible;
    enum HandMode
    {
        TrackedVisible,
        TrackedInvisible
    }
    [System.Serializable]
    public class BoneDisplayModel
    {
        [SerializeField]
        public string boneId;
        [SerializeField]
        public float positionX, positionY, positionZ;
        [SerializeField]
        public float rotationX, rotationY, rotationZ;

        public BoneDisplayModel()
        {
            boneId = "";
            positionX = positionY = positionZ = 0;
            rotationX = rotationY = rotationZ = 0;
        }

        public void SetOVRBoneValues(OVRBone bone)
        {
            boneId = bone.Id.ToString();
            positionX = bone.Transform.position.x;
            positionY = bone.Transform.position.y;
            positionZ = bone.Transform.position.z;
            rotationX = bone.Transform.rotation.x;
            rotationY = bone.Transform.rotation.y;
            rotationZ = bone.Transform.rotation.z;
        }
        public void SetHPTKBoneValues(Transform bone)
        {
            boneId = bone.name;
            positionX = bone.position.x;
            positionY = bone.position.y;
            positionZ = bone.position.z;
            rotationX = bone.rotation.x;
            rotationY = bone.rotation.y;
            rotationZ = bone.rotation.z;
        }

    }

    void Awake()
    {
        if (MovingParts == null)
            MovingParts = new List<GameObject>();

        LeftHandPool = GameObject.Find("LeftHandPoolParent").GetComponent<HandPoolManager>();
        RightHandPool = GameObject.Find("RightHandPoolParent").GetComponent<HandPoolManager>();

        ReloadEmptyInteraction(false);

        if (InterpolationDurationSet == 0) InterpolationDurationSet = 0.4f;
        if (FeedforwardMultiplierSet == 0) FeedforwardMultiplierSet = 1;
        if (RewindMultiplierSet == 0) FeedforwardMultiplierSet = 2;

        handContainerLeft.Status = HandStatus.TrackedVisible;
        handContainerRight.Status = HandStatus.TrackedVisible;

        // if (Handedness == OVRHand.Hand.HandLeft)
        // {
        //     handParent = handContainer.LeftParent.transform;
        //     handGrandParent = handParent.transform.parent;

        //     PhysicsHand = handContainer.PhysicsHandModelLeft;
        //     MasterHand = handContainer.MasterHandModelLeft;

        //     OVRHand = handContainer.LeftOVRHand.GetComponent<OVRHand>();
        //     handPhysicsModel = handContainer.PhysicsModelLeft;
        //     handInputModel = handContainer.InputModelLeft;
        //     inputController = handContainer.InputControllerLeft;
        //     physicsController = handContainer.PhysicsControllerLeft;
        //     proxyHandModel = handContainer.ProxyHandModelLeft;
        // }
        // else
        // {
        //     handParent = handContainer.RightParent.transform;
        //     handGrandParent = handParent.transform.parent;
        //     PhysicsHand = handContainer.PhysicsHandModelRight;
        //     MasterHand = handContainer.MasterHandModelRight;
        //     OVRHand = handContainer.RightOVRHand.GetComponent<OVRHand>();
        //     handPhysicsModel = handContainer.PhysicsModelRight;
        //     handInputModel = handContainer.InputModelRight;
        //     inputController = handContainer.InputControllerRight;
        //     physicsController = handContainer.PhysicsControllerRight;
        //     proxyHandModel = handContainer.ProxyHandModelRight;
        // }

        OnFeedforward += HandleFeedforward;
        OnDisableHandTracking += HandleDisableHandTracking;
        OnDestroyHand += HandleDestroyHand;
        OnFinishFeedforward += HandleFinishFeedforward;
        OnSetHandVisibility += HandleSetVisibility;
    }

    private void OnDisable()
    {

        OnFeedforward -= HandleFeedforward;
        OnDisableHandTracking -= HandleDisableHandTracking;
        OnDestroyHand -= HandleDestroyHand;
        OnFinishFeedforward -= HandleFinishFeedforward;
        OnSetHandVisibility -= HandleSetVisibility;
    }
    private void Start()
    {
        HandDisplayBones = new List<BoneDisplayModel>();
        HandDisplayBones.Add(new BoneDisplayModel());
        HandDisplayBones.Add(new BoneDisplayModel());
        RightHandMeshVisibility = false;
        LeftHandMeshVisibility = false;
        smartphone = GameObject.Find("smartphone");
        int leftHandCount = 0, rightHandCount = 0;
        foreach (Interaction i in Interactions)
        {
            i.AddListener();
            i.AddScripts();
            i.SetGhostActive(false);
            i.SetMovingPartsMaterials();
            if (i.Handedness == Hand.HandLeft)
                leftHandCount++;
            else
                rightHandCount++;

            Debug.Log("Left hands: " + leftHandCount + " - Right hands:" + rightHandCount);
        }

        //set collisions of all ff hands to false and set them to true only at ff time
        SetCollisionsHands(false, true);

    }
    public HandContainerFF GetLeftHandContainer()
    {
        return handContainerLeft;
    }
    public HandContainerFF GetRightHandContainer()
    {
        return handContainerRight;
    }
    public HandContainerFF GetMatchingHandContainer()
    {
        if (NewInteractionHandedness == Hand.HandLeft)
            return handContainerLeft;
        else
            return handContainerRight;
    }
    public HandContainerFF GetMatchingHandContainer(OVRHand.Hand handedness)
    {
        if (handedness == Hand.HandLeft)
            return handContainerLeft;
        else
            return handContainerRight;
    }

    public HandContainerFF GetMatchingHandContainer(Interaction i)
    {
        return GetMatchingHandContainer(i.Handedness);
    }


    HandContainerFF GetAvailableHandInPool(Interaction interaction)
    {
        HandContainerFF hand;
        if (interaction.Handedness == OVRHand.Hand.HandLeft)
        {
            hand = LeftHandPool.GetAvailableHand();

        }
        else
        {
            hand = RightHandPool.GetAvailableHand();
        }

        return hand;

    }

    GameObject smartphone;
    public Transform GetVisibleHandStart()
    {
        return handStartVisible.transform;
    }

    public Interaction GetInteraction(InteractionId id)
    {
        Interaction i = currentInteraction;
        foreach (Interaction j in Interactions)
            if (j.Id == id) i = j;
        return i;
    }
    public bool LeftHandIsValid()
    {
        return RegisteredHandLeft && handContainerLeft.OVRHand.IsTracked && handContainerLeft.OVRHand.HandConfidence == TrackingConfidence.High;
    }

    public bool RightHandIsValid()
    {
        return RegisteredHandRight && handContainerRight.OVRHand.IsTracked && handContainerRight.OVRHand.HandConfidence == TrackingConfidence.High;
    }

    void HandleDestroyHand(HandContainerFF hc)
    {
        if (hc != null)
            Destroy(hc.gameObject);
    }

    //Reload new empty interaction as the current one and make the trigger the object attached to the controller.
    void ReloadEmptyInteraction(bool reloadMovingParts)
    {
        if (reloadMovingParts)
        {
            List<GameObject> NewMovingParts = new List<GameObject>(MovingParts);
            MovingParts = NewMovingParts;

        }

        currentInteraction = new Interaction();

        currentInteraction.Handedness = NewInteractionHandedness;
        currentInteraction.SetMovingParts(MovingParts, GhostMovingParts);
        currentInteraction.SetGhost(GhostObjectDuplicate);
        currentInteraction.RelativeTo = RelativeTo;
        currentInteraction.Id = NewInteractionId;

    }

    void HandleDisableHandTracking(HandContainerFF hc)
    {
        SetTrackingMasterHand(false, hc);
    }


    //the physics/visible  hand tracks the master hand
    void SetTrackingMasterHand(bool value, HandContainerFF hc)
    {
        hc.InputModel.isActive = value;
        hc.ProxyHandModel.updateValuesForMaster = value;
    }
    void SetTrackingPhysicsHand(bool value, HandContainerFF hc)
    {
        hc.PhysicsModel.isActive = value;
        hc.ProxyHandModel.updateValuesForSlave = value;
    }

    void UpdateDisplayBones()
    {
        HandDisplayBones[0].SetHPTKBoneValues(handContainerLeft.MasterHandModel.allTransforms[3]);
        HandDisplayBones[1].SetHPTKBoneValues(handContainerLeft.PhysicsHandModel.allTransforms[3]);
    }
    public IEnumerator RunDelayedAction(float delay, System.Action myMethodName)
    {
        if (myMethodName == StartRecording)
            uiManager.SetText("Starting...");

        yield return new WaitForSeconds(delay);

        myMethodName();
    }


    public void StartRecording()
    {

        Record = true;
        VisualDebugClass.SetMessage("Recording gesture..");
        Debug.Log("Record");

    }

    public void StopRecording()
    {

        Record = false;
        VisualDebugClass.SetMessage("");
        Debug.Log("Finished recording");

    }

    public bool isRecording()
    {
        return Record;
    }

    bool TriggerInitializeCollisionManager = false;

    public float GetVisibleHandVelocity()
    {
        return 0f;
    }

    private void FixedUpdate()
    {
        if (Record)
        {
            RecordGesture(GetMatchingHandContainer());
        }
        else
        {
            if (GestureCounter > 0)
            {
                SaveInteraction();
            }
            GestureCounter = 0;
        }
    }

     void DisappearHandsIfLowConfidence()
    {
        bool left = handContainerLeft.OVRHand.IsDataHighConfidence;
        bool right = handContainerRight.OVRHand.IsDataHighConfidence;

        handContainerLeft.MeshObject.GetComponent<SkinnedMeshRenderer>().enabled = left;
        handContainerRight.MeshObject.GetComponent<SkinnedMeshRenderer>().enabled = right;
        if(smartphone!=null)
            smartphone.SetActive(left);
    }

    void Update()
    {
        HandStatusLeft = LeftHandIsValid();
        HandStatusRight = RightHandIsValid();

        InterpolationDuration = InterpolationDurationSet;
        FeedforwardMultiplier = FeedforwardMultiplierSet;
        RewindMultiplier = RewindMultiplierSet;

        DisappearHandsIfLowConfidence();

        //Don't record or play gestures while the system gesture is in progress
        if (handContainerLeft.OVRHand.IsSystemGestureInProgress)
            return;
        if (handContainerRight.OVRHand.IsSystemGestureInProgress)
            return;
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Pressed A ");
            StartRecording();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Pressed S");
            StopRecording();
        }

        //play interaction with collisions disabled
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayAndRewindLastAddedInteraction(Target.All);
        }



        if (LeftHandMeshVisibility)
        {
            handContainerLeft.DisableMesh();
        }

        if (RightHandMeshVisibility)
        {
            handContainerRight.DisableMesh();

        }

    }

    bool TriggerPoseChange = false;

    public void PlayLastAddedInteraction(Target locus)
    {
        HandContainerFF hc = GetMatchingHandContainer(Interactions[Interactions.Count - 1]);
        if (Interactions.Count > 0)
            StartCoroutine(Interactions[Interactions.Count - 1].PlayInteraction(-1, hc, Target.All, TargetDuplication.None, Ghost.None, Visualization.None, false));

    }
    public void PlayLastAddedInteractionDuplicate(Target locus)
    {
        HandContainerFF hc = GetMatchingHandContainer(Interactions[Interactions.Count - 1]);
        if (Interactions.Count > 0)
            StartCoroutine(Interactions[Interactions.Count - 1].PlayInteraction(-1, hc, Target.All, TargetDuplication.All, Ghost.None, Visualization.None, false));
    }

    public void PlayAndRewindLastAddedInteraction(Target locus)
    {
        HandContainerFF hc = GetMatchingHandContainer(Interactions[Interactions.Count - 1]);
        if (Interactions.Count > 0)
            StartCoroutine(Interactions[Interactions.Count - 1].PlayInteraction(-1, hc, Target.All, TargetDuplication.None, Ghost.None, Visualization.None, true));

    }

    void SaveInteraction()
    {

        if (currentInteraction.RelativeTo == null)
        {
            throw new Exception("RelativeTo should be non null. Add any object in scene that is static and persists.");
        }

        Interactions.Add(currentInteraction);
        ReloadEmptyInteraction(true);
        Debug.Log("Finished recording gesture....");
    }

    void RecordGesture(HandContainerFF hc)
    {
        currentInteraction.RecordGesture(hc.PhysicsHandModel);
        GestureCounter++;
    }

    Interaction getInteraction(InteractionId id)
    {
        foreach (Interaction i in Interactions)
        {
            if (i.Id == id)
                return i;

        }
        return Interactions[0];
    }

    public static void SetCollisionsHands(bool value, bool duplicated)
    {
        //0 is default layer
        //8 is feedforward hands layer
        //9 is tracked hands layer
        int l = LayerMask.NameToLayer("TrackedHands");
        int CarBracket = LayerMask.NameToLayer("NoCarCollide");
        if (duplicated) l = LayerMask.NameToLayer("FeedforwardHands");
        Debug.Log("Set collisions between layer " + l + " to: " + value);
    }



    void HandleFeedforward(int TriggerId, Interaction interaction, Target target, TargetDuplication duplicate, Ghost ghostType, Visualization vis, bool rewind, int repetitions) // everything duplicated is ghost but  objects can have ghost meshes and not be duplicated no?
    { //here get the main tracked hands before sending the ff further
        HandContainerFF hc = null;
        switch (duplicate)
        {
            case TargetDuplication.All: //get hand from pool
                if (interaction.Handedness == Hand.HandLeft)
                {
                    hc = LeftHandPool.GetAvailableHand();
                    if (vis == Visualization.Lens && FFWindowLeft != null)
                        FFWindowLeft.SetActive(true);
                        Debug.Log("Lens option not available");
                }
                else
                {
                    hc = RightHandPool.GetAvailableHand();
                    if (vis == Visualization.Lens && FFWindowRight != null)
                        FFWindowRight.SetActive(true);
                        Debug.Log("Lens option not available");
                }
                if (hc != null)
                    hc.SetMeshVisibility(true);
                else
                    return; // no more hands do not start ff
                break;
            case TargetDuplication.None: // embodied ff, may be on tracked hand so send it further
                if (interaction.Handedness == Hand.HandLeft)
                {
                    hc = handContainerLeft;
                }
                else
                {
                    hc = handContainerRight;
                }
                break;
            case TargetDuplication.Avatar:
                Debug.Log("GETTING HAND FROM POOL");
                if (interaction.Handedness == Hand.HandLeft)
                {
                    hc = LeftHandPool.GetAvailableHand();
                }
                else
                {
                    hc = RightHandPool.GetAvailableHand();
                }
                if (hc != null)
                    hc.SetMeshVisibility(true);
                else
                    return; // no more hands do not start ff
                break;
            case TargetDuplication.Objects: //If objects are duplicated then we need ff lens if it exists and original hand
                if (interaction.Handedness == Hand.HandLeft)
                {
                    hc = handContainerLeft;
                    if (vis == Visualization.Lens && FFWindowLeft != null)
                        FFWindowLeft.SetActive(true);
                }
                else
                {
                    hc = handContainerRight;
                    if (vis == Visualization.Lens && FFWindowRight != null)
                        FFWindowRight.SetActive(true);
                }

                break;
        }


        PlayInteraction(TriggerId, interaction, hc, target, duplicate, ghostType, vis, rewind, repetitions);
      
        StartCoroutine(TakeUserToFeedforward(hc));
    }

    IEnumerator TakeUserToFeedforward(HandContainerFF hc)
    {
        Transform Head = GameObject.Find("OVRCameraRig").transform;
        GameObject TargetGO = GameObject.Find("FFPerspective");

        if(TargetGO==null) yield break;
        Transform Target = TargetGO.transform;
        Vector3 initPost = new Vector3(Head.transform.position.x,Head.transform.position.y,Head.transform.position.z);
        Quaternion initRot = new Quaternion( Head.transform.rotation.x,Head.transform.rotation.y, Head.transform.rotation.z,1);

        float elapsedTime = 0;
        float duration = 1f;
        float ratio = elapsedTime / duration;

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;

            Debug.Log("IN WAIT going to interactions");

            Head.position = Vector3.Lerp(Head.position, Target.position, ratio);
            Head.rotation = Quaternion.Lerp(Head.rotation, Target.rotation,  ratio);

            yield return null;
        }
        yield return new WaitUntil(() => hc.Status == HandStatus.ToTrackedHand);

        elapsedTime = 0;
        ratio = elapsedTime / duration;

        Debug.Log("GOING back");

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;

            Head.position = Vector3.Lerp(Head.position, initPost, ratio);
            Head.rotation = Quaternion.Lerp(Head.rotation, initRot, ratio);

            yield return null;
        }

        yield return null;
    }



    void RestorePostFFStates(HandContainerFF hc, bool duplicateHand)
    {
        SetCollisionsHands(true, duplicateHand);
        SetTrackingMasterHand(true, hc);
        if (FFWindowLeft != null)
            FFWindowLeft.SetActive(false);
        if (FFWindowRight != null)
            FFWindowRight.SetActive(false);
        if (duplicateHand)
        {
            hc.SetMeshVisibility(false);
            hc.Status = HandStatus.TrackedInvisible;

        }
        else
        {
            hc.Status = HandStatus.TrackedVisible;

        }
    }

    void HandleFinishFeedforward(HandContainerFF hc, bool duplicateHand)
    {
        RestorePostFFStates(hc, duplicateHand);

    }

    void HandleSetVisibility(HandContainerFF hc, bool value)
    {
        hc.SetMeshVisibility(value);
    }
    void PlayInteraction(int TriggerId, Interaction interaction, HandContainerFF hc, Target target, TargetDuplication duplicate, Ghost ghostType, Visualization vis, bool rewind, int repetitions)
    {
        Debug.Log("Playing interaction from " + TriggerId);
        StartCoroutine(interaction.PlayInteraction(TriggerId, hc, target, duplicate, ghostType, vis, rewind, repetitions));
    }


    // public void LoadFirstGestures()
    // {
    //     HandContainerFF hc;

    //     if (Interactions.Count > 0)
    //     {
    //         Interaction interaction = Interactions[0];
    //         //handAnchor.rotation = BoneTransform.GetGlobalRotation(interaction.getAnchor().getFingerData()[0].boneRotation, interaction.Trigger);
    //         //load gesture at i
    //         if (interaction.Handedness == Hand.HandLeft)
    //             hc = handContainerLeft;
    //         else
    //             hc = handContainerRight;

    //         StartCoroutine(interaction.LoadFirstGesture(hc));

    //     }
    //     //hands.LeftVisibleHandPrefab.transform.SetParent(handGrandParent);

    // }

    bool TrackHandMeshEnabled;
    void ChangeHandState()
    {
        switch (CurrentHandMode)
        {
            case HandMode.TrackedInvisible:
                SetHandVisible(true);
                break;
            case HandMode.TrackedVisible:
                SetHandVisible(false);
                break;
            default:
                break;
        }
    }
    void SetHandVisible(bool state)
    {
        if (state)
        {
            Debug.Log("Changing mode to visible");
            CurrentHandMode = HandMode.TrackedVisible;
            TrackHandMeshEnabled = true;
        }
        else
        {
            Debug.Log("Changing mode to invisible");
            CurrentHandMode = HandMode.TrackedInvisible;
            TrackHandMeshEnabled = false;
        }

    }


}
