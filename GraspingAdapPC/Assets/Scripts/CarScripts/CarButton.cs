using UnityEngine;
using UnityEngine.Events;

public class CarButton : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent { }
    public bool isPressed=false;
    public ButtonEvent downEvent;
  
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float deadzone = 0.025f;
    public Vector3 startPos = new Vector3();
    ConfigurableJoint joint;

    [SerializeField] private bool CarStarted=false;
    Rigidbody rb;
    [SerializeField] float value;
    void Start()
    {
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
        rb = GetComponent<Rigidbody>();
    }

    private float GetValue()
    {
        value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;
        if (Mathf.Abs(value) < deadzone)
            value = 0;


        return Mathf.Clamp(value, -1, 1);
    }

    void Press()
    {
        isPressed = true;

        if (!CarStarted)
        {
            CarStarted = true;
           CarManager.OnCarStarted();
        } else {
            CarStarted = false;
            CarManager.OnCarStopped();
            
        }
        Debug.Log("press");
    }

    void Release()
    {
        isPressed = false;
        Debug.Log("release");
    }

    public float PresValue, ReleaseValue;
    void Update()
    {
        PresValue = GetValue() + threshold;
        ReleaseValue = GetValue() - threshold;
        if (!isPressed && (GetValue() + threshold >= 1))
            Press();
        if (isPressed && (GetValue() - threshold <= 0))
            Release();
    }

}