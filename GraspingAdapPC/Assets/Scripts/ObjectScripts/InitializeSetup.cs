using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeSetup : MonoBehaviour
{


    public GameObject TableWithObjects;
    public GameObject TrackedHead;
    public Vector3 distance;
    public bool InitializeTableTrigger = false;
    public bool InitializedTable = false;
    // Start is called before the first frame update
    void Start()
    {
        TableWithObjects.SetActive(false);
    }


    IEnumerator InitializeTable()
    {
        InitializedTable = true;
        yield return new WaitForSeconds(2);
        TableWithObjects.transform.position = TrackedHead.transform.position+ distance;
        TableWithObjects.SetActive(true);
       
    }
    // Update is called once per frame
    void Update()
    {

        if (!InitializedTable && InitializeTableTrigger)
        {
            StartCoroutine(InitializeTable());
        }
        if (!InitializeTableTrigger && GetComponent<HandContainer>().LeftOVRHand.GetComponent<OVRHand>().IsTracked)
        {
            
            InitializeTableTrigger = true;


        }
    }
}
