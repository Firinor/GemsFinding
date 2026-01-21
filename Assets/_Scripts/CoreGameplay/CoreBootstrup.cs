using UnityEngine;

[DefaultExecutionOrder(-1)]
public class CoreBootstrup : MonoBehaviour
{
    [SerializeField] 
    private FindObjectManager mainManager;

    public PuzzleContex PuzzleContex;
    
    void Awake()
    {
        LoadPlayerData();
        mainManager.Initialize(PuzzleContex);
    }

    private void LoadPlayerData()
    {
        
    }
}
