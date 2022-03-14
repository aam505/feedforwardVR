using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistancePerspectiveTrigger : Trigger
{
    public float Distance;
    public float TriggerAt;
    public bool DistanceTriggerFlag = false;
    public bool SwapDistanceFlag = false;

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
