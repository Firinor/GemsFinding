using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine.Serialization;

public class GemInRecipe : MonoBehaviour
{
    [SerializeField]
    private Image image; 
    [SerializeField]
    private int gemID;
    
    public void SetView(Sprite sprite, Color color)
    {
        image.sprite = sprite;
        image.SetNativeSize();
        image.color = color;
        image.raycastTarget = false;
        enabled = false;
    }
    public void AddToRecipe(int id)
    {
        gemID = id;
    }
    public void Success()
    {
        //puzzleManager.Particles(
        //    MainCamera.WorldToScreenPoint(transform.position) / canvasManager.ScaleFactor,
        //    success: true);
        image.color = Color.white;
    }
}