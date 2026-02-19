using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine.Rendering.Universal;

public class Gem : MonoBehaviour
{
    public static BoxCollider2D riverZone;

    public Action<Vector3> OnBoundTink;
    
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer Sprite => spriteRenderer;
    
    private const float BREAKING_FACTOR = 3;
    private const int ERROR_FORCE = 8;

    private Vector3 impulse;
    private float rotation;
    private float rotationFromSpeedCoefficient;

    private Action UpdateBehaviour;

    private void Update()
    {
        UpdateBehaviour?.Invoke();
    }

    private void GravitySortForce()
    {
        //impulse += 10 * Time.deltaTime;
    }

    private void ForceToIngredient()
    {
        Vector3 pos = transform.localPosition;

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

    public void SetView(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
    
    private void MoveToSort(Vector3 dir)
    {
        transform.localPosition += dir;
    }
}