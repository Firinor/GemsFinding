using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Mathematics;

public class Gem : MonoBehaviour
{
    public static Transform centerZone;

    public Action<Vector3> OnBoundTink;
    
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer Sprite => spriteRenderer;
    
    private const float EDGE_RADIUS = 3;
    private const float BREAKING_FACTOR = 2;
    private const int ERROR_FORCE = 8;

    private Vector3 impulse;
    private float rotation;
    private float rotationFromSpeedCoefficient;

    private bool InCircle;

    private void Update()
    {
        InCircle = Vector3.Distance(transform.position, centerZone.position) <= EDGE_RADIUS;
        if (!InCircle)
            GravityForce();
        
        if (impulse != Vector3.zero)
            ForceToIngredient();
    }

    private void GravityForce()
    {
        Vector3 direction = (centerZone.position - transform.position).normalized;
        impulse += direction * 10 * Time.deltaTime;
    }

    private void ForceToIngredient()
    {
        Vector3 pos = transform.position;

        pos += impulse * Time.deltaTime;

        Vector3 toEdge = pos - centerZone.position;
        if (InCircle && toEdge.magnitude > EDGE_RADIUS)
        {
            Vector3 normal = toEdge.normalized;
            impulse = Vector3.Reflect(impulse, -normal);
            pos = centerZone.position + normal * EDGE_RADIUS;
            OnBoundTink?.Invoke(pos);
        }

        Vector3 brakingVector = impulse.normalized * BREAKING_FACTOR * Time.deltaTime;

        if (impulse.magnitude > brakingVector.magnitude)
            impulse -= brakingVector;
        else
        {
            impulse = Vector3.zero;
        }

        transform.position = pos;

        /*if(rotationFromSpeedCoefficient == 0)
            return;

        rotation = impulse.magnitude / rotationFromSpeedCoefficient;

        transform.rotation *= Quaternion.Euler(0,0, rotation); */
    }
    
    public void SetRandomImpulse(float forse, bool randomForse = true)
    {
        forse *= randomForse ? Random.value : 1;
        float randomDirection = Random.value * 360 * Mathf.Deg2Rad;

        impulse = new Vector3(math.cos(randomDirection), 0, math.sin(randomDirection)) * forse;

        //NewSortRotation();
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