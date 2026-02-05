using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GemBox : MonoBehaviour
{
    private int capacity;

    public float Speed;
    public BoxCollider2D boxZone;
    public BoxCollider2D boxOutZone;
    public event Action<Vector3> OnMove;
    public event Action<Gem> OnСatch;

    private List<Gem> gems = new();
    private Camera _camera;

    private float offset = 4;

    private void Awake()
    {
        _camera = Camera.main;
        capacity = 50;
    }

    public void Initialize(int capacity)
    {
        _camera = Camera.main;
        this.capacity = capacity;
    }
    
    void Update()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPoint = _camera.ScreenToWorldPoint(mousePosition);
        worldPoint.z = 0;
        worldPoint.x += offset;

        Vector3 delta = (worldPoint - transform.position) * Time.deltaTime;

        transform.position += delta;
        
        OnMove?.Invoke(delta);
    }
}
