using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    public CameraPosition CachPosition;
    public CameraPosition SortPosition;

    public float SwitchTime;

    private CameraPosition targetPosition;

    private Vector3 deltaPosition;
    private float deltaSize;
    
    private Coroutine coroutine;
    
    private void Awake()
    {
        _camera.orthographicSize = CachPosition.Size;
        _camera.transform.position = CachPosition.Position;
        
        deltaPosition = (SortPosition.Position - CachPosition.Position) / SwitchTime;
        deltaSize = (SortPosition.Size - CachPosition.Size)  / SwitchTime;
    }

    [ContextMenu(nameof(ToCach))]
    public void ToCach()
    {
#if UNITY_EDITOR
        _camera.transform.position += CachPosition.Position;
        _camera.orthographicSize += CachPosition.Size;
#else
        targetPosition = CachPosition;
        coroutine = StartCoroutine(MoveCamera());
#endif
    }

    [ContextMenu(nameof(ToSort))]
    public void ToSort()
    {
#if UNITY_EDITOR
        _camera.transform.position += SortPosition.Position;
        _camera.orthographicSize += SortPosition.Size;
#else
        targetPosition = SortPosition;
        coroutine = StartCoroutine(MoveCamera());
#endif
    }

    private IEnumerator MoveCamera()
    {
        float sizeEdge = deltaSize * Time.deltaTime;
        while(Mathf.Abs(_camera.orthographicSize - targetPosition.Size) > Mathf.Abs(sizeEdge))
        {
            _camera.transform.position += deltaPosition * Time.deltaTime;
            sizeEdge = deltaSize * Time.deltaTime;
            _camera.orthographicSize += sizeEdge;
            yield return null;
        }
    }
}

[Serializable]
public struct CameraPosition
{
    public float Size;
    public Vector3 Position;
}