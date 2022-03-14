using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectTransform 
{
    [SerializeField]
    public Vector3 ObjectPosition;
    [SerializeField]
    public Quaternion ObjectRotation;

    public ObjectTransform(Vector3 bpos, Quaternion brot)
    {
        ObjectPosition = bpos;
        ObjectRotation = brot;
    }

}
