using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine.Rendering.Universal;

public class Gem : MonoBehaviour
{
    public static Bounds Bounds;

    public event Action<Vector3> OnBoundTink;
    
    [SerializeField]
    private float mass;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField] 
    private float brakingFactor;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private TrailRenderer trailRenderer;
    [SerializeField]
    private Light2D Light2D;
    public SpriteRenderer Sprite => spriteRenderer;
    [SerializeField]
    private int id;
    
    private const int ERROR_FORCE = 10;

    private Vector3 impulse;
    private float rotation;
    private float rotationFromSpeedCoefficient;

    void FixedUpdate()
    {
        if (!Bounds.Contains(transform.localPosition))
            GravityForce();
        
        if (impulse != Vector3.zero)
            ForceToIngredient();
    }

    private void GravityForce()
    {
        impulse += (Bounds.center - transform.localPosition).normalized * (Physics2D.gravity.magnitude * Time.fixedDeltaTime);
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
            Vector3 afterPos = pos + impulse * Time.fixedDeltaTime;
            if (afterPos.x < Bounds.min.x || afterPos.x > Bounds.max.x)
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

        pos += impulse * Time.fixedDeltaTime;

        Vector3 brakingVector = impulse.normalized * brakingFactor * Time.fixedDeltaTime;

        if (impulse.magnitude > brakingVector.magnitude)
            impulse -= brakingVector;
        else
        {
            impulse = Vector3.zero;
            enabled = false;
        }

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
}