using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public string SceneName;
    public ADSManager AdsManager;

    public void SwitchToScene()
    {
        if (AdsManager != null)
            AdsManager.ShowAdsInterstitial(callback: () => SceneManager.LoadScene(SceneName));
        else
            SceneManager.LoadScene(SceneName);
    }
}
