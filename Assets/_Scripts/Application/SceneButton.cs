using MirraGames.SDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public string SceneName;
    public ADSManager AdsManager;

    public void SwitchToScene()
    {
        AdsManager.ShowAdsInterstitial(callback: () => SceneManager.LoadScene(SceneName));
    }
}
