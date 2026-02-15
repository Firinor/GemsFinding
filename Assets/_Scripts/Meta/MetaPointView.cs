using UnityEngine;
using UnityEngine.UI;

public class MetaPointView : MonoBehaviour
{
    public Image Frame;
    public Image Icon;
    public Image Plus;
    public Button Button;
    public MetaPointData Data;

    [ContextMenu(nameof(Initialize))]
    public void Initialize()
    {
        gameObject.SetActive(true);
        
        Icon.sprite = Data.Icon;
    }

    public void ToLevelFrame(Sprite levelFrame)
    {
        Frame.enabled = true;
        Frame.sprite = levelFrame;
    }

    public void ToMaxFrame(Sprite maxFrame)
    {
        Frame.enabled = true;
        Plus.enabled = false;
        Frame.sprite = maxFrame;
    }

    public void ShowPlus(bool v)
    {
        Plus.enabled = v;
    }

    public void ToDisableFrame()
    {
        Frame.enabled = false;
    }
}