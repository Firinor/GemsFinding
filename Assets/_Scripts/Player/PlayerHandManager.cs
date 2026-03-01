using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using FirMath;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerHandManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool IsMobileBrowser();
#endif

    private static bool IsMobile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return IsMobileBrowser();
#else
        return SystemInfo.deviceType == DeviceType.Handheld;
#endif
    }
    
    private Gem gem;
    [SerializeField]
    private bool isMouseClickOn;
    private InputActionAsset action;
    [SerializeField] 
    private GemPool pool;
    [SerializeField] 
    private Recipe recipe;
    [SerializeField] 
    private Transform spotLight;
    [SerializeField]
    private float impulseCoefficient;
    [SerializeField] 
    private Camera _camera;
    [SerializeField] 
    private float sencivity;
    [SerializeField] 
    private Vector3[] cameraBorder;//[0] min - [1] max
    [SerializeField]
    private float zoomSpeed = 0.01f;

    private GemData gemData;

    private int lastPositionIndex;
    private Vector2[] lastMousePosition = new Vector2[5];
    private Vector2 mouseImpulse;
    private Vector3 cameraStartPosition;

    private Vector3 gemInHandOffset;

    private float previousPinchDistance;
    private bool isPinching;
    
    public void Initialize()
    {
        action = InputSystem.actions;
        EnhancedTouchSupport.Enable();
        action.FindAction("Click").performed += FindGem;
        if (IsMobile())
            action.FindAction("TouchLook").performed += MoveImage; 
        else
            action.FindAction("Look").performed += MoveImage;
    }

    private void MoveImage(InputAction.CallbackContext obj)
    {
        Vector3 hitPoint = GetRayHitPoint();
        
        spotLight.position = hitPoint;
        if (gem != null)
            gem.transform.position = hitPoint + gemInHandOffset;
        else if(isMouseClickOn)
        {
            Vector2 mouseDelta = obj.ReadValue<Vector2>();
            Vector3 delta = new Vector3(mouseDelta.y, 0, -mouseDelta.x);
            Vector3 newCameraPosition = _camera.transform.position + delta * sencivity;
            newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, cameraBorder[0].x, cameraBorder[1].x);
            newCameraPosition.z = Mathf.Clamp(newCameraPosition.z, cameraBorder[0].z, cameraBorder[1].z);
            _camera.transform.position = newCameraPosition;
        }
    }

    private void FindGem(InputAction.CallbackContext obj)
    {
        if (!isMouseClickOn && obj.control.IsPressed())
            FindGem();
        else if(isMouseClickOn && !obj.control.IsPressed())
            ReleaseGem();
    }

    private void FixedUpdate()
    {
        Vector2 currentMousePosition;
        if (Touch.activeTouches.Count > 0)
            currentMousePosition = Touch.activeTouches[0].screenPosition;
        else
            currentMousePosition = Mouse.current.position.ReadValue();

        lastMousePosition[lastPositionIndex] = currentMousePosition;
        lastPositionIndex = (lastPositionIndex + 1) % lastMousePosition.Length;
        mouseImpulse = currentMousePosition - lastMousePosition[lastPositionIndex];
    }

    private void Update()
    {
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll != 0)
                ZoomCamera(scroll * zoomSpeed);
        }
        
        if (Touch.activeTouches.Count == 2)
        {
            float currentDistance = Vector2.Distance(
                Touch.activeTouches[0].screenPosition,
                Touch.activeTouches[1].screenPosition);
            if (isPinching)
                ZoomCamera((currentDistance - previousPinchDistance) * zoomSpeed);
            isPinching = true;
            previousPinchDistance = currentDistance;
        }
        else
        {
            isPinching = false;
        }
    }

    private void ZoomCamera(float delta)
    {
        Vector3 pos = _camera.transform.position;
        pos.y = Mathf.Clamp(pos.y - delta, cameraBorder[0].y, cameraBorder[1].y);
        _camera.transform.position = pos;
    }

    private void WashHand()
    {
        if (gem is not null)
        {
            gem.enabled = true;
            gem.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Gems");
        }
        gem = null;
        mouseImpulse = Vector2.zero;
        
        Vector2 mousePosition;
        if (Touch.activeTouches.Count > 0)
            mousePosition = Touch.activeTouches[0].screenPosition;
        else
            mousePosition = Mouse.current.position.ReadValue();
        
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
        isMouseClickOn = false;
        
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
    }

    private void FindGem()
    {
        WashHand();
        
        isMouseClickOn = true;
        
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
        gem.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("VFX");
        
        gemData.Sprite = gem.Sprite.sprite;
 
        gemInHandOffset = gem.transform.position - hitPoint;
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
        Vector2 position;
        if (Touch.activeTouches.Count > 0)
            position = Touch.activeTouches[0].screenPosition;
        else
            position = Mouse.current.position.ReadValue();
        
        Ray ray = Camera.main.ScreenPointToRay(position);

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
        InputSystem.actions.FindAction("TouchLook").performed -= MoveImage;
    }
}
