using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public enum GestureId
{
    New,
    OK,
    Tumb,
    Cube,
    CapsuleSmall,
    CapsuleBig,
    SphereSmall,
    SphereBig
}

//Add here all gestures in an enum
[System.Serializable]
public class Gesture
{
    //public GestureId Id
    [SerializeField]
    private List<BoneTransform> FingerData;

    public void setFingerData(List<BoneTransform> data)
    {
        FingerData = data;
    }
    public List<BoneTransform> getFingerData()
    {
        return FingerData;
    }

    public Gesture ReverseGesture()
    {
        List <BoneTransform> reversedFingerData = new List<BoneTransform>();
        foreach (var bone in FingerData)
        {
            Vector3 inversedPos = -1 * bone.bonePosition;
            Quaternion inverseRot = bone.boneRotation;
            BoneTransform reversedBone = new BoneTransform(bone.boneId, inversedPos, inverseRot);
            reversedFingerData.Add(reversedBone);
        }
        Gesture reversed = new Gesture();
        reversed.setFingerData(reversedFingerData);
        return reversed;
    }

    //public UnityEvent onRecognized;
   
}
