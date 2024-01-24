using System;
using UnityEngine;

[Flags]
public enum ResetOptions
{
    None = 0,
    SaveInitialPosition = 1 << 0,
    SaveInitialRotation = 1 << 1,
    SaveInitialScale = 1 << 2,
    SaveAnimationState = 1 << 3,
    SaveTriggerState = 1 << 4,
}

public class ResetCondition : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    [SerializeField] Animation animationState;
    [SerializeField] TriggerEffect triggerEffect;

    public ResetOptions resetOptions;

    void Awake()
    {
        StoreInitialState();
    }

    private void StoreInitialState()
    {
        if ((resetOptions & ResetOptions.SaveInitialPosition) != 0)
            initialPosition = transform.position;

        if ((resetOptions & ResetOptions.SaveInitialRotation) != 0)
            initialRotation = transform.rotation;

        if ((resetOptions & ResetOptions.SaveInitialScale) != 0)
            initialScale = transform.localScale;
        if ((resetOptions & ResetOptions.SaveAnimationState) != 0)
        {
            if (animationState == null)
            {
                animationState = GetComponent<Animation>();
            }
        }; 
    }

    public void ResetObjectState()
    {
        // Reset the object to its initial state based on the selected reset options
        if ((resetOptions & ResetOptions.SaveInitialPosition) != 0)
            transform.position = initialPosition;

        if ((resetOptions & ResetOptions.SaveInitialRotation) != 0)
            transform.rotation = initialRotation;

        if ((resetOptions & ResetOptions.SaveInitialScale) != 0)
            transform.localScale = initialScale;

        if ((resetOptions & ResetOptions.SaveAnimationState) != 0 && animationState != null)
        {
            // Check if the animation is playing or has completed
            if ((resetOptions & ResetOptions.SaveAnimationState) != 0 && animationState != null)
            {
                // Stop the animation
                animationState.Stop();
            }
        }

        if ((resetOptions & ResetOptions.SaveTriggerState) != 0)
        {
            triggerEffect.ResetToDefaultValues();
        }
    }
}
