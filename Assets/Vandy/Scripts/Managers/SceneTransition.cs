using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;
    public UnityEvent onComplete;
    public UnityEvent onFail;

    public bool holdFlag;
    public Image asyncLoadBar;
    public Slider asyncLoadSlider;

   public void changeScene (string sceneTarget)   {

        SceneTransition.instance = Instantiate(this.gameObject).GetComponent<SceneTransition>();
        DontDestroyOnLoad(SceneTransition.instance.gameObject);
        SceneTransition.instance.StartCoroutine(SceneTransition.instance.LoadScene(sceneTarget));
        print("Changing the scene to " + sceneTarget);
   }
    

    IEnumerator LoadScene(string destinationScene)
    {
        
         if (!Application.CanStreamedLevelBeLoaded (destinationScene)) {

        Debug.LogError (destinationScene + " is not on the list!!");
        //errorScreen.SetActive (true);
        onFail.Invoke ();
        yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync (destinationScene);

        if(holdFlag == true) {
			asyncLoad.allowSceneActivation = false;
			print("Hey! Gimme a minute!");
		}

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
                if(asyncLoadBar) asyncLoadBar.fillAmount = asyncLoad.progress;
                if(asyncLoadSlider) asyncLoadSlider.value = asyncLoad.progress;

                if(holdFlag == false) asyncLoad.allowSceneActivation = true;
                yield return new WaitForEndOfFrame();
        }

            AudioListener.pause = false;
        onComplete.Invoke ();
        Destroy(gameObject,3);
    }
}