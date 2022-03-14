using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransforms : MonoBehaviour
{
    public GameObject relativeTo;
    public GameObject target;

    public bool Load = false;
    public bool Record = false;
    public bool LockPosition = false;




    public Vector3 position;
    public Quaternion rotation;

    public Vector3 recordedPosition;
    public Quaternion recordedRotation;

    public Vector3 loadedPosition;
    public Quaternion loadedRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    static Quaternion GeGlobalRotation(Quaternion relativeRotation, GameObject o)
    {

        Quaternion rotation = Quaternion.Inverse(o.transform.rotation) * relativeRotation ;
        return rotation;
    }
    static Quaternion GetRelativeRotation(Quaternion globalRotation, GameObject o)
    {
        Quaternion rotation = Quaternion.Inverse(o.transform.rotation) * globalRotation;
        return rotation;
    }

    static Vector3 GetRelativePosition(Vector3 pos, GameObject o)
    {

        Vector3 position = o.transform.InverseTransformPoint(pos);
        return position;
    }

    static Vector3 GetGlobalPosition(Vector3 relativePos, GameObject o)
    {

        Vector3 position = o.transform.TransformPoint(relativePos);
        return position;
    }


    // Update is called once per frame
    void Update()
    {
        position = target.transform.position;
        rotation = target.transform.rotation;
        if (Record)
        {
            recordedPosition = GetRelativePosition(target.transform.position, relativeTo);
            recordedRotation = GetRelativeRotation(target.transform.rotation, relativeTo);
            Record = false;
        }
        if (Load)
        {
            loadedPosition = GetGlobalPosition(recordedPosition, relativeTo);
            target.transform.position = loadedPosition;
            loadedRotation = GeGlobalRotation(recordedRotation, relativeTo);
            target.transform.rotation = loadedRotation;
            Load = false;
        }

        if (LockPosition)
        {
            loadedPosition = GetGlobalPosition(target.transform.position, relativeTo);
            target.transform.position = loadedPosition;
        }
    }

}