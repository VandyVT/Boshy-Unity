/*
 * BufferedEvent.cs
 * Created by: Newgame+ LD
 * Created on: ??/??/???? (dd/mm/yy)
 * 
 * This script is used to trigger a Unity event after a certain period of time
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BufferedEvent : MonoBehaviour {

  public UnityEvent onBufferDone;
  public bool triggerOnActive;
  public float triggerOnActiveTime;
	public bool unscaled;

  void OnEnable () {

    if (triggerOnActive) TriggerEventAfterTime (triggerOnActiveTime);
  }

  public void TriggerEventAfterTime (float delay) {

		if((this.enabled && this.gameObject.activeSelf))
    StartCoroutine (execute (delay));
		else print("Event on " + gameObject.name + "is not active! Skipping this.");

  }

  public void TriggerInstantly () {

    if((this.enabled && this.gameObject.activeSelf))
    onBufferDone.Invoke ();
    else print("Instant trigger failed. " + gameObject.name + " or its component ain't active!");

    //if(!gameObject.activeSelf) print("GameObject was not active");
    //if(!this.enabled) print("Component was not active");
  }

  IEnumerator execute (float delay) {

    if(!unscaled)yield return new WaitForSeconds (delay);
		else yield return new WaitForSecondsRealtime (delay);
    if((this.enabled && this.gameObject.activeSelf))
    onBufferDone.Invoke ();
    else print("Failed to execute event. Game object was already disabled!");

  }

}
