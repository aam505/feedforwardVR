using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public TMPro.TextMeshProUGUI Instructions;
    // Start is called before the first frame update
    void Start()
    {
        Instructions.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SetTextTemporaryCoroutine(float duration, string text)
    {
        Instructions.text = text;
        yield return new WaitForSeconds(duration);
        Instructions.text = "";
    }

    public void SetText(string v)
    {
        Instructions.text = v;
    }

    public void SetTextTemporary(float seconds, string text)
    {
        StartCoroutine(SetTextTemporaryCoroutine(seconds, text));
    }
     
}
