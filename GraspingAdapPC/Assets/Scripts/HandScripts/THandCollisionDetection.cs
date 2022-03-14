using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class THandCollisionDetection : MonoBehaviour
{

    public List<InteractionId> Ids;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddId(InteractionId id){

        if(Ids==null){
            Ids = new List<InteractionId>();
        }

        if(!Ids.Contains(id)) //keep unique
            Ids.Add(id);
    }

    private void OnCollisionEnter(Collision other) {
       //Debug.Log(transform.name +" collided with "+other.transform.name);
      //  if(other.transform.gameObject.layer == LayerMask.NameToLayer("TrackedHands")){
       //    foreach(InteractionId id in Ids)
      //      Interaction.OnInterrupted(id);
      //  }
    }
}
