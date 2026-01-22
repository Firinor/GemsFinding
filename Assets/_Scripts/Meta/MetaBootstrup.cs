using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MetaBootstrup : MonoBehaviour
{
    [SerializeField] 
    private MetaTreeManager MetaTree;

    public MetaContext MetaContext;
    
    void Awake()
    {
        MetaContext = new();
        
        LoadPlayerData();
        MetaTree.Initialize(MetaContext);
    }

    private void LoadPlayerData()
    {
        MetaContext.Player = SaveLoadSystem<ProgressData>.Load(Default: new());
    }
}