using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Discription;
    [SerializeField] private TextMeshProUGUI Effect;
    [SerializeField] private TextMeshProUGUI Cost;
    [SerializeField] private TextMeshProUGUI Level;

    public void Show(MetaPointInfo info)
    {
        gameObject.SetActive(true);
        if(info.Ancor is not null)
            RectTransform.anchoredPosition = info.Ancor.position;

        Name.text = info.Name;
        Discription.text = info.Discription;
        if(info.Level == info.MaxLevel)
            Effect.text = $"{info.Effect}";
        else
            Effect.text = $"{info.Effect} => {info.NextEffect}";

        Cost.text = info.Cost;
        Level.text = $"{info.Level}/{info.MaxLevel}";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}