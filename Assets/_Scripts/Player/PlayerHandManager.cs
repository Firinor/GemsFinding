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
    private RectTransform mouseFolower;
    [SerializeField] 
    private Image inHandGem;
    [SerializeField] 
    private Transform spotLight;
    [SerializeField] 
    private float impulseCoefficient;

    private GemData gemData;

    private int lastPositionIndex;
    private readonly Vector2[] lastMousePosition = new Vector2[5];
    private Vector2 mouseImpulse;

    private Vector2 gemInHandOffset;
    
    private void Start()
    {
        action = InputSystem.actions;
        action.FindAction("Click").performed += FindGem;
        action.FindAction("Look").performed += MoveImage;
    }

    private void MoveImage(InputAction.CallbackContext obj)
    {
        var mousePosition = Mouse.current.position.ReadValue();
        mouseFolower.anchoredPosition = mousePosition;
        Vector3 position = Camera.main!.ScreenToWorldPoint(mousePosition);
        position.z = 0;
        spotLight.position = position;
    }

    private void FindGem(InputAction.CallbackContext obj)
    {
        if (!inHandGem.gameObject.activeSelf && obj.control.IsPressed())
            FindGem();
        else if(inHandGem.gameObject.activeSelf && !obj.control.IsPressed())
            ReleaseGem();
    }

    private void FixedUpdate()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        lastMousePosition[lastPositionIndex] = currentMousePosition;
        lastPositionIndex = (lastPositionIndex+1) % lastMousePosition.Length;
        mouseImpulse = currentMousePosition - lastMousePosition[lastPositionIndex];
    }

    public void WashHand()
    {
        gem = null;
    }
    private void ReleaseGem()
    {
        if (gem is null)
            return;
        
        bool isCorrectGem = recipe.CheckGem(gemData);

        if (!isCorrectGem)
        {
            //Vector2 currentMousePosition = Mouse.current.position.ReadValue();
            Vector3 pos = inHandGem.transform.position;
            pos.z = 0;
            
            gem.transform.position = pos;
            gem.SetSortImpulse(mouseImpulse * impulseCoefficient);
            gem.gameObject.SetActive(true);
        }
        
        inHandGem.gameObject.SetActive(false);
        enabled = false;
    }

    private void FindGem()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = Camera.main!.ScreenToWorldPoint(mousePosition);
        
        int index = 0;
        
        WashHand();
        for (int i = pool.GemParent.childCount - 1; i >= 0; i--)
        {
            Gem checkedGem = pool.GemParent.GetChild(i).GetComponent<Gem>();
            if(!checkedGem.gameObject.activeSelf
               || !checkedGem.Sprite.bounds.Contains(worldMousePosition)
               || !isGemOnPoint(checkedGem, worldMousePosition, ref index))
                continue;

            gem = checkedGem;
            break;
        }
        
        if(gem is null)
            return;

        pool.Return(gem);
        
        inHandGem.sprite = gem.Sprite.sprite;
        gemData.Sprite = inHandGem.sprite;
        gemData.Color = gem.Sprite.color;
        inHandGem.color = new Color(gemData.Color.r, gemData.Color.g, gemData.Color.b, 1);
        gemInHandOffset = Camera.main!.WorldToScreenPoint(gem.transform.position);
        inHandGem.rectTransform.anchoredPosition = gemInHandOffset - mousePosition;
        inHandGem.rectTransform.rotation = gem.transform.rotation;
        
        inHandGem.gameObject.SetActive(true);
        enabled = true;
    }

    private bool isGemOnPoint(Gem gem, Vector3 mousePosition, ref int index)
    {
        Sprite gemSprite = gem.Sprite.sprite;

        Vector2 localPos = gem.transform.InverseTransformPoint(mousePosition);
        localPos *= 100; // Texture scale
        localPos += gemSprite.pivot;

        if (localPos.x < 0
            || localPos.y < 0
            || localPos.x >= gemSprite.rect.width
            || localPos.y >= gemSprite.rect.height)
            return false;
        
        var testTexture = gemSprite.texture;
        Vector2 spritePos = gemSprite.rect.position;
        Vector2Int pixelPos = new Vector2Int((int)(spritePos.x + localPos.x), (int)(spritePos.y + localPos.y));
        Color pixelColor = testTexture.GetPixel(pixelPos.x, pixelPos.y);
        
        return pixelColor.a > 0;
    }
    
    private void OnDestroy()
    {
        action.FindAction("Click").performed -= FindGem;
        action.FindAction("Look").performed -= MoveImage;
    }
}
