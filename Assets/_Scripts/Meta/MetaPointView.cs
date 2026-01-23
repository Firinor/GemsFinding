using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaPointView : MonoBehaviour
{
    public Image Frame;
    public Image Icon;
    [SerializeField] private Sprite LevelFrame;
    [SerializeField] private Sprite MaxFrame;
    
    public Button Button;
    public TextMeshProUGUI LevelText;
    public MetaPointData Data;

    [ContextMenu(nameof(Initialize))]
    public void Initialize()
    {
        gameObject.SetActive(true);
        
        Icon.sprite = Data.Icon;
    }
    
    public void SetText(int level)
    {
        LevelText.text = level + "/" + Data.MaxLevel;
    }

    public void ToLevelFrame()
    {
        Frame.sprite = LevelFrame;
    }

    public void ToMaxFrame()
    {
        Frame.sprite = MaxFrame;
    }
}