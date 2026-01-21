using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MetaBootstrup : MonoBehaviour
{
    [SerializeField] 
    private MetaTreeManager MetaTree;

    public MetaContex MetaContex;
    
    void Awake()
    {
        LoadPlayerData();
        MetaTree.Initialize(MetaContex);
    }

    private void LoadPlayerData()
    {
        
    }
}