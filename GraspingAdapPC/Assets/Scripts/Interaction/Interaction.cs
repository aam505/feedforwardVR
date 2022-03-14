using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HPTK.Models.Avatar;
using HPTK.Controllers.Avatar;
using HPTK.Controllers.Input;
using System;

public enum InteractionId
{
    New,
    Left_StickWheelGear,
    Right_Wheel,
    NormanDoor,
    Fridge,
    Oven,
    Pot,
    Airfrier,
    Cup,
    Microwave,
    WashingMachine,
    Lights
}

public enum RecordingType
{
    RelativeTo,
    Global
}

public enum InteractionStatus
{
    PlayingDuplicated,
    PlayingTracked,
    None,
    AwaitingTrigger,
    Starting, //going from tracked hand to first gesture
    AwaitingUntrigger,
    AwaitingRewind,
    RewidingDuplicated,
    RewindingTracked,
    Interrupted // going from the last gesture to the tracked hand?
}
[System.Serializable]
public class Interaction
{
    //would add triggertype
    //public GestureId Id;
    public InteractionId Id;
    public InteractionStatus Status;
    [SerializeField]
    private List<Gesture> Gestures;
    float StartedPlayingTime = 0;

    [Tooltip("Set before recoding.5Objects involved in the feedforward that move and need to be restored to the initial state.")]

    [SerializeField]
    public List<GameObject> MovingPartsGameObject = new List<GameObject>();
    public GameObject GhostObjectDuplicate;
    public List<GameObject> GhostMovingParts = new List<GameObject>();

    [SerializeField]
    public List<FeedforwardObjectState> MovingPartsStates = new List<FeedforwardObjectState>();
    public static Action<InteractionId> OnInterrupted;
    //To see what type of interaction is for developer without recording strings
    private List<List<Material>> TriggerObjectMovingPartsMaterials = new List<List<Material>>();
    // Start is called before the first frame update
    [SerializeField]
    public GameObject RelativeTo;
    public bool Active = false;
    public OVRHand.Hand Handedness;


    public Interaction()
    {
        Gestures = new List<Gesture>();
        Status = InteractionStatus.None;
    }

    public void HandleInterrupt(InteractionId id)
    {
        if (id == Id && (Status == InteractionStatus.PlayingDuplicated || Status == InteractionStatus.RewidingDuplicated))
        {
            float delay = Time.time - StartedPlayingTime;
            if (delay > 1)
            {
                Status = InteractionStatus.Interrupted;
                Debug.Log("Interrupted interaction " + Id);
            }
        }
    }
    public void SetMovingParts(List<GameObject> movingParts, List<GameObject> ghostMovingParts)
    {
        this.MovingPartsGameObject = movingParts;
        this.GhostMovingParts = ghostMovingParts; //all scripts and rigid bodies and colliders should be removed before this step manually

        foreach (var movingPart in movingParts)
        {
            THandCollisionDetection script;
            script = movingPart.GetComponent<THandCollisionDetection>();
            if (script == null)
                script = movingPart.AddComponent<THandCollisionDetection>();

            script.AddId(Id);
        }

        SetMovingPartsMaterials();
    }


    public void SetMovingPartsMaterials()
    {
        foreach (GameObject movingPart in MovingPartsGameObject)
        {
            List<Material> MovingPartsMaterials = new List<Material>();

            for (int i = 0; i < movingPart.GetComponent<Renderer>().sharedMaterials.Length; i++)
                MovingPartsMaterials.Add(movingPart.GetComponent<Renderer>().sharedMaterials[i]);

            TriggerObjectMovingPartsMaterials.Add(MovingPartsMaterials);
        }
    }
    public void SetGhostActive(bool value)
    {
        Debug.Log("Setting ghost active");
        foreach (var movingPart in this.GhostMovingParts)
        {
            movingPart.SetActive(value);
        }
    }
    public void SetGhost(GameObject ghost)
    {
        GhostObjectDuplicate = ghost;
    }

