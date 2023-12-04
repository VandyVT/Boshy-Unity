using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScn : MonoBehaviour
{
    public void IntroShaker()
    {
        CameraManager.instance.Shake(0.5f, 0.5f, 0.25f);
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("scn_mainMenu");
    }
}