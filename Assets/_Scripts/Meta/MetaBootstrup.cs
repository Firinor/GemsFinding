using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class MetaBootstrup : MonoBehaviour
{
    [SerializeField] 
    private Settings settings;
    
    [SerializeField] 
    private MetaTreeManager MetaTree;
    
    [SerializeField] 
    private RectTransform playerGoldRect;
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
    
    [SerializeField] 
    private Cheats cheats;
    
    private ProgressData player;
    
    void Awake()
    {
        settings.Initialize();
        
        LoadPlayerData();
        SubscribeToGold();
        SubscribeToProgress();
        MetaTree.Initialize(player);
        cheats.Initialize(player);
    }

    private void SubscribeToProgress()
    {
        progressSlider.maxValue = MetaTree.AllProgressCount;
        progressSlider.value = player.GetPointsLevel();
        //MetaTree.OnNewPointLearned += SetProgressBar;
        SetProgressBar();
    }

    private void SetProgressBar()
    {
        progressSlider.value = player.GetPointsLevel();
        textSlider.text = $"{(int)(progressSlider.value/progressSlider.maxValue * 100)}%";
        if(progressSlider.value >= progressSlider.maxValue)
            endButton.onClick.AddListener(() =>
            {
                endScreen.SetActive(true);
            });
        
        player.InitializeStats(MetaTree.PointsData.Select(p => p.Data));
    }

    private void SubscribeToGold()
    {
        player.OnGoldChange += GoldText;
        GoldText(player.GoldCoins);
    }

    private void GoldText(int count)
    {
        playerGold.text = count.ToString();
        float imageBorder = 160;
        playerGoldRect.sizeDelta = new Vector2(playerGold.preferredWidth + imageBorder, playerGoldRect.sizeDelta.y);
    }

    private void LoadPlayerData()
    {
        player = SaveLoadSystem<ProgressData>.Load(Default: new());
        player.InitializeStats(MetaTree.PointsData.Select(p => p.Data));
    }
    private void LoadSettingsData()
    {
        throw new System.NotImplementedException();
    }

    private void OnDestroy()
    {
        endButton.onClick.RemoveAllListeners();
        //MetaTree.OnNewPointLearned -= SetProgressBar;
        player.OnGoldChange -= GoldText;
    }
}