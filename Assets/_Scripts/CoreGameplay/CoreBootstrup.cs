using System.Linq;
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
    
    void Awake()
    {
        settings.Initialize();
        LoadPlayerData(out ProgressData player);
        mainManager.Initialize(player);
    }

    private void LoadPlayerData(out ProgressData data)
    {
        MetaPointData[] pointsData = Resources.LoadAll("Points", typeof(MetaPointData)).Cast<MetaPointData>().ToArray();
        data = SaveLoadSystem<ProgressData>.Load(Default: new());
        if(!isDebugMode)
            data.InitializeStats(pointsData);
        else
            data.Stats = stats;
    }
}
