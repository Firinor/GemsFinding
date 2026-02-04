using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine.Rendering.Universal;

public class Gem : MonoBehaviour
{
    public static GemBox box;
    public static BoxCollider2D riverZone;
    private static Bounds Bounds => box.boxZone.bounds;

    public event Action<Vector3> OnBoundTink;
    public event Action<Gem> OnEdge;
    
    [SerializeField]
    private float mass;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField] 
    private float brakingBoxFactor;
    [SerializeField] 
    private float brakingRiverFactor;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private TrailRenderer trailRenderer;
    [SerializeField]
    private Light2D Light2D;
    [SerializeField]
    private RectTransform dirt;
    public SpriteRenderer Sprite => spriteRenderer;
    [SerializeField]
    private int id;
    
    private const int ERROR_FORCE = 8;
    private const int RIVER_FORCE = 3;
    private const float X_EDGE = -50;
    private const float BOX_MOVE_COEFFICIENT = 1f;

    private Vector3 impulse;
    private float rotation;
    private float rotationFromSpeedCoefficient;
    private bool isInBox;

    void FixedUpdate()
    {
        if (!Bounds.Contains(transform.localPosition))
            GravityForce();
        
        ForceToIngredient();
    }

    private void GravityForce()
    {
        if (!isInBox)
            return;
        
        if (transform.localPosition.y > riverZone.bounds.max.y)
            impulse += Vector3.down * (ERROR_FORCE * Time.fixedDeltaTime);
        if (transform.localPosition.y < riverZone.bounds.min.y)
            impulse += Vector3.up * (ERROR_FORCE * Time.fixedDeltaTime);
    }

    public void ResetPhysics()
    {
        impulse = Vector3.zero;
    }

    private void ForceToIngredient()
    {
        Vector3 pos = transform.localPosition;

        if (Bounds.Contains(pos))
        {
            if (!isInBox)
            {
                isInBox = true;
                box.OnMove += BoxMove;
            }
            
            Vector3 afterPos = pos + impulse * Time.fixedDeltaTime;
            if (afterPos.x < Bounds.min.x)
            {
                impulse.x = -impulse.x;
                NewRotation();
                OnBoundTink?.Invoke(pos);
                //afterPos.x = Mathf.Clamp(afterPos.x, bounds.min.x, bounds.max.x);
            }

            if (afterPos.y < Bounds.min.y || afterPos.y > Bounds.max.y)
            {
                impulse.y = -impulse.y;
                NewRotation();
                OnBoundTink?.Invoke(pos);
                //afterPos.y = Mathf.Clamp(afterPos.y, bounds.min.y, bounds.max.y);
            }
        }
        else
        {
            if (isInBox)
            {
                isInBox = false;
                box.OnMove -= BoxMove;
            }
            impulse += Vector3.left * (RIVER_FORCE * Time.fixedDeltaTime);
        }

        pos += impulse * Time.fixedDeltaTime;

        Vector3 brakingVector;

        if (Bounds.Contains(transform.localPosition))
        {
            brakingVector = impulse.normalized * brakingBoxFactor * Time.fixedDeltaTime;
            if (impulse.magnitude > brakingVector.magnitude)
                impulse -= brakingVector;
            else
            {
                impulse = Vector3.zero;
                //enabled = false;
            }
        }
        else
        {
            brakingVector = impulse.normalized * brakingRiverFactor * Time.fixedDeltaTime;
            if (impulse.magnitude > brakingVector.magnitude)
                impulse -= brakingVector;
            else
            {
                impulse = Vector3.zero;
                //enabled = false;
            }
        }

        transform.localPosition = pos;

        if (transform.localPosition.x < X_EDGE)
        {
            OnEdge?.Invoke(this);
            return;
        }
        
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
        
        trailRenderer.colorGradient = gradient;
    }

    public void ResetTail()
    {
        trailRenderer.Clear();

    }

    private void BoxMove(Vector3 delta)
    {
        transform.localPosition += delta * BOX_MOVE_COEFFICIENT;
    }
    
    private void OnDestroy()
    {
        box.OnMove -= BoxMove;
    }
}