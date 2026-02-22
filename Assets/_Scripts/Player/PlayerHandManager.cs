using UnityEngine;
using UnityEngine.InputSystem;
using FirMath;
// ReSharper disable All

public class PlayerHandManager : MonoBehaviour
{
    private Gem gem;
    [SerializeField]
    private bool isGemOn;
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

    private Vector3 gemInHandOffset;
    
    public void Initialize()
    {
        action = InputSystem.actions;
        action.FindAction("Click").performed += FindGem;
        action.FindAction("Look").performed += MoveImage;
    }

    private void MoveImage(InputAction.CallbackContext obj)
    {
        Vector3 hitPoint = GetRayHitPoint();
        
        spotLight.position = hitPoint;
        if(gem != null)
            gem.transform.position = hitPoint + gemInHandOffset;
    }

    private void FindGem(InputAction.CallbackContext obj)
    {
        if (!isGemOn && obj.control.IsPressed())
            FindGem();
        else if(isGemOn && !obj.control.IsPressed())
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

        gemData.Position = gem.transform.position;

        if (!recipe.IsPlayerOnRecipe)
        {
            gem.SetSortImpulse(new Vector3(-mouseImpulse.y, 0, mouseImpulse.x) * impulseCoefficient);
        }
        else
        {
            bool isCorrectGem = recipe.CheckGem(gemData);
            if (isCorrectGem)
                pool.Return(gem);
        }
        
        WashHand();
        isGemOn = false;
        enabled = false;
    }

    private void FindGem()
    {
        WashHand();
        
        Vector3 hitPoint = GetRayHitPoint();
        
        for (int i = pool.GemParent.childCount - 1; i >= 0; i--)
        {
            Gem checkedGem = pool.GemParent.GetChild(i).GetComponent<Gem>();
            if(!checkedGem.gameObject.activeSelf
               || !isGemOnPoint(checkedGem, hitPoint))
                continue;

            gem = checkedGem;
            break;
        }
        
        if(gem == null)
            return;

        gem.enabled = false;
        
        gemData.Sprite = gem.Sprite.sprite;
 
        gemInHandOffset = gem.transform.position - hitPoint;

        isGemOn = true;
        enabled = true;
    }

    private bool isGemOnPoint(Gem gem, Vector3 hitPoint)
    {
        //Debug.Log($"hitPoint: {hitPoint}");
        Sprite gemSprite = gem.Sprite.sprite;
        //Debug.Log($"Bounds: {gemSprite.bounds}");
        //Debug.Log($"Offset: {hitPoint - gem.transform.position}");
        if (!gemSprite.bounds.Contains((hitPoint - gem.transform.position).XZ()))
            return false;
        
        Vector2 localPos = gem.transform.InverseTransformPoint(hitPoint);
        localPos *= gemSprite.pixelsPerUnit; // Texture scale
        localPos += gemSprite.pivot;
        
        Texture2D testTexture = gemSprite.texture;
        Vector2 spritePos = gemSprite.textureRect.position;
        Vector2Int pixelPos = new Vector2Int((int)(spritePos.x + localPos.x), (int)(spritePos.y + localPos.y));
        Color pixelColor = testTexture.GetPixel(pixelPos.x, pixelPos.y);
        //Debug.Log($"pixelColor: {pixelColor}");
        return pixelColor.a > 0;
    }

    private static Vector3 GetRayHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        float denominator = ray.direction.y;
        //if (Mathf.Approximately(denominator, 0))return false;

        float t = (Gem.centerZone.position.y - ray.origin.y) / denominator;
        //if (t < 0) return false;

        Vector3 hitPoint = ray.origin + ray.direction * t;
        
        Debug.DrawLine(ray.origin, hitPoint, Color.yellow, 0.5f);
        float cross = 0.05f;
        Debug.DrawLine(hitPoint - Vector3.right * cross, hitPoint + Vector3.right * cross, Color.red, 0.5f);
        Debug.DrawLine(hitPoint - Vector3.forward * cross, hitPoint + Vector3.forward * cross, Color.red, 0.5f);
        
        return hitPoint;
    }

    private void OnDestroy()
    {
        InputSystem.actions.FindAction("Click").performed -= FindGem;
        InputSystem.actions.FindAction("Look").performed -= MoveImage;
    }
}
