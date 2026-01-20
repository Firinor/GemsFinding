using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Bootstrup : MonoBehaviour
{
    [SerializeField] 
    private FindObjectManager mainManager;
    
    void Awake()
    {
        LoadPlayerData();
        mainManager.Initialize(new());
    }

    private void LoadPlayerData()
    {
        
    }
}
