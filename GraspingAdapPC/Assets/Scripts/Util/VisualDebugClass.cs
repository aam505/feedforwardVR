using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualDebugClass : MonoBehaviour
{
    public TextMeshPro TextContainer;

    static string Log;
    static float duration;
    float timer = 0;

    public static void AddMessage(string s)
    {
        Log += s + '\n';
    }

    public static void SetMessage(string s)
    {
        Log = s + '\n';
    }

    public static void ResetMessage(string s)
    {
        Log = "";
    }

    public static void SetMessageTemporary(float seconds, string text)
    {
        duration = seconds;
        Log = text;
    }

    void SetRectTransform()
    {
        RectTransform rect = TextContainer.gameObject.GetComponent<RectTransform>();
        //rect.pivot = new Vector2(0.5f, 0.92f);
        //rect.position = new Vector3(rect.position.x, rect.position.y, 0.55f);
        //rect.sizeDelta = new Vector2(20, 5);

        TextContainer.alignment = TextAlignmentOptions.Center;
        TextContainer.fontSize = 0.3f;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetRectTransform();
        Log = "INIT";
    }

    // Update is called once per frame
    void Update()
    {
        TextContainer.text = Log;
        if (duration > 0)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                Log="";
                duration = 0;
            }

        }
    }
}
