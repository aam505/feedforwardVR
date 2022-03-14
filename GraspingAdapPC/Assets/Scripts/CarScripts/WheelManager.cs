using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    Rigidbody rigidbody;
    // Start is called before the first frame update
  void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
       rigidbody.centerOfMass = Vector3.zero;
        rigidbody.inertiaTensorRotation = new Quaternion(0, 0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
