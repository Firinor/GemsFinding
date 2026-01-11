using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using Unity.Mathematics;

public class Gem : MonoBehaviour
{
    private static Bounds bounds;
    
    [SerializeField]
    private float mass;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField] 
    private float brakingFactor;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer Sprite => spriteRenderer;
    [SerializeField]
    private int id;

    private bool drag = false;
    private float timer;
    private const float FIFTH_SEC = 0.2f;
    private const int ERROR_FORCE = 10;

    private Vector3 impulse;
    private float rotation;
    private float rotationFromSpeedCoefficient;
    private Vector3 lastPosition;
    private bool ingredientDrag = false;

    void FixedUpdate()
    {
        if (ingredientDrag)
            return;

        if (!bounds.Contains(transform.localPosition))
            GravityForce();
        
        if (impulse != Vector3.zero)
            ForceToIngredient();
    }

    private void GravityForce()
    {
        impulse += (bounds.center - transform.localPosition).normalized * (Physics2D.gravity.magnitude * Time.fixedDeltaTime);
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
    private void ForceToIngredient()
    {
        Vector3 pos = transform.localPosition;

        if (bounds.Contains(pos))
        {
            Vector3 afterPos = pos + impulse * Time.fixedDeltaTime;
            if (afterPos.x < bounds.min.x || afterPos.x > bounds.max.x)
            {
                impulse.x = -impulse.x;
                NewRotation();
                //afterPos.x = Mathf.Clamp(afterPos.x, bounds.min.x, bounds.max.x);
            }
            if (afterPos.y < bounds.min.y || afterPos.y > bounds.max.y)
            {
                impulse.y = -impulse.y;
                NewRotation();
                //afterPos.y = Mathf.Clamp(afterPos.y, bounds.min.y, bounds.max.y);
            }
        }

        pos += impulse * Time.fixedDeltaTime;

        Vector3 brakingVector = impulse.normalized * brakingFactor * Time.fixedDeltaTime;

        if (impulse.magnitude > brakingVector.magnitude)
            impulse -= brakingVector;
        else
            impulse = Vector3.zero;

        transform.localPosition = pos;
        
        if(rotationFromSpeedCoefficient == 0)
            return;
        
        rotation = impulse.magnitude / rotationFromSpeedCoefficient;
        
        transform.rotation *= Quaternion.Euler(0,0, rotation); 
    }
    
    public void SetRandomImpulse(float forse, bool randomForse = true)
    {
        forse *= randomForse ? Random.value : 1;
        float randomDirection = Random.value * 360 * Mathf.Deg2Rad;

        impulse = new Vector3(math.cos(randomDirection), math.sin(randomDirection), 0) * forse;
        
        NewRotation();
    }

    private void NewRotation()
    {
        rotation = rotationSpeed * Random.value;
        rotation *= FirMath.GameMath.HeadsOrTails() ? 1 : -1;
        
        rotationFromSpeedCoefficient = impulse.magnitude / rotation;
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