using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    public void OnPress()
    {
        Debug.Log("SteamVR Button pressed!");
    }

    public void OnCustomButtonPress()
    {
        Debug.Log("We pushed our custom button!");
    }
}