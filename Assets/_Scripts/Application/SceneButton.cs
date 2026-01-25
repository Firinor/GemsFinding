using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public string SceneName;

    public void SwitchToScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
