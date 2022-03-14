using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TriggerType
{
    ObjectCollision,
    Distance,
    Gaze
}

public enum Ghost
{
    None,
    Avatar,
    Object,
    All
}

public enum TargetDuplication
{
    All,
    None,
    Avatar,
    Objects
}

public enum RewindStrategy
{
    On,
    None
}
public enum RepeatStrategy
{
    Continuous,
    OnTrigger
}

public enum TriggerStatus
{
    Active,
    Deactivated,
    Feedforward,
    Destroy
}

public enum Target
{
    All,
    Avatar,
    Object
}

public enum Visualization
{
    None,
    Lens
}
//triggers need to be uniquely identified to deactivated or changed the status
//cannot uniquely identify trigger by object id as there can be multiple triggers on the same object
//cannot identify trigger by type because it represents a group
//cannot identify trigger by interaction because an interaction could have multiple types of triggers
public class Trigger : MonoBehaviour
{
    [Tooltip("Unique identifier for trigger, set by system.")]
    [ReadOnly]
    public int Id; //Used to check trigger status

    [Tooltip("Object that triggers the feedforward. This object is checked for distance, gaze target, etc.")]
    public GameObject TriggerObject = null;
    [Tooltip("Id of the interaction previwed by the trigger..")]
    public InteractionId InteractionID;
    
    public static Action<int> OnFinishedFeedforward, OnFinishedRepetition;
    public static Action<int> OnActivateTrigger;
    public static Action<int> OnDeactivateTrigger;

    //FF related variables
    [Tooltip(" How the previews repeat: Continuous - continually repeat the same preview a set number of times or until interrupt, OnTrigger - this preview is triggered a set number of times. Recommended for repeating once. }")]
    public RepeatStrategy repeat;
    [Tooltip("How many repeats left in the feedforward, wheter continuous or on trigger.")]
    public int RemainingRepetitions;
    [Tooltip("Total number of repetitions, before starting previewing.")]
    public int TotalRepetitions;
    [Tooltip("On - Select whether the preview rewinds at the end, bringing all objects to their initial state by playing back the interactions, or None - the preview simply ends and does not rewind objects, simply snaps them at the initial positions. ")]
    public RewindStrategy RewindType;
    [Tooltip("This option selects which feedforward targets move during previewing: All - both avatar and objects move, Avatar - only the avatar previews the interaction, Objects- only objects move and preview the interactions.")]
    public Target FeedforwardTarget;
    
    [Tooltip("Select here which targets are duplicated. The preview will show on the duplicated targets, if any duplication is selected. All - duplicate avatar and objects, None - use the original objects and avatar, Avatar - duplicate the avatar, Objects - duplicate the objects")]
    public TargetDuplication DuplicationType;

    [Tooltip("Select which targets have a ghosted appearance: None - all objects and avatar are original i.e. embodied feedforward, Avatar - the avatar is ghosted, Object - the objects are ghosted, All - all targets are ghosted. Note: duplicates are always ghosted. This option is useful when there are no duplciates to ghost the original targets.")]
    public Ghost GhostType;

    [Tooltip("Visualization disabled for current implementation.")]
    [ReadOnly]
    public Visualization VisualizationType;
    protected Interaction interaction;

    [Tooltip(" Status of the trigger: Active - awaiting trigger, Deactivated - cannot be triggered anymore, Feedforward - trigger actively prevewing, Destroy - trigger has been destroyed.")]
    public TriggerStatus Status;

    //todo generate trigger id automatically in a global thread safe threadid/id generator

    private void OnEnable()
    {
        OnFinishedFeedforward += HandleFinishedFeedforward;
        OnFinishedRepetition += HandleFinishedRepetition;
    }

    private void OnDisable()
    {
        OnFinishedFeedforward -= HandleFinishedFeedforward;
        OnFinishedRepetition -= HandleFinishedRepetition;
    }
    private void Awake()
    {
        RemainingRepetitions = TotalRepetitions;
        if (TriggerObject == null)
            TriggerObject = gameObject;

        //ActivateTrigger();

    }

    void HandleFinishedFeedforward(int TriggerId)
    {
        if (this.Id == TriggerId)
        {
            Debug.Log("T" + TriggerId + ": finished ff");

            if (RemainingRepetitions == 0)
                DeactivateTrigger();
            else
                ActivateTrigger();
        }
    }

    void HandleFinishedRepetition(int TriggerId)
    {
        Debug.Log("In trigger setting reps");
        if (this.Id == TriggerId)
        {
            RemainingRepetitions--;
        }
    }
    protected virtual void Start()
    {
        TriggerManager Manager = GameObject.Find("Controller").GetComponent<TriggerManager>();

        interaction = GameObject.Find("Controller").GetComponent<HandInteractionManager>().GetInteraction(InteractionID);

        Id = Manager.gameObject.GetComponent<TriggerManager>().GetNewValue();

        Manager.GetComponent<TriggerManager>().Add(this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public InteractionId GetInteraction()
    {
        return InteractionID;
    }
    public void SetInteraction(InteractionId newInteraction)
    {
        this.InteractionID = newInteraction;
    }

    //start playing the ff related to this trigger
    public virtual void StartFeedforward()
    {
        int reps;

        if (Status == TriggerStatus.Active && !(Status == TriggerStatus.Feedforward))
        {

            if (Status == TriggerStatus.Feedforward)
                return;

            this.Status = TriggerStatus.Feedforward;

            bool rewindEnabled;

            if (RewindType == RewindStrategy.On)
                rewindEnabled = true;
            else
                rewindEnabled = false;

            if (repeat == RepeatStrategy.Continuous)
                reps = RemainingRepetitions;
            else
                reps = 1;
            // maybe add restrictions here
            //not all combinations are valid
            //cannot have duplicate all and ghost none for example
            HandInteractionManager.OnFeedforward(Id,interaction,FeedforwardTarget,DuplicationType,GhostType,VisualizationType, rewindEnabled, reps);

        }
    }


    public virtual void InterruptFeedforward()
    {
        //
    }

    public virtual void ActivateTrigger()
    {
        this.Status = TriggerStatus.Active;
    }
    //trigger is dormant and may be activated
    public virtual void DeactivateTrigger()
    {
        this.Status = TriggerStatus.Deactivated;
    }

    //Trigger is removed from object and does not listen
    public virtual void DestroyTrigger()
    {
        this.Status = TriggerStatus.Destroy;
    }



}