    public void AddScripts()
    {

        foreach (var movingPart in MovingPartsGameObject)
        {
            THandCollisionDetection script;
            script = movingPart.GetComponent<THandCollisionDetection>();
            if (script == null)
                script = movingPart.AddComponent<THandCollisionDetection>();
            script.AddId(Id);
        }
    }
    public void AddListener()
    {
        OnInterrupted += HandleInterrupt;
    }
    public void RemoveListener()
    {
        OnInterrupted -= HandleInterrupt;
    }
    public void AddTransformData(List<BoneTransform> boneData, List<ObjectTransform> objectData)
    {
        Gesture newGesture = new Gesture();
        newGesture.setFingerData(boneData);
        Gestures.Add(newGesture);

        FeedforwardObjectState states = new FeedforwardObjectState(objectData);
        MovingPartsStates.Add(states);

    }
    public void AddTransformData(List<BoneTransform> boneData)
    {
        Gesture newGesture = new Gesture();
        newGesture.setFingerData(boneData);
        Gestures.Add(newGesture);
    }

    public Gesture getFirstGesture()
    {
        return Gestures[0];
    }

    public List<Gesture> getGestures()
    {
        return Gestures;
    }


    //todo add recording of objects optional
    public void RecordGesture(HandModel MasterHand, bool rewind = false)
    {
        //Debug.Log("Recording gesture ");

        List<BoneTransform> boneData = new List<BoneTransform>();


        for (int i = 0; i < MasterHand.allTransforms.Length; i++)
        {
            var boneTransform = MasterHand.allTransforms[i];

            BoneTransform fingerTransformData
                = new BoneTransform(i, BoneTransform.GetRelativePosition(boneTransform.position, RelativeTo), BoneTransform.GetRelativeRotation(boneTransform.rotation, RelativeTo));
            //BoneTransform fingerTransformData = new BoneTransform(bone.Id, bone.Transform.position, bone.Transform.rotation);
            boneData.Add(fingerTransformData);
        }

        List<ObjectTransform> ObjectData = new List<ObjectTransform>();

        for (int i = 0; i < MovingPartsGameObject.Count; i++)
        {
            GameObject o = MovingPartsGameObject[i];
            ObjectData.Add(new ObjectTransform(BoneTransform.GetRelativePosition(o.transform.position, RelativeTo), BoneTransform.GetRelativeRotation(o.transform.rotation, RelativeTo)));
        }

        AddTransformData(boneData, ObjectData);
    }

    void SetMovingObjectsScripts(bool value)
    {
        foreach (GameObject movingPart in MovingPartsGameObject)
        {
            foreach (MonoBehaviour script in movingPart.transform.GetComponents<MonoBehaviour>())
            {
                if (!(script is Trigger) && !(script is THandCollisionDetection) && !(script is LocalSaceConstraints) && !(script is LightsOnScript)
                && !(script is CarButton))
                {
                    Debug.Log(script);
                    script.enabled = value;
                }
            }
        }
    }
    public void SetGhostObjectsVisibilityCoroutine(bool value)
    {
        Debug.Log("Setting ghost renderer to " + value);

        for (int i = 0; i < GhostMovingParts.Count; i++)
        {
            GameObject movingPartGhost = GhostMovingParts[i];
            movingPartGhost.GetComponent<Renderer>().enabled = value;
        }

    }

    //if lags change to coroutine
    public void ChangeMovingPartsMat(bool ghost)
    {
        Debug.Log("Changing moving parts materials");
        for (int i = 0; i < MovingPartsGameObject.Count; i++)
        {
            GameObject movingPart = MovingPartsGameObject[i];

            if (ghost)  //change to ghost
            {
                int len = TriggerObjectMovingPartsMaterials[i].Count;
                Material[] matsOther = new Material[3];
                matsOther[0] = HandPoolManager.GhostsBaseMaterial; 
                matsOther[1] = HandPoolManager.GhostsMaterial; 
                matsOther[2] = HandPoolManager.GhostsMaterial; 
          
                movingPart.GetComponent<Renderer>().sharedMaterials = matsOther;
                Debug.Log("Set objects transparent. ");
            }
            else  //change to not ghost
            {
                int len = TriggerObjectMovingPartsMaterials[i].Count;
                Material[] matsOther = new Material[len];
                for (int j = 0; j < TriggerObjectMovingPartsMaterials[i].Count; j++)
                {
                    matsOther[j] = TriggerObjectMovingPartsMaterials[i][j];
                }
                movingPart.GetComponent<Renderer>().sharedMaterials = matsOther;
                Debug.Log("Set objects original materials.");
            }

        }
    }

