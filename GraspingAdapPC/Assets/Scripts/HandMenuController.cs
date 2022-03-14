using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HandMenuController : MonoBehaviour
{
    public static Action OnSelectedButton;
    public List<HandButtonScript> Buttons;

    // Start is called before the first frame update
    void Start()
    {
        Buttons = new List<HandButtonScript>(gameObject.GetComponentsInChildren<HandButtonScript>());
     
    }

    private void OnEnable() {
        OnSelectedButton += TempDisableSelection;
    }
    private void OnDisable() {
           OnSelectedButton -= TempDisableSelection;
    }

    void TempDisableSelection(){
        foreach (HandButtonScript s in Buttons)
        {
            s.StartSelectionCooldown();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
