using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class MetaBootstrup : MonoBehaviour
{
    [SerializeField] 
    private MetaTreeManager MetaTree;
    
    [SerializeField] 
    private TextMeshProUGUI playerGold;
    
    [SerializeField] 
    private Slider progressSlider; 
    [SerializeField] 
    private TextMeshProUGUI textSlider;
    [SerializeField] 
    private Button endButton;
    [SerializeField] 
    private GameObject endScreen;

#if UNITY_EDITOR
    [SerializeField] 
    private Cheats cheats;
#endif

    private MetaContext MetaContext;
    
    void Awake()
    {
        MetaContext = new();
        
        LoadPlayerData();
        SubscribeToGold();
        SubscribeToProgress();
        MetaTree.Initialize(MetaContext);
#if UNITY_EDITOR
        cheats.Initialize(MetaContext);
#endif
    }

    private void SubscribeToProgress()
    {
        progressSlider.maxValue = MetaTree.AllProgressCount;
        progressSlider.value = MetaContext.Player.GetPointsLevel();
        MetaTree.OnNewPointLearned += SetProgressBar;
        SetProgressBar();
    }

    private void SetProgressBar()
    {
        progressSlider.value++;
        textSlider.text = $"{(int)(progressSlider.value/progressSlider.maxValue * 100)}%";
        if(Mathf.Approximately(progressSlider.value, progressSlider.maxValue))
            endButton.onClick.AddListener(() =>
            {
                endScreen.SetActive(true);
            });
        
        MetaContext.Player.InitializeStats(MetaTree.PointsData.Select(p => p.Data));
    }

    private void SubscribeToGold()
    {
        MetaContext.Player.OnGoldChange += GoldText;
        GoldText(MetaContext.Player.GoldCoins);
    }

    private void GoldText(int count)
    {
        playerGold.text = count.ToString();
    }

    private void LoadPlayerData()
    {
        MetaContext.Player = SaveLoadSystem<ProgressData>.Load(Default: new());
        MetaContext.Player.InitializeStats(MetaTree.PointsData.Select(p => p.Data));
    }

    private void OnDestroy()
    {
        endButton.onClick.RemoveAllListeners();
        MetaTree.OnNewPointLearned -= SetProgressBar;
        MetaContext.Player.OnGoldChange -= GoldText;
    }
}