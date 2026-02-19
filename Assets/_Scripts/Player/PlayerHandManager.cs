using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable All

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
    
    public void Initialize()
    {
        action = InputSystem.actions;
        action.FindAction("Click").performed += FindGem;
        action.FindAction("Look").performed += MoveImage;
    }

    private void MoveImage(InputAction.CallbackContext obj)
    {
        var mousePosition = Mouse.current.position.ReadValue();
        Vector3 position = Camera.main!.ScreenToWorldPoint(mousePosition);
        position.y = Gem.centerZone.position.y;
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
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        WashHand();
        for (int i = pool.GemParent.childCount - 1; i >= 0; i--)
        {
            Gem checkedGem = pool.GemParent.GetChild(i).GetComponent<Gem>();
            if(!checkedGem.gameObject.activeSelf
               //|| !checkedGem.Sprite.bounds.Contains(worldMousePosition)
               || !isGemOnPoint(checkedGem, worldMousePosition))
                continue;

            gem = checkedGem;
            break;
        }
        
        if(gem == null)
            return;

        gem.enabled = false;
        
        gemData.Sprite = gem.Sprite.sprite;
 
        gemInHandOffset = Camera.main!.WorldToScreenPoint(gem.transform.position);

        enabled = true;
    }

    private bool isGemOnPoint(Gem gem, Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        float denominator = ray.direction.y;
        //if (Mathf.Approximately(denominator, 0))return false;

        float t = (gem.transform.position.y - ray.origin.y) / denominator;
        //if (t < 0) return false;

        Vector3 hitPoint = ray.origin + ray.direction * t;

        Debug.DrawLine(ray.origin, hitPoint, Color.yellow, 0.5f);
        float cross = 0.05f;
        Debug.DrawLine(hitPoint - Vector3.right * cross, hitPoint + Vector3.right * cross, Color.red, 0.5f);
        Debug.DrawLine(hitPoint - Vector3.forward * cross, hitPoint + Vector3.forward * cross, Color.red, 0.5f);

        Sprite gemSprite = gem.Sprite.sprite;

        Vector3 localMousePosition = gem.transform.InverseTransformPoint(hitPoint);
        Vector2 localPos = new Vector2(localMousePosition.x, localMousePosition.z);
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
