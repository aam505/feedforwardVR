using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarManager : MonoBehaviour
{


    public static Action OnCarStarted, OnCarStopped;
    public AudioSource carStartAudio;
    public AudioSource carOnAudio;
    public AudioSource carStopAudio;
    // Start is called before the first frame update

    private void Awake()
    {

    }

    private void OnEnable()
    {
        OnCarStarted += HandleCarStart;
        OnCarStopped += HandleCarStopped;
    }

    private void OnDisable()
    {
        OnCarStarted -= HandleCarStart;
        OnCarStopped -= HandleCarStopped;
    }

    void HandleCarStart()
    {
        Debug.Log("Starting car..");
        carStartAudio.Play();
        carOnAudio.PlayScheduled(0.2f);
    }
    void HandleCarStopped()
    {
        Debug.Log("Stopping car..");
        if (carOnAudio.isPlaying)
        {
            carOnAudio.Pause();
        }

        carStopAudio.Play();

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
