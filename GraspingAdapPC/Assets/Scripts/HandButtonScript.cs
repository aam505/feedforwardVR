using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandButtonScript : MonoBehaviour
{
    public int Condition;
    private bool selected=false;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        Condition=  int.Parse(gameObject.name[gameObject.name.Length-1].ToString());
        button = GetComponent<Button>();

        StartSelectionCooldown(6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        if(!selected && other.gameObject.name.Contains("index3")){
            selected = true; 
            HandMenuController.OnSelectedButton();
            Debug.Log("pressed "+Condition);
            //Trigger.OnMenuItemPressed(Condition);
        }
    }

    public void StartSelectionCooldown(int s = 3){
         StartCoroutine(SelectionCooldown(s));
    }
    IEnumerator SelectionCooldown(int s){
        selected = true;
        yield return new WaitForSeconds(s);
        selected = false;
    }
}
