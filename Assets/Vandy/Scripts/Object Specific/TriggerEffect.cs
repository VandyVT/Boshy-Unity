using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEffect : MonoBehaviour
{
    [SerializeField] bool triggerOnce = true;

    [SerializeField] UnityEvent onTrigger;
    [SerializeField] Animation animationTrigger;

    private bool hasTriggered = false;

    private bool defaultTriggerOnce;

    void Start()
    {
        SaveDefaultValues();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && (!triggerOnce || !hasTriggered))
        {
            Trigger();

            if (triggerOnce)
                hasTriggered = true;
        }
    }

    void Trigger()
    {
        Debug.Log("Triggered!");

        onTrigger.Invoke();
        if (animationTrigger != null)
        {
            animationTrigger.Play();
        }
    }

    void SaveDefaultValues()
    {
        defaultTriggerOnce = triggerOnce;
    }

    public void ResetToDefaultValues()
    {
        triggerOnce = defaultTriggerOnce;
        hasTriggered = false; 
    }
}
