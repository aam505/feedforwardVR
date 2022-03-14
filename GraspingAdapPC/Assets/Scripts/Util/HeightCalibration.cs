using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightCalibration : MonoBehaviour
{
    [Tooltip("Object containing the room which is the experiment setup")]
    public float posX, posY,posZ;
    public GameObject Setting;
    static GameObject headGameObject;
    OVRCameraRig cameraRig;
    bool FirstTracked=true, FirstAnchors=true;
    private void Awake()
    {
        headGameObject = GameObject.Find("CenterEyeAnchor");
        cameraRig = headGameObject.transform.parent.parent.GetComponent<OVRCameraRig>();
        //Setting = GameObject.Find("Room");
    }

    private void OnEnable()
    {
        cameraRig.UpdatedAnchors += OnUpdatedAnchors;
        OVRManager.TrackingAcquired += OnTrackingAcquired;
    }

    private void Update()
    {
        //Debug.Log(HeadToString());

        //uncomment to check
        //if(!initPos.Equals(Vector3.zero))
        //    Setting.transform.position = initPos - new Vector3(posX, posY, posZ);
    }

    static string HeadToString()
    {
        return "P(" + headGameObject.transform.position.x + ", " + headGameObject.transform.position.y + ", " + headGameObject.transform.position.z
            + "), R(" + headGameObject.transform.eulerAngles.x + ", " + headGameObject.transform.eulerAngles.y + ", " + headGameObject.transform.eulerAngles.z + ")";
    }

   private Vector3 initPos=Vector3.zero;
   public IEnumerator PositionSetting()
    {
        //yield return new WaitForSeconds(1f);
        if (!headGameObject.transform.position.Equals(Vector3.zero))
        {
            Debug.Log("Positioning at nonzero " + " " +HeadToString()) ;
            GameObject camera = GameObject.Find("CenterEyeAnchor");

            Setting.transform.rotation = new Quaternion(0.0f, camera.transform.rotation.y, 0.0f, camera.transform.rotation.w);
            Setting.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y,camera.transform.position.z);
           //Setting.transform.rotation = new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, camera.transform.rotation.w);
            initPos = Setting.transform.position;
            Setting.transform.position = initPos - new Vector3(posX, posY, posZ);            
            Setting.transform.position += camera.transform.forward  * 2.5f;



            //Quaternion rotation = Quaternion.Inverse(Setting.transform.rotation) * camera.transform.rotation;
            //Vector3 position = camera.transform.InverseTransformPoint(pos);

            //Setting.transform.position = camera.transform.position + camera.transform.forward * 2.5f;
            //initPos = Setting.transform.position;
            //Setting.transform.position = initPos - new Vector3(posX, posY, posZ);
            //Setting.transform.position -= new Vector3(posX, posY, posZ); //move to 1.2 in apk
            
        }

        yield return null;
    }

    private void OnTrackingAcquired()
    {
        
        StartCoroutine(PositionSetting());
        OVRManager.TrackingAcquired -= OnTrackingAcquired;
    }

    //todo leave this before deployment
    private void OnUpdatedAnchors(OVRCameraRig obj)
    {
       
        StartCoroutine(PositionSetting());
        cameraRig.UpdatedAnchors -= OnUpdatedAnchors;

    }
}
