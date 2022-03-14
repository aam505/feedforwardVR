using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FeedforwardObjectState 
{
    [SerializeField]
    public List<ObjectTransform> States;
    // Start is called before the first frame update

    public FeedforwardObjectState(){
        States  = new List<ObjectTransform>();
    }

    public FeedforwardObjectState(List<ObjectTransform> states){
        States = states;
    }
    
}
