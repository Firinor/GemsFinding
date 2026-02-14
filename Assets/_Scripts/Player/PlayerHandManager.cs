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
    private Transform spotLight;
    [SerializeField] 
    private float impulseCoefficient;

    private GemData gemData;

    private int lastPositionIndex;
    private Vector2[] lastMousePosition = new Vector2[5];
    private Vector2 mouseImpulse;

    private Vector2 gemInHandOffset;
    
    private void Start()
    {
        action = InputSystem.actions;
        action.FindAction("Click").performed += FindGem;
        action.FindAction("Look").performed += MoveImage;
        enabled = false;
    }

    private void MoveImage(InputAction.CallbackContext obj)
    {
        var mousePosition = Mouse.current.position.ReadValue();
        Vector3 position = Camera.main!.ScreenToWorldPoint(mousePosition);
        position.z = 0;
        spotLight.position = position;
        if(gem != null)
            gem.transform.position = position;
    }

    private void FindGem(InputAction.CallbackContext obj)
    {
        if (!enabled && obj.control.IsPressed())
            FindGem();
        else if(enabled && !obj.control.IsPressed())
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
        if (gem is not null)
            gem.enabled = true;
        gem = null;
        mouseImpulse = Vector2.zero;
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        lastMousePosition = new Vector2[]
        {
            mousePosition,
            mousePosition, 
            mousePosition,
            mousePosition,
            mousePosition
        };
    }
    private void ReleaseGem()
    {
        if (gem is null)
            return;
        
        bool isCorrectGem = recipe.CheckGem(gemData);

        if (!isCorrectGem)
        {
            gem.SetSortImpulse(mouseImpulse * impulseCoefficient);
        }
        else
        {
            pool.Return(gem);
        }
        
        WashHand();
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
        
        if(gem == null)
            return;

        gem.enabled = false;
        gem.RemoveDirt();
        
        gemData.Sprite = gem.Sprite.sprite;
        gemData.Color = gem.Sprite.color;
 
        gemInHandOffset = Camera.main!.WorldToScreenPoint(gem.transform.position);

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
