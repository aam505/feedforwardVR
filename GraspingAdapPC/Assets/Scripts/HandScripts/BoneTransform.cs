using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRSkeleton;


[System.Serializable]
public class BoneTransform
{

    public BoneId boneId;
    public Vector3 bonePosition;
    public Quaternion boneRotation;

    public BoneTransform(BoneId id, Vector3 bpos, Quaternion brot)
    {
        boneId = id;
        bonePosition = bpos;
        boneRotation = brot;
    }
    public BoneTransform(int idx, Vector3 bpos, Quaternion brot)
    {
        boneId = GetBoneIdFromIdx(idx);
        bonePosition = bpos;
        boneRotation = brot;
    }

    BoneId GetBoneIdFromIdx(int idx)
    {
        /*
        * 0 - wrist
        * 1 - forearm
        * 
        * 2 - thumb0
        * 3 - thumb1
        * 4 - thumb2
        * 5 - thumb3
        * 
        * 6 - index1
        * 7 - index2
        * 8 - index3
        * 
        * 9 - middle1
        * 10 - middle2
        * 11 - middle3
        * 
        * 12 - ring1
        * 13 - ring2
        * 14 - ring3
        * 
        * 15 - pinky0
        * 16 - pinky1
        * 17 - pinky2
        * 18 - pinky3
        * 
        * 19 - thumbTip
        * 20 - indexTip
        * 21 - middleTip
        * 22 - ringTip
        * 23 - pinkyTip
        */
        BoneId bid = BoneId.Invalid;
        switch (idx)
        {
            case 0:
                bid = BoneId.Hand_WristRoot;
                break;
            case 1:
                bid = BoneId.Hand_ForearmStub;
                break;
            case 2:
                bid = BoneId.Hand_Thumb0;
                break;
            case 3:
                bid = BoneId.Hand_Thumb1;
                break;
            case 4:
                bid = BoneId.Hand_Thumb2;
                break;
            case 5:
                bid = BoneId.Hand_Thumb3;
                break;
            case 6:
                bid = BoneId.Hand_Index1;
                break;
            case 7:
                bid = BoneId.Hand_Index2;
                break;
            case 8:
                bid = BoneId.Hand_Index3;
                break;
            case 9:
                bid = BoneId.Hand_Middle1;
                break;
            case 10:
                bid = BoneId.Hand_Middle2;
                break;
            case 12:
                bid = BoneId.Hand_Ring1;
                break;
            case 13:
                bid = BoneId.Hand_Ring2;
                break;
            case 14:
                bid = BoneId.Hand_Ring3;
                break;
            case 15:
                bid = BoneId.Hand_Pinky0;
                break;
            case 16:
                bid = BoneId.Hand_Pinky1;
                break;
            case 17:
                bid = BoneId.Hand_Pinky2;
                break;
            case 18:
                bid = BoneId.Hand_Pinky3;
                break;
            case 19:
                bid = BoneId.Hand_ThumbTip;
                break;
            case 20:
                bid = BoneId.Hand_IndexTip;
                break;
            case 21:
                bid = BoneId.Hand_MiddleTip;
                break;
            case 22:
                bid = BoneId.Hand_RingTip;
                break;
            case 23:
                bid = BoneId.Hand_PinkyTip;
                break;
        }
        return bid;
    }
    public void SetPositionRotation(Vector3 pos, Quaternion r)
    {
        bonePosition = pos;
        boneRotation = r;
    }

    public void SetPosition(Vector3 pos)
    {
        bonePosition = pos;
    }
    public void SetRotation(Quaternion r)
    {
        boneRotation = r;
    }


    //transform the position from world space to local space of object b. Uset it to set the hand relative to object b.
    public static BoneTransform GetRelativeCoords(BoneTransform globalTransforms, GameObject o)
    {

        Vector3 position = GetRelativePosition(globalTransforms.bonePosition, o);

        Quaternion rotation = GetRelativeRotation(globalTransforms.boneRotation, o);

        return new BoneTransform(globalTransforms.boneId, position, rotation);

    }

    public static BoneTransform GetRelativeCoords(OVRBone globalBone, GameObject o)
    {

        Vector3 position = GetRelativePosition(globalBone.Transform.position, o);
        Quaternion rotation = GetRelativeRotation(globalBone.Transform.rotation, o);
        return new BoneTransform(globalBone.Id, position, rotation);

    }

    //get the world space coordinates of an object from its coordinates in the local space of object o.
    public static BoneTransform GetGlobalCoords(BoneTransform relativeBoneTransform, GameObject o)
    {
        Vector3 position = GetGlobalPosition(relativeBoneTransform.bonePosition, o);
        Quaternion rotation = GetGlobalRotation(relativeBoneTransform.boneRotation, o);
        return new BoneTransform(relativeBoneTransform.boneId, position, rotation);
    }


    public static Quaternion GetGlobalRotation(Quaternion relativeRotation, GameObject o)
    {
        Quaternion rotation = o.transform.rotation * relativeRotation;
        //Quaternion rotation = Quaternion.Inverse(relativeRotation ) * o.transform.rotation;
        return rotation;
    }

    public static Quaternion GetRelativeRotation(Quaternion globalRotation, GameObject o)
    {
        Quaternion rotation = Quaternion.Inverse(o.transform.rotation) * globalRotation;
        return rotation;
    }

    public static Vector3 GetRelativePosition(Vector3 pos, GameObject o)
    {

        Vector3 position = o.transform.InverseTransformPoint(pos);
        return position;
    }

    public static Vector3 GetGlobalPosition(Vector3 relativePos, GameObject o)
    {

        Vector3 position = o.transform.TransformPoint(relativePos);
        return position;
    }

}