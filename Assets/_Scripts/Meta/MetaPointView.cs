using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaPointView : MonoBehaviour
{
    public Image Icon;
    public Button Button;
    public TextMeshProUGUI LevelText;
    public MetaPointData Data;

    [ContextMenu(nameof(Initialize))]
    public void Initialize()
    {
        gameObject.SetActive(true);
        
        Icon.sprite = Data.Icon;
    }
    
    public void Initialize(PlayerDataMetaPoint data)
    {
        LevelText.text = data.Level + "/" + Data.MaxLevel;
    }
}