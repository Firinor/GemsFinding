using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class PlayerHandManager : MonoBehaviour
{
    private Gem gem;
    private InputActionAsset action;
    [SerializeField] 
    private GemPool pool;
    [SerializeField] 
    private Image inHandGem;
    [SerializeField, Range(0,1)] 
    private float alfaEdge;

    public Image Indicator;
    
    private void Start()
    {
        action = InputSystem.actions;
        action.FindAction("Click").performed += FindGem;
        //action.FindAction("Look").performed += FindGem;
        //action.FindAction("Look").performed += MoveImage;
    }

    private void MoveImage(InputAction.CallbackContext obj)
    {
        inHandGem.rectTransform.anchoredPosition = Mouse.current.position.ReadValue();
    }

    private void FindGem(InputAction.CallbackContext obj)
    {
        if (!obj.control.IsPressed())
            return;

        Indicator.color = Color.clear;
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = Camera.main!.ScreenToWorldPoint(mousePosition);

        Gem firstGem = pool.Gems
            .Where(g => g.gameObject.activeSelf)
            .LastOrDefault(
                g => g.Sprite.bounds.Contains(worldMousePosition) 
                         && isGemOnPoint(g, worldMousePosition));
        
        if(firstGem is null)
            return;

        inHandGem.sprite = firstGem.Sprite.sprite;
        inHandGem.color = firstGem.Sprite.color;
    }

    private bool isGemOnPoint(Gem gem, Vector3 mousePosition)
    {
        float angle = gem.transform.rotation.eulerAngles.z;
        Vector3 gemCenterPoint = gem.transform.position;
        //Vector2 pointOnTexture = gem.transform.InverseTransformPoint(mousePosition);

        float x, y;

        Vector3 mouseDelta = ((mousePosition - gemCenterPoint) * 100) + new Vector3(32,32,0);

        x = mouseDelta.x * Mathf.Cos(angle) + mouseDelta.y * Mathf.Sin(angle);
        y = mouseDelta.y * Mathf.Cos(angle) + mouseDelta.x * Mathf.Sin(angle);

        Debug.Log($"x:{x}, y:{y}");
        //pointOnTexture += (Vector2)gem.Sprite.bounds.max;
        //pointOnTexture *= 100;

        if (gem.Sprite.sprite.texture.GetPixel((int)x, (int)y).a > alfaEdge)
        {
            gem.Sprite.sprite.texture.SetPixel((int)x, (int)y, Color.green);
            Indicator.color = Color.green;
        }
        else
            Indicator.color = Color.red;
        
        gem.Sprite.sprite.texture.Apply();
        
        return gem.Sprite.sprite.texture.GetPixel((int)x, (int)y).a > alfaEdge;
    }
    
    private void OnDestroy()
    {
        action.FindAction("Click").performed -= FindGem;
        action.FindAction("Look").performed -= MoveImage;
    }
}
