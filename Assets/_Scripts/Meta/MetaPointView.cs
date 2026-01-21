using UnityEngine;
using UnityEngine.UI;

public class MetaPointView : MonoBehaviour
{
    public Image Icon;
    public Button Button;
    public MetaPointData Data;

    [ContextMenu(nameof(Initialize))]
    private void Initialize()
    {
        Icon.sprite = Data.Icon;
    }
}