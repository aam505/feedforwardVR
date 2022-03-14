using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSaceConstraints : MonoBehaviour
{
    // Start is called before the first frame update
    new Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
      void Update() 
    {
        //Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        //localVelocity.x = 0;
        //localVelocity.y = 0;

        //rigidbody.velocity = transform.TransformDirection(localVelocity); 
        transform.localPosition= new Vector3(0,0,transform.localPosition.z);
    }
}
