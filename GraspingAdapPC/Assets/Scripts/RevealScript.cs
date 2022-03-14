using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RevealScript : MonoBehaviour
{
    // Start is called before the first frame update

        [SerializeField] Material RevealMat;

     void Update()
    {
        RevealMat.SetVector("_Position",transform.position);
        //Shader.SetGlobalVector("_Position", transform.position);
    }
}
