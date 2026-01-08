using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using Unity.Mathematics;

public class Gem : MonoBehaviour,
IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static Bounds bounds;
    
    [SerializeField]
    private float mass;
    [SerializeField] 
    private float brakingFactor;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private int id;

    private bool drag = false;
    private float timer;
    private const float FIFTH_SEC = 0.2f;
    private const int ERROR_FORCE = 10;

    private Vector3 impulse;
    private Vector3 lastPosition;
    private bool ingredientDrag = false;

    void FixedUpdate()
    {
        if (!ingredientDrag)
        {
            if (impulse != Vector3.zero)
                ForseToIngredient();
        }
    }
    void Update()
    {
        if (ingredientDrag && EveryFifthSec())
            CheckLastPosition();
    }

    public void ResetPhysics()
    {
        drag = false;
        ingredientDrag = false;
        impulse = Vector3.zero;
        lastPosition = Vector3.zero;
        timer = 0;
    }

    private bool EveryFifthSec()
    {
        timer += Time.deltaTime;
        if (timer > FIFTH_SEC)
        {
            timer -= FIFTH_SEC;
            return true;
        }
        return false;
    }
    private void CheckLastPosition()
    {
        lastPosition = transform.localPosition;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        ingredientDrag = true;
        CheckLastPosition();
        //startMousePosition = Input.mousePosition/ canvasManager.ScaleFactor - transform.localPosition;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            drag = true;
        }
        else
        {
            eventData.pointerDrag = null;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (drag && eventData.button == PointerEventData.InputButton.Left)
        {
            //transform.localPosition = Input.mousePosition / canvasManager.ScaleFactor - startMousePosition;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        ingredientDrag = false;
       
        if (false)
        {
            if (id > 0)
            {
                Destroy(gameObject);
            }
            else
            {
                //puzzleManager.Particles(Input.mousePosition / canvasManager.ScaleFactor, success: false);
                SetRandomImpulse(ERROR_FORCE, randomForse: false);
            }
            return;
        }
        impulse = (transform.localPosition - lastPosition) / mass;
        //Debug.Log(impulse + " " + impulse.x + " " + impulse.y);
    }
    private void ForseToIngredient()
    {
        Vector3 pos = transform.localPosition;

        Vector3 afterPos = pos + impulse * Time.fixedDeltaTime;
        if (afterPos.x < bounds.min.x || afterPos.x > bounds.max.x)
        {
            impulse.x = -impulse.x;
            //afterPos.x = Mathf.Clamp(afterPos.x, bounds.min.x, bounds.max.x);
        }
        if (afterPos.y < bounds.min.y || afterPos.y > bounds.max.y)
        {
            impulse.y = -impulse.y;
            //afterPos.y = Mathf.Clamp(afterPos.y, bounds.min.y, bounds.max.y);
        }
        
        pos += impulse * Time.fixedDeltaTime;

        Vector3 brakingVector = impulse.normalized * brakingFactor * Time.fixedDeltaTime;

        if (impulse.magnitude > brakingVector.magnitude)
            impulse -= brakingVector;
        else
            impulse = Vector3.zero;

        transform.localPosition = pos;
    }
    
    public void SetRandomImpulse(float forse, bool randomForse = true)
    {
        forse *= randomForse ? Random.value : 1;
        float randomDirection = Random.value * 360 * Mathf.Deg2Rad;

        impulse = new Vector3(math.cos(randomDirection), math.sin(randomDirection), 0) * forse;
    }
    public void SetImpulse(Vector3 impulse)
    {
        this.impulse = impulse;
    }
    public void SetImpulse(float force, bool toZeroPoint = true)
    {
        if (force == 0)
        {
            SetImpulse(Vector3.zero);
            return;
        }

        if (!toZeroPoint)
            force *= -1;

        Vector3 resultVector = (Vector3.zero - transform.localPosition).normalized * force;
        SetImpulse(resultVector);
    }
    public void SetView(Sprite sprite, Color color)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
    }
    public void AddToRecipe(int number)
    {
        id = number;
    }
    public void Success()
    {
        //puzzleManager.Particles(
        //    MainCamera.WorldToScreenPoint(transform.position) / canvasManager.ScaleFactor,
        //    success: true);
        spriteRenderer.color = Color.white;
    }
    public bool OnBox(float border)
    {
        Vector3 pos = transform.localPosition;
        return Mathf.Abs(pos.x) < border && Mathf.Abs(pos.y) < border;
    }

    public void SetBounds(Bounds gemZoneBounds)
    {
        bounds = gemZoneBounds; 
    }
}