 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceTrigger : Trigger
{

    [Tooltip("Current distance from the trigger object.")]
    public float Distance;
    [Tooltip("Distance at which the previews will be triggered: if distance < triggerAt")]
    public float TriggerAt;

    [Tooltip("Flag, true if triggered preview")]
    public bool DistanceTriggerFlag = false;

    protected override void Start()
    {
        base.Start();
    }


    protected override void Update()
    {
        if (MainHandContainer.RegisteredHands())
        {
            CheckDistance();

            if (Distance < TriggerAt)
            {
                DistanceTriggerFlag = true;
            }
            if (DistanceTriggerFlag)
            {
                
                StartFeedforward();
                DistanceTriggerFlag = false;
            }
        }


    }

    void CheckDistance()
    {

        if (base.interaction.Handedness == OVRHand.Hand.HandLeft)
        {
            Distance = Vector3.Distance(MainHandContainer.OVRHandLeft.transform.position, TriggerObject.transform.position);
        }
        else
        {
            Distance = Vector3.Distance(MainHandContainer.OVRHandRight.transform.position, TriggerObject.transform.position);
        }


    }
    public override void StartFeedforward()
    {
        // check if conditions is achieved
        base.StartFeedforward();

    }
}
