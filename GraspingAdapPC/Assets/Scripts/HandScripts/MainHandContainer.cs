using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHandContainer : MonoBehaviour
{

    public static OVRHand OVRHandLeft, OVRHandRight;
    private static GameObject OVRHandObjectLeft, OVRHandObjectRight;

    public static bool RegisteredHandLeft=false;
    public static bool RegisteredHandRight=false;

    public void PrintMsg(){
        Debug.Log("asdas");
    }
    // Start is called before the first frame update
    void Start()
    {
        OVRHandObjectLeft = GameObject.Find("LeftHandAnchor").transform.GetChild(0).gameObject;
        OVRHandObjectRight = GameObject.Find("RightHandAnchor").transform.GetChild(0).gameObject;
    }

    public static bool RegisteredHands(){
        return RegisteredHandLeft && RegisteredHandRight;
    }
     public void InitializeHandLeft()
    {
        OVRHandLeft = OVRHandObjectLeft.GetComponent<OVRHand>();
        RegisteredHandLeft = true;
    }
    public void InitializeHandRight()
    {
        OVRHandRight = OVRHandObjectRight.GetComponent<OVRHand>();
        RegisteredHandRight=true;
    }
    private void Update() {

        if (!RegisteredHandLeft && OVRManager.isHmdPresent)
        {
            InitializeHandLeft();
        }

        if (!RegisteredHandRight && OVRManager.isHmdPresent)
        {
            InitializeHandRight();
        }
    }
}
