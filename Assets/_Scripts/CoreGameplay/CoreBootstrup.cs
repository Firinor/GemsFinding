using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class CoreBootstrup : MonoBehaviour
{
    [SerializeField] 
    private FindObjectManager mainManager;

    private ProgressData player;
    
    void Awake()
    {
        LoadPlayerData();
        mainManager.Initialize(player.Stats);
    }

    private void LoadPlayerData()
    {
        MetaPointData[] pointsData = Resources.LoadAll("Points", typeof(MetaPointData)).Cast<MetaPointData>().ToArray();
        player = SaveLoadSystem<ProgressData>.Load(Default: new());
        player.InitializeStats(pointsData);
    }
}
