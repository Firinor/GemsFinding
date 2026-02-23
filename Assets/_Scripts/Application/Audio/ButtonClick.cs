using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    public void OnClickSound()
    {
        if(SoundManager.Instance == null)
            return;
        
        SoundManager.Instance.PlayButtonClick();
    }
}
