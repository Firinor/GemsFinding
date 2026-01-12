using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHandManager : MonoBehaviour
{
    private Gem gem;
    private InputActionAsset action;
    [SerializeField] 
    private GemPool pool;
    [SerializeField] 
    private Image inHandGem;
    [SerializeField] 
    private Camera playerHandCamera;
    [SerializeField] 
    private Color backgroundColor;

    public GameObject SpriteScreen;
    private RenderTexture renderTexture;

    //public Texture2D testTexture;
    
    private void Start()
    {
        action = InputSystem.actions;
        action.FindAction("Click").performed += FindGem;
        //action.FindAction("Look").performed += FindGem;
        //action.FindAction("Look").performed += MoveImage;
        
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        playerHandCamera.targetTexture = renderTexture;
    }

    private void MoveImage(InputAction.CallbackContext obj)
    {
        inHandGem.rectTransform.anchoredPosition = Mouse.current.position.ReadValue();
    }

    private void FindGem(InputAction.CallbackContext obj)
    {
        if (!obj.control.IsPressed())
            return;

        StartCoroutine(FindGemCoroutine());
    }

    private IEnumerator FindGemCoroutine()
    {
        SpriteScreen.SetActive(true);
        inHandGem.gameObject.SetActive(true);
        
        //yield return new WaitForEndOfFrame();
        RenderTexture.active = renderTexture;
        playerHandCamera.Render();
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = Camera.main!.ScreenToWorldPoint(mousePosition);

        Gem firstGem = null;

        var gems = pool.Gems;
        for (int i = gems.Count - 1; i >= 0; i--)
        {
            if(!gems[i].gameObject.activeSelf
               || !gems[i].Sprite.bounds.Contains(worldMousePosition)
               || !isGemOnPoint(gems[i], mousePosition))
                continue;

            firstGem = gems[i];
            break;
        }
        
        SpriteScreen.SetActive(false);
        inHandGem.gameObject.SetActive(false);
        RenderTexture.active = null;
            
        if(firstGem is null)
            yield break;

        inHandGem.gameObject.SetActive(true);
    }

    private bool isGemOnPoint(Gem gem, Vector3 mousePosition)
    {
        inHandGem.sprite = gem.Sprite.sprite;
        inHandGem.color = gem.Sprite.color;
        inHandGem.rectTransform.anchoredPosition = Camera.main!.WorldToScreenPoint(gem.transform.position);
        inHandGem.rectTransform.rotation = gem.transform.rotation;

        var testTexture = new Texture2D(Screen.width, Screen.height);
        testTexture.ReadPixels(new Rect(mousePosition.x, mousePosition.y, 1, 1), 0, 0);
        //testTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        testTexture.Apply();
        Color pixelColor = testTexture.GetPixel(0, 0);
        
        Destroy(testTexture);
        
        Debug.Log(pixelColor);

        return pixelColor != backgroundColor;
    }
    
    private void OnDestroy()
    {
        action.FindAction("Click").performed -= FindGem;
        action.FindAction("Look").performed -= MoveImage;
    }
}
