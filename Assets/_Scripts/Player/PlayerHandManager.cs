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
    [SerializeField] 
    private Transform spotLight;

    public GameObject SpriteScreen;
    private RenderTexture renderTexture;

    private Color gemColor;
    
    //public Texture2D testTexture;
    
    private void Start()
    {
        action = InputSystem.actions;
        action.FindAction("Attack").performed += FindGem;
        //action.FindAction("Look").performed += FindGem;
        action.FindAction("Look").performed += MoveImage;
        
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        playerHandCamera.targetTexture = renderTexture;
    }

    private void MoveImage(InputAction.CallbackContext obj)
    {
        var mousePosition = Mouse.current.position.ReadValue();
        inHandGem.rectTransform.anchoredPosition = mousePosition;
        Vector3 position = Camera.main!.ScreenToWorldPoint(mousePosition);
        position.z = 0;
        spotLight.position = position;
    }

    private void FindGem(InputAction.CallbackContext obj)
    {
        if (!inHandGem.gameObject.activeSelf)
            FindGemCoroutine();
        else
            ReleaseGem();
    }

    private void ReleaseGem()
    {
        Gem releaseGem = pool.Get();
        releaseGem.SetView(inHandGem.sprite, gemColor);
        Vector3 pos = Camera.main!.ScreenToWorldPoint(inHandGem.rectTransform.anchoredPosition);
        pos.z = 0;
        releaseGem.transform.position = pos;
        inHandGem.gameObject.SetActive(false);
    }

    private void FindGemCoroutine()
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
            return;

        pool.Return(firstGem);
        
        inHandGem.gameObject.SetActive(true);
    }

    private bool isGemOnPoint(Gem gem, Vector3 mousePosition)
    {
        inHandGem.sprite = gem.Sprite.sprite;
        gemColor = gem.Sprite.color;
        inHandGem.color = new Color(gemColor.r, gemColor.g, gemColor.b, 1);
        inHandGem.rectTransform.anchoredPosition = Camera.main!.WorldToScreenPoint(gem.transform.position);
        inHandGem.rectTransform.rotation = gem.transform.rotation;

        var testTexture = new Texture2D(Screen.width, Screen.height);
        testTexture.ReadPixels(new Rect(mousePosition.x, mousePosition.y, 1, 1), 0, 0);
        //testTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        testTexture.Apply();
        Color pixelColor = testTexture.GetPixel(0, 0);
        
        Destroy(testTexture);
        
        //Debug.Log(pixelColor);

        return pixelColor != backgroundColor;
    }
    
    private void OnDestroy()
    {
        action.FindAction("Attack").performed -= FindGem;
        action.FindAction("Look").performed -= MoveImage;
    }
}
