using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsOnScript : MonoBehaviour
{
    // Start is called before the first frame update

    public bool LightOnStatus = false;
    public List<Light> Lights;
    void Start()
    {
        LightsOff();
    }

    // Update is called once per frame
    void Update()
    {
        if (LightOnStatus && transform.localEulerAngles.x < 300)
        {
            LightsOff();
        }

        if (!LightOnStatus && transform.localEulerAngles.x > 330)
        {
            LightsOn();
        }
    }

    void LightsOn()
    {
        LightOnStatus = true;
        foreach (Light l in Lights)
        {
            l.enabled = true;
        }
    }

    void LightsOff()
    {
        LightOnStatus = false;
        foreach (Light l in Lights)
        {
            l.enabled = false;
        }
    }
}
