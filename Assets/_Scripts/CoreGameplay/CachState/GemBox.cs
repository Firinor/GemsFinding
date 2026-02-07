using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GemBox : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI capacityText;
    private int capacity;

    public float Speed;
    public BoxCollider2D boxZone;
    public BoxCollider2D boxOutZone;
    public event Action<Vector3> OnMove;
    public event Action<Vector3> OnMoveToSort;
    public event Action<Gem> OnСatch;
    public event Action OnFull;

    private HashSet<Gem> gems = new();
    private Camera _camera;

    private float offset = 4;

    public void Initialize(int capacity)
    {
        gems = new HashSet<Gem>(capacity);
        _camera = Camera.main;
        this.capacity = capacity;
    }

    public bool AddGem(Gem gem)
    {
        gems.Add(gem);
        capacityText.text = $"{gems.Count}/{capacity}";
        bool isFull = gems.Count == capacity;
        
        if(isFull)
            OnFull?.Invoke();
        
        return isFull;
    }
    public void RemoveGem(Gem gem)
    {
        gems.Remove(gem);
        capacityText.text = $"{gems.Count}/{capacity}";
    }
    
    public void FreezeGems()
    {
        foreach (var gem in gems)
        {
            gem.Freeze();
        }
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
