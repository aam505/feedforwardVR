using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class SphereReveal : MonoBehaviour
{

    private void Start()
    {

    }

    void Update()
    {

        Shader.SetGlobalVector("_PlayerPos", transform.position); //"transform" is the transform of the Player


    }
}