using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MetaPointView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //public RectTransform InfoAncor;
    public Image Frame;
    public Image Icon;
    public Image Plus;
    //[SerializeField] private Sprite LevelFrame;
    //[SerializeField] private Sprite MaxFrame;
    
    public Button Button;
    public event Action<MetaPointData> OnPointerEnterAction;
    public event Action OnPointerExitAction;
    public MetaPointData Data;

    [ContextMenu(nameof(Initialize))]
    public void Initialize()
    {
        gameObject.SetActive(true);
        
        Icon.sprite = Data.Icon;
    }

    public void ToLevelFrame()
    {
        //Frame.sprite = LevelFrame;
    }

    public void ToMaxFrame()
    {
        Plus.enabled = false;
        //Frame.sprite = MaxFrame;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterAction?.Invoke(Data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitAction?.Invoke();
    }

    public void ShowPlus(bool v)
    {
        Plus.enabled = v;
    }
}