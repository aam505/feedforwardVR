using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeTrigger: Trigger
{
     GazeManager GazeDetector;
 
    protected override void Start() {
        base.Start();
        GazeDetector = GameObject.Find("Controller").GetComponent<GazeManager>();
    }
    // Update is called once per frame
    protected override void  Update()
    {
        base.Update();
        if(GazeDetector.GazeTarget== gameObject){
            base.StartFeedforward();
        }
    
    }

}
