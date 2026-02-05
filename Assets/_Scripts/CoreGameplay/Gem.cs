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
    private static Bounds OutBounds => box.boxOutZone.bounds;

    public Action<Vector3> OnBoundTink;
    public Action<Gem> OnEdge;
    
    [SerializeField]
    private float mass;
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
    private int id;
    
    
    private const float BrakingBoxFactor = 4;
    private const float BrakingRiverFactor = 1;
    
    private const int ERROR_FORCE = 8;
    private const int RIVER_FORCE = 3;
    private const float X_EDGE = -50;
    private const float BOX_MOVE_COEFFICIENT = .9f;
    private const float BOX_BORDER_COEFFICIENT = 90;

    private Vector3 impulse;
    private float rotation;
    private float rotationFromSpeedCoefficient;
    private Action updateBehaviour;
    
    void Update()
    {
        updateBehaviour.Invoke();
        
        transform.localPosition += impulse * Time.deltaTime;
        
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

    public void ResetPhysics()
    {
        box.OnMove -= BoxBoarding;
        box.OnMove -= BoxMove;
        updateBehaviour = RiverBehaviour;
        box.OnMove += BoxBoarding;
        impulse = Vector3.zero;
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
    
#region Behaviour
    private void BoxMove(Vector3 delta)
    {
        Vector3 pos = transform.localPosition;
        Vector3 afterPos = pos + delta * BOX_MOVE_COEFFICIENT;
        
        if (afterPos.x < Bounds.min.x)
        {
            afterPos.x = Bounds.min.x;
            afterPos += delta / BOX_MOVE_COEFFICIENT;
            impulse += delta*BOX_BORDER_COEFFICIENT*(1+Random.value);
            OnBoundTink?.Invoke(afterPos);
        }
        if (afterPos.y < Bounds.min.y || afterPos.y > Bounds.max.y)
        {
            afterPos.y = math.clamp(afterPos.y, Bounds.min.y, Bounds.max.y);
            afterPos += delta / BOX_MOVE_COEFFICIENT;
            impulse += delta*BOX_BORDER_COEFFICIENT*(1+Random.value);
            OnBoundTink?.Invoke(afterPos);
        }

        transform.localPosition = afterPos;
    }
    private void BoxBoarding(Vector3 delta)
    {
        Vector3 pos = transform.localPosition;

        if (!OutBounds.Contains(pos))
            return;

        Vector3 afterPos = pos + delta*(1+Random.value);

        if (afterPos.x > OutBounds.max.x)
        {
            transform.localPosition = afterPos;
            return;
        }
        
        /*afterPos.x = Math.Min(afterPos.x, OutBounds.min.x);
        if(afterPos.y < OutBounds.center.y)
            afterPos.y = Math.Min(afterPos.y, OutBounds.min.y);
        else
            afterPos.y = Math.Max(afterPos.y, OutBounds.max.y);*/

        impulse += delta * BOX_BORDER_COEFFICIENT;//*(1+Random.value);
        OnBoundTink?.Invoke(afterPos);

        transform.localPosition = afterPos;
    }
    
    private void BoxBehaviour()
    {
        Vector3 pos = transform.localPosition;
        Vector3 afterPos = pos + impulse * Time.deltaTime;
        
        if (afterPos.x < Bounds.min.x)
        {
            impulse.x = -impulse.x;
            NewRotation();
            OnBoundTink?.Invoke(pos);
        }

        if (afterPos.y < Bounds.min.y || afterPos.y > Bounds.max.y)
        {
            impulse.y = -impulse.y;
            NewRotation();
            OnBoundTink?.Invoke(pos);
        }

        pos += impulse * Time.deltaTime;

        Vector3 brakingVector;

        brakingVector = impulse.normalized * BrakingBoxFactor * Time.deltaTime;
        if (impulse.magnitude > brakingVector.magnitude)
            impulse -= brakingVector;
        else
        {
            impulse = Vector3.zero;
        }

        transform.localPosition = pos;
            
        if (!Bounds.Contains(transform.localPosition))
        {
            box.OnMove -= BoxMove;
            box.OnMove += BoxBoarding;
            updateBehaviour = RiverBehaviour;
        }
        
        if(rotationFromSpeedCoefficient == 0)
            return;
            
        rotation = impulse.magnitude / rotationFromSpeedCoefficient;
            
        transform.rotation *= Quaternion.Euler(0,0, rotation); 
    }

    private void RiverBehaviour()
    {
        GravityForce();

        Vector3 pos = transform.localPosition;

        impulse += Vector3.left * (RIVER_FORCE * Time.deltaTime);

        Vector3 afterPos = pos + impulse * Time.deltaTime;

        Vector3 brakingVector = impulse.normalized * BrakingRiverFactor * Time.deltaTime;
        if (impulse.magnitude > brakingVector.magnitude)
            impulse -= brakingVector;
        else
        {
            impulse = Vector3.zero;
            //enabled = false;
        }

        if (Bounds.Contains(afterPos))
        {
            box.OnMove -= BoxBoarding;
            box.OnMove += BoxMove;
            updateBehaviour = BoxBehaviour;
            return;
        }
        
        if (pos.x < OutBounds.max.x 
                 && OutBounds.Contains(afterPos))
        {
            if (afterPos.y > OutBounds.min.y || afterPos.y < OutBounds.max.y)
            {
                impulse.y = -impulse.y;
                NewRotation();
                OnBoundTink?.Invoke(afterPos);
            }

            if (afterPos.x > OutBounds.min.x)
            {
                impulse.x = -impulse.x;
                NewRotation();
                OnBoundTink?.Invoke(afterPos);
            }
            
            afterPos = pos + impulse * Time.deltaTime; 
        }
    }
    
    private void GravityForce()
    {
        if (transform.localPosition.y > riverZone.bounds.max.y)
            impulse += Vector3.down * (ERROR_FORCE * Time.deltaTime);
        if (transform.localPosition.y < riverZone.bounds.min.y)
            impulse += Vector3.up * (ERROR_FORCE * Time.deltaTime);
    }
#endregion

    private void OnDestroy()
    {
        box.OnMove -= BoxMove;
    }
}