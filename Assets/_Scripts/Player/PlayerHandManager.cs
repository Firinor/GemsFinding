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

    private void Start()
    {
        action = InputSystem.actions;
        action.FindAction("Click").performed += FindGem;
        action.FindAction("Look").performed += MoveImage;
    }

    private void MoveImage(InputAction.CallbackContext obj)
    {
        inHandGem.rectTransform.anchoredPosition = Mouse.current.position.ReadValue();
    }

    private void FindGem(InputAction.CallbackContext obj)
    {
        if (!obj.control.IsPressed())
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = Camera.main!.ScreenToWorldPoint(mousePosition);

        Gem firstGem = pool.Gems
            .Where(g => g.gameObject.activeSelf)
            .LastOrDefault(g => g.Sprite.bounds.Contains(worldMousePosition));
        
        if(firstGem is null)
            return;

        inHandGem.sprite = firstGem.Sprite.sprite;
        inHandGem.color = firstGem.Sprite.color;
    }

    private void OnDestroy()
    {
        action.FindAction("Click").performed -= FindGem;
        action.FindAction("Look").performed -= MoveImage;
    }
}
