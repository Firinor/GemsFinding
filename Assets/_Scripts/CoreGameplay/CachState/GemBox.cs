using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GemBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private GemPool pool;
    [SerializeField] private BoxCacher cacher;
    private int capacity;
    private int limit;
    public int Limit => limit;
    [SerializeField] private GameObject[] Borders;
    public float Speed;
    public BoxCollider2D boxZone;
    public BoxCollider2D boxOutZone;
    public event Action<Vector3> OnMove;
    public event Action<Vector3> OnMoveToSort;
    public event Action<Gem> OnСatch;
    public event Action OnFull;

    private HashSet<Gem> gems = new();
    private List<Gem> riverGems = new();
    private Camera _camera;
    public Vector3 SortPosition;
    public bool IsSortMode;

    private float offset = 4;

    public void Initialize(int capacity, int limit)
    {
        gems = new HashSet<Gem>(capacity);
        _camera = Camera.main;
        this.capacity = capacity;
        this.limit = limit;
        capacityText.gameObject.SetActive(true);
        capacityText.text = $"{gems.Count}/{capacity}";
        foreach (var go in Borders)
        {
            go.SetActive(true);
        }

        IsSortMode = false;
        enabled = true;
    }

    public bool AddGem(Gem gem)
    {
        if (gems.Count >= capacity)
            return true;

        gems.Add(gem);
        limit--;
        capacityText.text = $"{gems.Count}/{capacity}";
        bool isFull = gems.Count == capacity
                      || limit == 0;

        if (isFull)
        {
            OnFull?.Invoke();
        }
        
        return isFull;
    }
    public void RemoveGem(Gem gem)
    {
        gems.Remove(gem);
        limit++;
        capacityText.text = $"{gems.Count}/{capacity}";
    }

    public bool ToSotrMode()
    {
        if (gems.Count <= 0)
            return false;
        
        IsSortMode = true;
        cacher.enabled = false;
        
        enabled = false;
        foreach (var go in Borders)
        {
            go.SetActive(false);
        }
        riverGems = pool.GetAllActiveGems();
        foreach (var gem in gems)
        {
            riverGems.Remove(gem);
            gem.Freeze();
        }
        foreach (var gem in riverGems)
        {
            pool.Return(gem);
        }
        return true;
    }
    public void ToCachMode()
    {
        foreach (var gem in gems)
        {
            pool.Return(gem);
        }
        gems = new HashSet<Gem>(capacity);
        OnMove = null;
        enabled = true;
        foreach (var go in Borders)
        {
            go.SetActive(true);
        }

        foreach (var gem in riverGems)
        {
            gem.gameObject.SetActive(true);
        }
        
        IsSortMode = false;
        cacher.enabled = true;
        capacityText.gameObject.SetActive(true);
        capacityText.text = $"{gems.Count}/{capacity}";
    }
    
    public IEnumerator MoveToSortPoint(Action onComplete)
    {
        if (limit == 0)
            capacityText.text = "EMPTY";
        else if (gems.Count == capacity)
            capacityText.text = "FULL";
        
        while (true)
        {
            float speed = 2;
            Vector3 delta = (SortPosition - transform.position) * Time.deltaTime * speed;
            transform.position += delta;
            OnMoveToSort.Invoke(delta);
            yield return null;
            if(0.05f < Vector3.Distance(SortPosition, transform.position))
                continue;

            transform.position = SortPosition;
            break;
        }
        foreach (var gem in gems)
        {
            gem.Unfreeze();
        }
        if(limit > 0)
            capacityText.gameObject.SetActive(false);
        onComplete.Invoke();
    }
    
    void Update()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPoint = _camera.ScreenToWorldPoint(mousePosition);
        worldPoint.z = 0;
        worldPoint.x += offset;

        Vector3 delta = (worldPoint - transform.position) * Speed * Time.deltaTime;

        transform.position += delta;
        
        OnMove?.Invoke(delta);
    }
}
