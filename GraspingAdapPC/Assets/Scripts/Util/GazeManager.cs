using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeManager: MonoBehaviour
{
    public Camera CenterCamera;
    public float distance;
    RaycastHit _hit;
    public GameObject GazeTarget;

    void Start()
    {
        _hit = new RaycastHit();
    }

    // Update is called once per frame
    void Update()
    {

        if (Physics.Raycast(CenterCamera.transform.position, CenterCamera.transform.forward, out _hit, distance))
        {
            Debug.DrawRay(CenterCamera.transform.position, CenterCamera.transform.forward * _hit.distance, Color.yellow);
            if(_hit.transform.tag.Equals("GazeTarget")){
                GazeTarget = _hit.transform.gameObject;
                //Debug.Log(_hit.transform.name + " at " + _hit.distance);
            } else {
                GazeTarget = null;
            }
        }
    }

}
