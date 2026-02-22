using UnityEngine;

[DefaultExecutionOrder(-1)]
public class CoreBootstrup : MonoBehaviour
{
    [SerializeField] 
    private Settings settings;
    [SerializeField] 
    private FindObjectManager mainManager;

    [SerializeField]
    private bool isDebugMode;
    [SerializeField]
    private Stats stats;
    
    public void StartPuzzle()
    {
        settings.Initialize();
        LoadPlayerData(out ProgressData player);
        mainManager.Initialize(player);
    }

    private void LoadPlayerData(out ProgressData data)
    {
#if UNITY_EDITOR
        if (isDebugMode)
        {
            data = new()
            {
                Stats = stats
            };
            return;
        }
#endif
        data = SaveLoadSystem<ProgressData>.Load(Default: new());
    }
}
