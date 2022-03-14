using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoorController : MonoBehaviour
{
    public Transform Door, Knob, Frame;
    private HingeJoint knobJoint;

    public float speedAdjust=0.0001f, speed;

    public Vector3 CurrentTranslationValue;

    public Vector3 CurrentLocalPosDoor;

    Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        knobJoint = Knob.gameObject.GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentLocalPosDoor = Door.localPosition;
        //Debug.Log("force " + knobJoint.currentForce);
        //Debug.Log("torque " + knobJoint.currentTorque);
        //Debug.Log("angle " + knobJoint.angle);
        //Debug.Log("velocity " + knobJoint.velocity);

//        Debug.Log("magnitude "+knobJoint.currentForce.magnitude );
        //speed = knobJoint.currentForce.magnitude * speedAdjust;

        
        speed = speedAdjust;

        if (knobJoint.velocity  < 0)
        {
            //dir = Door.forward;
            dir = -Vector3.forward;
        }
        else
        {
           // dir = -Door.forward;
            dir = Vector3.forward;
        }

       CurrentTranslationValue = dir * speed * Time.deltaTime;

        //move left
        if(  ( knobJoint.angle <= -75 && knobJoint.angle >= -90) )
            Door.Translate(CurrentTranslationValue);
        
        //move right
        if(  ( knobJoint.angle <= 90 && knobJoint.angle >= 75))
            Door.Translate(CurrentTranslationValue);
    


    }

    private void FixedUpdate() {
 
    }
}
