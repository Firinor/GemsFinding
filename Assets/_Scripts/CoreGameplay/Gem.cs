using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine.Rendering.Universal;

public class Gem : MonoBehaviour
{
    public static GemBox box;
    public static BoxCollider2D riverZone;
    private static Bounds bounds => box.boxZone.bounds;

    public Action<Vector3> OnBoundTink;
    public Action<Gem> OnEdge;
    
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private TrailRenderer trailRenderer;
    [SerializeField]
    private Light2D Light2D;
    [SerializeField]
    private Transform dirt;
    public SpriteRenderer Sprite => spriteRenderer;
    [SerializeField] 
    private Rigidbody2D rigidbody2D;
    
    private const float BREAKING_FACTOR = 3;
    private const int ERROR_FORCE = 8;
    private const float X_EDGE = -50;
    private const float BOX_MOVE_COEFFICIENT = 1;
    private const float CLEANING_SPEED = .3f;

    private Vector3 impulse;
    private float rotation;
    private float rotationFromSpeedCoefficient;

    private Action UpdateBehaviour;

    private void Update()
    {
        UpdateBehaviour?.Invoke();
    }

    void CachUpdate()
    {
        if (transform.localPosition.x < X_EDGE)
        {
            OnEdge?.Invoke(this);
            return;
        }

        GravityForce();
    }

    void SortUpdate()
    {
        if (!bounds.Contains(transform.localPosition))
            GravitySortForce();

        if (impulse != Vector3.zero)
            ForceToIngredient();
    }

    private void GravitySortForce()
    {
        impulse += (bounds.center - transform.position).normalized * (10 * Time.deltaTime);
    }

    private void ForceToIngredient()
    {
        Vector3 pos = transform.localPosition;

        if (bounds.Contains(pos))
        {
            Vector3 afterPos = pos + impulse * Time.deltaTime;
            if (afterPos.x < bounds.min.x || afterPos.x > bounds.max.x)
            {
                impulse.x = -impulse.x;
                NewSortRotation();
                OnBoundTink?.Invoke(pos);
                //afterPos.x = Mathf.Clamp(afterPos.x, bounds.min.x, bounds.max.x);
            }
            if (afterPos.y < bounds.min.y || afterPos.y > bounds.max.y)
            {
                impulse.y = -impulse.y;
                NewSortRotation();
                OnBoundTink?.Invoke(pos);
                //afterPos.y = Mathf.Clamp(afterPos.y, bounds.min.y, bounds.max.y);
            }
        }

        pos += impulse * Time.deltaTime;

        Vector3 brakingVector = impulse.normalized * BREAKING_FACTOR * Time.deltaTime;

        if (impulse.magnitude > brakingVector.magnitude)
            impulse -= brakingVector;
        else
        {
            impulse = Vector3.zero;
        }

        transform.localPosition = pos;

        if(rotationFromSpeedCoefficient == 0)
            return;

        rotation = impulse.magnitude / rotationFromSpeedCoefficient;

        transform.rotation *= Quaternion.Euler(0,0, rotation); 
    }
    private void GravityForce()
    {
        if (transform.localPosition.y > riverZone.bounds.max.y)
            rigidbody2D.AddForce(Vector3.down * (ERROR_FORCE * Time.deltaTime), ForceMode2D.Impulse);
        if (transform.localPosition.y < riverZone.bounds.min.y)
            rigidbody2D.AddForce(Vector3.up * (ERROR_FORCE * Time.deltaTime), ForceMode2D.Impulse);
    }
    
    public void ResetPhysics()
    {
        UpdateBehaviour = box.IsSortMode ? SortUpdate : CachUpdate;
        impulse = Vector3.zero;
        rigidbody2D.linearVelocity = Vector2.zero;
        rigidbody2D.angularVelocity = 0;
        rigidbody2D.totalForce = Vector2.zero;
        rigidbody2D.totalTorque = 0;
    }
    
    public void SetRandomImpulse(float forse, bool randomForse = true)
    {
        forse *= randomForse ? Random.value : 1;
        float randomDirection = Random.value * 360 * Mathf.Deg2Rad;

        rigidbody2D.AddForce(new Vector3(math.cos(randomDirection), math.sin(randomDirection), 0) * forse, ForceMode2D.Impulse);
        
        NewRotation();
    }

    private void NewRotation()
    {
        var rotation = rotationSpeed * Random.value;
        rotation *= FirMath.GameMath.HeadsOrTails() ? 1 : -1;

        rigidbody2D.AddTorque(rotation, ForceMode2D.Impulse);
    }
    
    public void SetSortRandomImpulse(float forse, bool randomForse = true)
    {
        forse *= randomForse ? Random.value : 1;
        float randomDirection = Random.value * 360 * Mathf.Deg2Rad;

        impulse = new Vector3(math.cos(randomDirection), math.sin(randomDirection), 0) * forse;

        NewSortRotation();
    }

    private void NewSortRotation()
    {
        rotation = rotationSpeed * Random.value;
        rotation *= FirMath.GameMath.HeadsOrTails() ? 1 : -1;

        rotationFromSpeedCoefficient = impulse.magnitude / rotation;
    }
    public void SetSortImpulse(Vector3 impulse)
    {
        this.impulse = impulse;
    }
    public void SetImpulse(Vector3 force, bool toZeroPoint = true)
    {
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
    }
    public void SetView(Sprite sprite, Color color)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
        Light2D.color = color;
        
        Gradient gradient = new Gradient();
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
        alphaKeys[0].alpha = 0f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 1f;
        alphaKeys[1].time = 0.25f;
        alphaKeys[2].alpha = 0f;
        alphaKeys[2].time = 1f;
        
        GradientColorKey[] colorKeys = new GradientColorKey[3];
        colorKeys[0].color = Color.white;
        colorKeys[0].time = 0f;
        colorKeys[1].color = color;
        colorKeys[1].time = 0.25f;
        colorKeys[2].color = Color.white;
        colorKeys[2].time = 1f;
        
        gradient.SetKeys(colorKeys, alphaKeys);

        dirt.localScale = Vector3.one;
        trailRenderer.colorGradient = gradient;
    }

    public void ResetTail()
    {
        trailRenderer.Clear();
    }

    public void BoxMove(Vector3 delta)
    {
        rigidbody2D.AddForce(delta * BOX_MOVE_COEFFICIENT, ForceMode2D.Impulse);
        dirt.localScale *= (1 - CLEANING_SPEED * Time.deltaTime);
        if (dirt.localScale.x <= 0.05f)
            dirt.localScale = Vector3.zero;
    }

    public void Freeze()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        box.OnMoveToSort += MoveToSort;
    }

    private void MoveToSort(Vector3 dir)
    {
        transform.localPosition += dir;
    }

    public void Unfreeze()
    {
        Destroy(rigidbody2D);
        Destroy(GetComponent<Collider2D>());
        box.OnMoveToSort -= MoveToSort;
        UpdateBehaviour = SortUpdate;
        SetSortRandomImpulse(ERROR_FORCE);
    }

    public void RemoveDirt()
    {
        if(dirt == null)
            return;
        
        Destroy(dirt.gameObject);
    }
}