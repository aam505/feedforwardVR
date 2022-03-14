using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HPTK.Models.Avatar;
using HPTK.Controllers.Avatar;
using HPTK.Controllers.Input;

public class HandContainer : MonoBehaviour
{

    public int ParticipantId;
    public GameObject LeftParent, RightParent;

    public OVRHand LeftOVRHand, RightOVRHand;

    public  HandModel MasterHandModelLeft, PhysicsHandModelLeft;
    public  HandModel MasterHandModelRight, PhysicsHandModelRight;

    //The physics hand - slave. The slave follows the master i.e gets coordinates from it which in turn takes it from ovr hand
    public HandPhysicsModel PhysicsModelLeft,PhysicsModelRight;

    //The master hand
    public InputModel InputModelLeft, InputModelRight;
    
    public InputController InputControllerLeft,InputControllerRight;
    public HandPhysicsController PhysicsControllerLeft,PhysicsControllerRight;

    public ProxyHandModel ProxyHandModelLeft, ProxyHandModelRight;  
}
