using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static OVRSkeleton;


public class GestureManager : MonoBehaviour
{
    bool registeredHand = false;

    [SerializeField]
    public OVRHand.Hand Handedness;

    public float RecognitionThreshold = 0.1f;
    [SerializeField]
    public List<Gesture> gestures;
    private HandContainer hands;

    private OVRSkeleton skeleton;
    private OVRSkeleton skeletonTracked;
    private OVRHand hand;
    private OVRHand trackedHand;
    private Gesture previousGesture;

    private List<OVRBone> fingerBones;
    private List<OVRBone> trackedFingerBones;

    [SerializeField]
    public List<BoneTransform> current;

    public bool interruptAnimation = false;
    public bool interruptRecoveryAnimation = false;
    public bool RecognizeGestureFlag = false;
    public bool poseCorutineActive = false;
    public bool recoveryCorutineActive = false;
    // Start is called before the first frame update
    void Start()
    {
        hands = transform.gameObject.GetComponent<HandContainer>();

        hand = hands.LeftOVRHand.GetComponent<OVRHand>();
        trackedHand = hands.LeftOVRHand.GetComponent<OVRHand>();

        skeletonTracked = hands.LeftOVRHand.GetComponent<OVRSkeleton>();
        skeleton = hands.LeftOVRHand.GetComponent<OVRSkeleton>();

        previousGesture = new Gesture();
    }

