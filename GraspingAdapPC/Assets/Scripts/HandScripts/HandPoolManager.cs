using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPoolManager : MonoBehaviour
{

    public List<HandContainerFF> HandContainers;
    public Material Ghost;
    public Material GhostBase;

    public Material GhostsOpaque, Skin;
    public static Material GhostsOpaqueMaterial, GhostsBaseMaterial, GhostsMaterial, SkinMaterial;
    // Start is called before the first frame update
    void Start()
    {
        HandContainers = new List<HandContainerFF>();
        FetchHandContainers();
        GhostsOpaqueMaterial = GhostsOpaque;
        GhostsMaterial = Ghost;
        GhostsBaseMaterial = GhostBase;
        SkinMaterial= Skin;
    }

    void FetchHandContainers(){
        foreach(Transform hands in transform){
            HandContainerFF hc = hands.gameObject.GetComponent<HandContainerFF>();
            hc.Status  = HandStatus.TrackedInvisible;
            hc.SetMeshVisibility(false);
            HandContainers.Add(hc);
        }
    }

    public HandContainerFF GetAvailableHand(){
        foreach(HandContainerFF hand in HandContainers){
            if(hand.Status == HandStatus.TrackedInvisible)
                return hand;
        }
        return null;

    }   
    // Update is called once per frame
    void Update()
    {
        
    }
}