    public IEnumerator InterpolateToInteraction(HandContainerFF hc)
    {
        float InterpolationDuration = HandInteractionManager.InterpolationDuration;

        HandModel MasterHand = hc.MasterHandModel;
        hc.Status = HandStatus.ToInteraction;

        Debug.Log("Moving To interaction");

        float elapsedTime = 0;
        float ratio = elapsedTime / InterpolationDuration;

        Gesture target = getFirstGesture();

        List<BoneTransform> initialTrackedGesture = new List<BoneTransform>(MasterHand.allTransforms.Length);

        for (int i = 0; i < MasterHand.allTransforms.Length; i++)
        {

            Transform bone = MasterHand.allTransforms[i];
            initialTrackedGesture.Add(new BoneTransform(i, bone.position, bone.rotation));
        }

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / InterpolationDuration;

            for (int i = 0; i < MasterHand.allTransforms.Length; i++)
            {

                Vector3 globalPosition = BoneTransform.GetGlobalPosition(target.getFingerData()[i].bonePosition, RelativeTo);
                Quaternion globalRotation = BoneTransform.GetGlobalRotation(target.getFingerData()[i].boneRotation, RelativeTo);

                MasterHand.allTransforms[i].position = Vector3.Lerp(initialTrackedGesture[i].bonePosition, globalPosition, ratio);
                MasterHand.allTransforms[i].rotation = Quaternion.Lerp(initialTrackedGesture[i].boneRotation, globalRotation, ratio);
            }
            yield return null;
        }
    }

    public IEnumerator InterpolateToTrackedHand(HandContainerFF hc)
    {
        Debug.Log("Back to tracked hand");

        float InterpolationDuration = HandInteractionManager.InterpolationDuration;
        HandModel MasterVisibleHand = hc.MasterHandModel;
        OVRSkeleton TrackedHand = hc.Skeleton;

        hc.Status = HandStatus.ToTrackedHand;

        float elapsedTime = 0;
        float ratio = elapsedTime / InterpolationDuration;

        List<BoneTransform> visibleHandGesture = new List<BoneTransform>(MasterVisibleHand.allTransforms.Length);

        for (int i = 0; i < MasterVisibleHand.allTransforms.Length; i++)
        {

            Transform item = MasterVisibleHand.allTransforms[i];
            visibleHandGesture.Add(new BoneTransform(i, item.position, item.rotation));
        }

        List<BoneTransform> TrackedHandGesture = new List<BoneTransform>(TrackedHand.Bones.Count);

        for (int i = 0; i < TrackedHand.Bones.Count; i++)
        {

            Transform item = TrackedHand.Bones[i].Transform;
            TrackedHandGesture.Add(new BoneTransform(i, item.position, item.rotation));
        }

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / InterpolationDuration;

            for (int i = 0; i < MasterVisibleHand.allTransforms.Length - 5; i++)
            {

                MasterVisibleHand.allTransforms[i].position = Vector3.Lerp(visibleHandGesture[i].bonePosition, TrackedHandGesture[i].bonePosition, ratio);
                MasterVisibleHand.allTransforms[i].rotation = Quaternion.Lerp(visibleHandGesture[i].boneRotation, TrackedHandGesture[i].boneRotation, ratio);
            }

            yield return null;

        }
    }


    //call with triggerid=-1 to play interaction not tied to trigger
    public IEnumerator PlayInteraction(int TriggerId, HandContainerFF hc, Target target, TargetDuplication duplicate, Ghost GhostType, Visualization vis, bool rewind, int repetitions = 1)
    {
        HandModel MasterHand = hc.MasterHandModel;
        StartedPlayingTime = Time.time;
        List<GameObject> movingParts = MovingPartsGameObject;
        bool duplicateHands = false;
        InteractionStatus playingStatus=InteractionStatus.PlayingTracked, rewindingStatus=InteractionStatus.RewindingTracked;

        //if avatar is duplicated, hc is ghost hand
        if(duplicate == TargetDuplication.All || duplicate == TargetDuplication.Avatar){
            duplicateHands = true;
             playingStatus=InteractionStatus.PlayingDuplicated;
             rewindingStatus=InteractionStatus.RewidingDuplicated;
        }

       

        hc.Status = HandStatus.ToInteraction;
        this.Status = InteractionStatus.Starting;
        bool interpolate = false
        ;
        //if hand is part of feedforward
        if (target == Target.All || target == Target.Avatar)
        {
            interpolate = true;
            HandInteractionManager.SetCollisionsHands(false, duplicateHands); //disable collisions with correct hand so that the ff plays out and its not creating strange interactions
            HandInteractionManager.OnDisableHandTracking(hc); //disable hand tracking so the ff can move the hand
        }
        //if object is part of feedforward
        if (target == Target.All || target == Target.Object)
        {
            //Physics.IgnoreLayerCollision(0, 0, true);  may need to disable colliders of objects in ff
            if(duplicate!=TargetDuplication.All && duplicate!=TargetDuplication.Objects)
                SetMovingObjectsScripts(false); //disable scripts if the object is NOT duplciated

            if (duplicate == TargetDuplication.All || duplicate == TargetDuplication.Objects) //if object is duplicated, enable duplicated ghosts, by default duplicate are ghosted
            {
                movingParts = GhostMovingParts;
                SetGhostActive(true);
            }
            else // object is not duplicated check if it has to be ghosted = duplication none or avatar
            {
                if (GhostType == Ghost.All || GhostType == Ghost.Object)
                    ChangeMovingPartsMat(true);
                if (GhostType == Ghost.All || GhostType == Ghost.Avatar && duplicate != TargetDuplication.Avatar)
                    hc.ChangeToGhostMaterial();

            }
        }

        if (interpolate) //IF AVATAR is involved move the avatar there if not leave it
        {
            Debug.Log("Moving To interaction");
            float elapsedTime = 0;
            float ratio = elapsedTime / HandInteractionManager.InterpolationDuration;

            Gesture firstGesture = getFirstGesture();

            List<BoneTransform> initialTrackedGesture = new List<BoneTransform>(MasterHand.allTransforms.Length);

            for (int i = 0; i < MasterHand.allTransforms.Length; i++)
            {

                Transform bone = MasterHand.allTransforms[i];
                initialTrackedGesture.Add(new BoneTransform(i, bone.position, bone.rotation));
            }

            while (ratio < 1f)
            {
                elapsedTime += Time.deltaTime;
                ratio = elapsedTime / HandInteractionManager.InterpolationDuration;

                for (int i = 0; i < MasterHand.allTransforms.Length; i++)
                {

                    Vector3 globalPosition = BoneTransform.GetGlobalPosition(firstGesture.getFingerData()[i].bonePosition, RelativeTo);
                    Quaternion globalRotation = BoneTransform.GetGlobalRotation(firstGesture.getFingerData()[i].boneRotation, RelativeTo);

                    MasterHand.allTransforms[i].position = Vector3.Lerp(initialTrackedGesture[i].bonePosition, globalPosition, ratio);
                    MasterHand.allTransforms[i].rotation = Quaternion.Lerp(initialTrackedGesture[i].boneRotation, globalRotation, ratio);
                }
                yield return null;
            }

        }

        Debug.Log("At interaction, starting playing with multiplyer" + HandInteractionManager.FeedforwardMultiplier);
        this.Status = playingStatus;
        hc.Status = HandStatus.Feedforward;

        for (int r = 0; r < repetitions; r++)
        {
            string msg = String.Format("T{2}: playing interaction {0}/{1}", (r + 1), repetitions, TriggerId);

            Debug.Log(msg);
            VisualDebugClass.SetMessage(msg);

            switch (target)
            {
                case (Target.All):

                    for (int j = 0; j < getGestures().Count; j++)
                    {
                        Gesture gesture = getGestures()[j];
                        for (int i = 0; i < MasterHand.allTransforms.Length; i++)
                        {
                            MasterHand.allTransforms[i].position = BoneTransform.GetGlobalPosition(gesture.getFingerData()[i].bonePosition, RelativeTo);
                            MasterHand.allTransforms[i].rotation = BoneTransform.GetGlobalRotation(gesture.getFingerData()[i].boneRotation, RelativeTo);
                        }

                        for (int l = 0; l < movingParts.Count; l++)
                        {
                            GameObject o = movingParts[l];
                            o.transform.position = BoneTransform.GetGlobalPosition(MovingPartsStates[j].States[l].ObjectPosition, RelativeTo);
                            o.transform.rotation = BoneTransform.GetGlobalRotation(MovingPartsStates[j].States[l].ObjectRotation, RelativeTo);
                        }

                        if (Status == InteractionStatus.Interrupted)
                            break;
                        if (j % HandInteractionManager.FeedforwardMultiplier == 0)
                            yield return null;
                    }
                    break;
                case (Target.Avatar):

                    for (int j = 0; j < getGestures().Count; j++)
                    {
                        Gesture gesture = getGestures()[j];
                        for (int i = 0; i < MasterHand.allTransforms.Length; i++)
                        {
                            MasterHand.allTransforms[i].position = BoneTransform.GetGlobalPosition(gesture.getFingerData()[i].bonePosition, RelativeTo);
                            MasterHand.allTransforms[i].rotation = BoneTransform.GetGlobalRotation(gesture.getFingerData()[i].boneRotation, RelativeTo);
                        }
                        if (j % HandInteractionManager.FeedforwardMultiplier == 0)
                            yield return null;
                        if (Status == InteractionStatus.Interrupted)
                            break;
                    }
                    break;
                case (Target.Object):

                    for (int j = 0; j < getGestures().Count; j++)
                    {
                        for (int l = 0; l < movingParts.Count; l++)
                        {
                            GameObject o = movingParts[l];
                            o.transform.position = BoneTransform.GetGlobalPosition(MovingPartsStates[j].States[l].ObjectPosition, RelativeTo); //check if object states are in this order?
                            o.transform.rotation = BoneTransform.GetGlobalRotation(MovingPartsStates[j].States[l].ObjectRotation, RelativeTo);
                        }
                        if (j % HandInteractionManager.FeedforwardMultiplier == 0)
                            yield return null;
                        if (Status == InteractionStatus.Interrupted)
                            break;
                    }
                    break;
            }


            //this.Status = InteractionStatus.AwaitingRewind;
            //yield return new WaitForSeconds(1f);

            if (rewind && (Status != InteractionStatus.Interrupted)) //l do not rewind if the interraction was interrupted 
            {
                VisualDebugClass.AddMessage("..rewiding");
                Debug.Log("..rewinding +T" + TriggerId);

                this.Status = rewindingStatus;

                switch (target)
                {
                    case (Target.All):
                        for (int j = getGestures().Count - 1; j >= 0; j--)
                        {
                            Gesture gesture = getGestures()[j];

                            //load all hand bones into the gesture
                            for (int i = 0; i < MasterHand.allTransforms.Length; i++)
                            {
                                MasterHand.allTransforms[i].position = BoneTransform.GetGlobalPosition(gesture.getFingerData()[i].bonePosition, RelativeTo);
                                MasterHand.allTransforms[i].rotation = BoneTransform.GetGlobalRotation(gesture.getFingerData()[i].boneRotation, RelativeTo);
                            }

                            for (int l = 0; l < movingParts.Count; l++)
                            {
                                GameObject o = movingParts[l];
                                o.transform.position = BoneTransform.GetGlobalPosition(MovingPartsStates[j].States[l].ObjectPosition, RelativeTo); //check if object states are in this order?
                                o.transform.rotation = BoneTransform.GetGlobalRotation(MovingPartsStates[j].States[l].ObjectRotation, RelativeTo);
                            }
                            if (Status == InteractionStatus.Interrupted)
                                break;
                            if (j % HandInteractionManager.RewindMultiplier == 0)
                                yield return null;
                        }

                        break;
                    case (Target.Avatar):
                        for (int j = getGestures().Count - 1; j >= 0; j--)
                        {
                            Gesture gesture = getGestures()[j];

                            //load all hand bones into the gesture
                            for (int i = 0; i < MasterHand.allTransforms.Length; i++)
                            {
                                MasterHand.allTransforms[i].position = BoneTransform.GetGlobalPosition(gesture.getFingerData()[i].bonePosition, RelativeTo);
                                MasterHand.allTransforms[i].rotation = BoneTransform.GetGlobalRotation(gesture.getFingerData()[i].boneRotation, RelativeTo);

                            }
                            if (Status == InteractionStatus.Interrupted)
                                break;
                            if (j % HandInteractionManager.RewindMultiplier == 0)
                                yield return null;
                        }

                        break;
                    case (Target.Object):

                        for (int j = getGestures().Count - 1; j >= 0; j--)
                        {
                            for (int l = 0; l < movingParts.Count; l++)
                            {
                                GameObject o = movingParts[l];
                                o.transform.position = BoneTransform.GetGlobalPosition(MovingPartsStates[j].States[l].ObjectPosition, RelativeTo); //check if object states are in this order?
                                o.transform.rotation = BoneTransform.GetGlobalRotation(MovingPartsStates[j].States[l].ObjectRotation, RelativeTo);
                            }
                            if (Status == InteractionStatus.Interrupted)
                                break;
                            if (j % HandInteractionManager.RewindMultiplier == 0)
                                yield return null;
                        }
                        break;
                }
            }

            yield return new WaitForSeconds(0.1f);

            if (Status != InteractionStatus.Interrupted)
                this.Status = InteractionStatus.None;

            if (TriggerId > 0)
                Trigger.OnFinishedRepetition(TriggerId);
        }

        if (rewind == false || Status == InteractionStatus.Interrupted)
        {  //if there is no rewind or it has been interrupted return system to initial state
            for (int l = 0; l < movingParts.Count; l++)
            {
                GameObject o = movingParts[l];
                o.transform.position = BoneTransform.GetGlobalPosition(MovingPartsStates[0].States[l].ObjectPosition, RelativeTo);
                o.transform.rotation = BoneTransform.GetGlobalRotation(MovingPartsStates[0].States[l].ObjectRotation, RelativeTo);
            }
        }
        if (Status == InteractionStatus.None)
        {
            VisualDebugClass.SetMessageTemporary(1, "..stopping");
            Debug.Log("...stopping T" + TriggerId);
        }
        else
        {
            VisualDebugClass.SetMessageTemporary(1, "..interrupted");
            Debug.Log("...interrupted T" + TriggerId);
        }

        Status = InteractionStatus.None;

        StartedPlayingTime = 0;

        if (interpolate)
        {
            Debug.Log("Back to tracked hand");

            HandModel MasterVisibleHand = hc.MasterHandModel;
            OVRSkeleton TrackedHand = hc.Skeleton;


            hc.Status = HandStatus.ToTrackedHand;

            float elapsedTime = 0;
            float ratio = elapsedTime / HandInteractionManager.InterpolationDuration;

            List<BoneTransform> visibleHandGesture = new List<BoneTransform>(MasterVisibleHand.allTransforms.Length);

            for (int i = 0; i < MasterVisibleHand.allTransforms.Length; i++)
            {

                Transform item = MasterVisibleHand.allTransforms[i];
                visibleHandGesture.Add(new BoneTransform(i, item.position, item.rotation));
            }

            List<BoneTransform> TrackedHandGesture = new List<BoneTransform>(TrackedHand.Bones.Count);

            for (int i = 0; i < TrackedHand.Bones.Count; i++)
            {

                Transform item = TrackedHand.Bones[i].Transform;
                TrackedHandGesture.Add(new BoneTransform(i, item.position, item.rotation));
            }

            while (ratio < 1f)
            {
                elapsedTime += Time.deltaTime;
                ratio = elapsedTime / HandInteractionManager.InterpolationDuration;

                for (int i = 0; i < MasterVisibleHand.allTransforms.Length - 5; i++)
                {

                    MasterVisibleHand.allTransforms[i].position = Vector3.Lerp(visibleHandGesture[i].bonePosition, TrackedHandGesture[i].bonePosition, ratio);
                    MasterVisibleHand.allTransforms[i].rotation = Quaternion.Lerp(visibleHandGesture[i].boneRotation, TrackedHandGesture[i].boneRotation, ratio);
                }

                yield return null;

            }
        }
        //restore scripts
        if(duplicate!=TargetDuplication.All && duplicate!=TargetDuplication.Objects)
            SetMovingObjectsScripts(true);
            
        if (duplicate == TargetDuplication.All || duplicate == TargetDuplication.Objects) //if object is duplicated, enable duplicated ghosts, by default duplicate are ghosted
        {
            SetGhostActive(false); //disable ghosts of they were duplicated
        }
        else // object is not duplicated check if it has to be ghosted
        {
            if (GhostType == Ghost.All || GhostType == Ghost.Object)
                ChangeMovingPartsMat(false); //change material of original object back to normal
            if (GhostType == Ghost.All || GhostType == Ghost.Avatar && duplicate != TargetDuplication.Avatar)
                hc.ChangeToSkinMaterial();
        }


        if (TriggerId > 0)
            Trigger.OnFinishedFeedforward(TriggerId);

        HandInteractionManager.OnFinishFeedforward(hc, duplicateHands);
    }

}
