using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DontDestroyOnLoadScript : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