    // Update is called once per frame
    void Update()
    {
        if( !registeredHand && hand.IsTracked)
        {
            //populate with all bones that can be accessed
           
            fingerBones = new List<OVRBone>(skeleton.Bones);
            trackedFingerBones = new List<OVRBone>(skeletonTracked.Bones);

            current = new List<BoneTransform>();
            for (int i = 0; i < fingerBones.Count; i++)
            {
                var bone = fingerBones[i];
                current.Add(new BoneTransform(bone.Id,bone.Transform.position, bone.Transform.rotation));
            }

            registeredHand = true;
        }

        if (registeredHand)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Save();
                Debug.Log("Saved gesture....");
            }
        }
    }


    private void oldRecognizer()
    {
        if (registeredHand)
        {

            Gesture currentGesture = Recognize();
            bool recognized = !currentGesture.Equals(new Gesture()); //check is different from empty gesture

            if (recognized && !currentGesture.Equals(previousGesture))
            {
                previousGesture = currentGesture;
               // Debug.Log("NEW GESTURE " + currentGesture.Id);
               // currentGesture.onRecognized.Invoke();
            }


            //check if the current gesture wwas not recognized so we trigger an event only when a new gesture is recognized
        }
    }

    public bool grabbed = false;
    public void RecognizeGesture(GestureId target)
    {

        if (registeredHand)
        {
            
            //Gesture currentGesture = Recognize();
            //Debug.Log("NEW GESTURE " + currentGesture.Id);
            //if (!grabbed && currentGesture.Id ==target)
            //{
                
            //    Debug.Log("----------------------appropriate gesture triggered " + currentGesture.Id);
            //    Debug.Log("grabbing now");
            //    //currentGesture.onRecognized.Invoke();
            //    hands.LeftTrackedHandPrefab.GetComponent<CustomHandGrabber>().StartGrab();
            //    grabbed = hands.LeftTrackedHandPrefab.GetComponent<CustomHandGrabber>().grabbing;
            //    if(grabbed) previousGesture = currentGesture;
            //}

            //if(grabbed && currentGesture.Id != target)
            //{
            //    grabbed = false;
            //    Debug.Log("ending grabbing gesture changed");
            //    hands.LeftTrackedHandPrefab.GetComponent<CustomHandGrabber>().EndGrab();
            //}


            //check if the current gesture wwas not recognized so we trigger an event only when a new gesture is recognized
        }
    }
    private void FixedUpdate()
    {
       
    }

    public void TriggerAnimation(GestureId id)
    {
        if (registeredHand)
        {
            if (recoveryCorutineActive)
                interruptRecoveryAnimation = true;
            if (!poseCorutineActive)
                StartCoroutine(InterpolateGesture(id));
        }
    }

    public void TriggerHandRecovery()
    {
        if (registeredHand)
        {
            if (poseCorutineActive)
                interruptAnimation = true;
            if (!recoveryCorutineActive)
                StartCoroutine(InterpolateTrackedHand());
        }
    }
   
    //save coordinates relative to 
    void Save()
    {
        Gesture g = new Gesture();
      
        List<BoneTransform> data = new List<BoneTransform>();
        for (int i = 0; i < fingerBones.Count; i++)
        {
            var bone = fingerBones[i];
            BoneTransform fingerTransformData = new BoneTransform(bone.Id, BoneTransform.GetRelativePosition(bone.Transform.position,skeleton.transform.gameObject),BoneTransform.GetRelativeRotation(bone.Transform.rotation,skeleton.transform.gameObject));
            data.Add(fingerTransformData);
        }
        
        g.setFingerData(data);
        gestures.Add(g);
    }

    //when updating bone poses i should deselect update bone poses
    Gesture Recognize()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);

                //Vector3 currentData = skeletonTracked.transform.InverseTransformPoint(trackedFingerBones[i].Transform.position);

                float distance = Vector3.Distance(currentData, gesture.getFingerData()[i].bonePosition);

                if (distance > RecognitionThreshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance; 
            }

            if(!isDiscarded && sumDistance< currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }
        return currentGesture;
    }


    public void AnimateGesture(Gesture target)
    {
        float speed = 1.0f;

        skeleton.updateRootPose = false; //todo somewhere turn this back on

        for (int i = 0; i < fingerBones.Count; i++)
        {
            if (fingerBones[i].Id != BoneId.Hand_Start && fingerBones[i].Id != BoneId.Hand_ForearmStub)
            {
         
                //geting the relative position to self should be local position no????
                fingerBones[i].Transform.localPosition = BoneTransform.GetRelativePosition(target.getFingerData()[i].bonePosition, fingerBones[i].Transform.gameObject);

                fingerBones[i].Transform.localRotation = BoneTransform.GetRelativeRotation(target.getFingerData()[i].boneRotation, fingerBones[i].Transform.gameObject);

                // local 
                //fingerBones[i].Transform.localPosition = boneTransform.bonePosition;
                //fingerBones[i].Transform.localRotation = boneTransform.boneRotation;

            }
        }
    }

    public IEnumerator InterpolateGesture(GestureId id)
    {
        skeleton.updateRootPose = false; //todo somewhere turn this back on.

        Debug.Log("Interpolating " + id.ToString());
        Gesture target = gestures[0];
 
        float elapsedTime = 0;
        float time = 5f;

        poseCorutineActive = true;
        
        while (elapsedTime < time)
        {
            for (int i = 0; i < fingerBones.Count; i++)
            {
                if (fingerBones[i].Id != BoneId.Hand_Start && fingerBones[i].Id != BoneId.Hand_ForearmStub)
                {

                    //BoneTransform boneTransform = target.fingerData[i].boneTransformLocal;

                    Vector3 localPosition = BoneTransform.GetRelativePosition(target.getFingerData()[i].bonePosition, fingerBones[i].Transform.gameObject);
                    Quaternion localRotation = BoneTransform.GetRelativeRotation(target.getFingerData()[i].boneRotation, fingerBones[i].Transform.gameObject);
                    // local 
                    //fingerBones[i].Transform.localPosition = boneTransform.bonePosition;
                    //fingerBones[i].Transform.localRotation = boneTransform.boneRotation;

                    fingerBones[i].Transform.localPosition = Vector3.LerpUnclamped(fingerBones[i].Transform.localPosition,localPosition, (elapsedTime / time));
                    fingerBones[i].Transform.localRotation = Quaternion.LerpUnclamped(fingerBones[i].Transform.localRotation,localRotation, (elapsedTime / time));
                   

                }
            }
            if (interruptAnimation)
            {
                Debug.Log("interrupting animation..");
               
                break;
                
            }
            elapsedTime += Time.deltaTime;
            //yield return new WaitForEndOfFrame(); 
            yield return new WaitForFixedUpdate(); 
            //yield return null; 
        }
        poseCorutineActive = false;

        if (interruptAnimation)
        {
            interruptAnimation = false;

        }
        else
        {
            TriggerHandRecovery();
        }

        //skeleton.updateRootPose = true;
    }

    public IEnumerator InterpolateTrackedHand()
    {
        skeleton.updateRootPose = false; //todo somewhere turn this back on.

        Debug.Log("Interpolating to tracked hand ");

        float elapsedTime = 0;
        float time = 1f;

        recoveryCorutineActive = true;
        while (elapsedTime < time)
        {
            for (int i = 0; i < fingerBones.Count; i++)
            {
                if (fingerBones[i].Id != BoneId.Hand_Start && fingerBones[i].Id != BoneId.Hand_ForearmStub)
                {
                    fingerBones[i].Transform.localPosition = Vector3.LerpUnclamped(fingerBones[i].Transform.localPosition, trackedFingerBones[i].Transform.localPosition, (elapsedTime / time));
                    fingerBones[i].Transform.localRotation = Quaternion.LerpUnclamped(fingerBones[i].Transform.localRotation, trackedFingerBones[i].Transform.localRotation, (elapsedTime / time));
                }
            }
            if (interruptRecoveryAnimation)
            {
                Debug.Log("interrupting recovery..");
            
                break;

            }
            elapsedTime += Time.deltaTime;
            //yield return new WaitForEndOfFrame(); 
            yield return new WaitForFixedUpdate();
            //yield return null; 
        }
        recoveryCorutineActive = false;

        if (interruptRecoveryAnimation)
        {
            //interrupted only to display pose animation
            interruptRecoveryAnimation = false;

        }
        else
        {
            skeleton.updateRootPose = true;
        }
       
       
    }

    //public void AnimateGesture(GestureId id)
    //{
    //    Gesture target = gestures[0];
    //    foreach (var gesture in gestures)
    //    {
    //        if (gesture.Id == id)
    //            target = gesture;
    //    }


    //    skeleton.updateRootPose = false; //todo somewhere turn this back on

    //    for (int i = 0; i < fingerBones.Count; i++)
    //    {
    //        if (fingerBones[i].Id != BoneId.Hand_Start && fingerBones[i].Id != BoneId.Hand_ForearmStub)
    //        {
    //            geting the relative position to self should be local position no ????
    //            fingerBones[i].Transform.localPosition = BoneTransform.GetRelativePosition(target.FingerData[i].bonePosition, fingerBones[i].Transform.gameObject);

    //            fingerBones[i].Transform.localRotation = BoneTransform.GetRelativeRotation(target.FingerData[i].boneRotation, fingerBones[i].Transform.gameObject);

    //            local
    //           fingerBones[i].Transform.localPosition = boneTransform.bonePosition;
    //            fingerBones[i].Transform.localRotation = boneTransform.boneRotation;

    //        }
    //    }
    //}
}
