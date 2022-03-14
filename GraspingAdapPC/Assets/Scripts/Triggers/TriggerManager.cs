using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TriggerManager : Singleton<TriggerManager>
{
    // Start is called before the first frame update
    public  int Value = 1;
    public bool StartAllTriggers=false;
    public List<Trigger> Triggers;
    static Object ReadLock = new Object();
    static Object AddLock = new Object();
    public int GetNewValue()
    {
        Interlocked.Increment(ref Value);
        lock (ReadLock)
        {
            return Value;
        }
    }
    public void Add(Trigger t){
        lock(AddLock){
            Triggers.Add(t);
        }
    }
    void Start()
    {

    }

    void StartAll(){
        foreach(Trigger t in Triggers)
            t.StartFeedforward();
    }
    // Update is called once per frame
    void Update()
    {
        if(StartAllTriggers==true){
            StartAll();
            StartAllTriggers=false;
        }
    }
}
