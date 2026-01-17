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
    private Recipe recipe;
    [SerializeField] 
    private Image inHandGem;
    [SerializeField] 
    private Camera playerHandCamera;
    [SerializeField] 
    private Color backgroundColor;
    [SerializeField] 
    private Transform spotLight;
    [SerializeField] 
    private float impulseCoefficient;

    public GameObject SpriteScreen;
    private RenderTexture renderTexture;

    private GemData gemData;

    private int lastPositionIndex;
    private readonly Vector2[] lastMousePosition = new Vector2[5];
    private Vector2 mouseImpulse;
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

    private void FixedUpdate()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        lastMousePosition[lastPositionIndex] = currentMousePosition;
        lastPositionIndex = (lastPositionIndex+1) % lastMousePosition.Length;
        mouseImpulse = currentMousePosition - lastMousePosition[lastPositionIndex];
    }

    private void ReleaseGem()
    {
        bool isCorrectGem = recipe.CheckGem(gemData);

        if (!isCorrectGem)
        {
            Gem releaseGem = pool.Get();
            releaseGem.SetView(inHandGem.sprite, gemData.Color);
            Vector3 pos = Camera.main!.ScreenToWorldPoint(inHandGem.rectTransform.anchoredPosition);
            pos.z = 0;
            releaseGem.transform.position = pos;
            releaseGem.SetImpulse(mouseImpulse * impulseCoefficient);
        }
        
        inHandGem.gameObject.SetActive(false);
        enabled = false;
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
        enabled = true;
    }

    private bool isGemOnPoint(Gem gem, Vector3 mousePosition)
    {
        inHandGem.sprite = gem.Sprite.sprite;
        gemData.Sprite = inHandGem.sprite;
        gemData.Color = gem.Sprite.color;
        inHandGem.color = new Color(gemData.Color.r, gemData.Color.g, gemData.Color.b, 1);
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
