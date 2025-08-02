using UnityEngine;

public class MobileControls : MonoBehaviour
{
    public static MobileControls Instance;

    private void Awake()
    {
        if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
                Debug.Log("MobileControls instance created and marked as DoNotDestroy.");
            }

            else
            {
                Debug.Log("MobileControls instance already exists. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
