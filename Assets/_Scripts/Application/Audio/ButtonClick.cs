using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    private void Start()
    {
        var toggle = GetComponent<Toggle>();

        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(v => OnClickSound());
            return;
        }

        var button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnClickSound);
        }
    }

    public void OnClickSound()
    {
        if(SoundManager.Instance == null)
            return;
        
        SoundManager.Instance.PlayButtonClick();
    }
}
