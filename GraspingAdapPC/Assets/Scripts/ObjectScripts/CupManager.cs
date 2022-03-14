using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupManager : MonoBehaviour
{

    public ParticleSystem particleSystem;
    public int PourThreshold = 45;
    Transform origin = null;
    private bool isPouring = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float PourAngle;
    // Update is called once per frame
    void Update()
    {

        PourAngle = CalculatePourAngle();
        bool pourCheck =  PourAngle < PourThreshold;

        if(isPouring!=pourCheck){

            isPouring = pourCheck;

            if(!isPouring){
                StartPour();
            } else {
                EndPour();
            }
        }
    }

    private void StartPour(){
        particleSystem.Play();
    }

    private void EndPour(){
        particleSystem.Stop();
    }

    private float CalculatePourAngle(){
        return transform.right.x * Mathf.Rad2Deg;
    }
}
