using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MetaTreeManager : MonoBehaviour
{
    public int AllProgressCount;
    //public event Action OnNewPointLearned; 
    
    [SerializeField] 
    private List<MetaPointView> points;
    public List<MetaPointView> PointsData => points;
    [SerializeField] 
    private InfoPanel infoPanel;
    private MetaPointView selectedPoint;
    
    private ProgressData player;
    
    [SerializeField] 
    private Sprite LevelFrame;
    [SerializeField] 
    private Sprite MaxFrame;
    [SerializeField] 
    private Image AimFrame;
    
    [SerializeField] 
    private Button UpLevelButton;
    [SerializeField] 
    private Button DeLevelButton;

    [ContextMenu(nameof(CalculateAllProgress))]
    private void CalculateAllProgress()
    {
        AllProgressCount = -1;
        foreach (MetaPointView point in points)
        {
            AllProgressCount += point.Data.MaxLevel;
        }
    }
    
    public void Initialize(ProgressData player)
    {
        this.player = player;
        
        DisableAllPoints();
        EnablePointsBy();
        SubscribeButtons();
        AimFrame.enabled = false;
    }

    private void SubscribeButtons()
    {
        player.OnGoldChange += PointsShowPlus;
        PointsShowPlus(player.GoldCoins);
        
        foreach (MetaPointView point in points)
        {
            PlayerDataMetaPoint playerPoint = player.MetaPoints.FirstOrDefault(p => p.ID == point.Data.ID);
            int level = playerPoint is not null ? playerPoint.Level : 0;
            
            point.Button.onClick.AddListener(() => PointClick(point));
            if (level == 0)
                point.ToDisableFrame();
            else if(level < point.Data.MaxLevel)
                point.ToLevelFrame(LevelFrame);
            else
                point.ToMaxFrame(MaxFrame);
        }
    }

    private void PointsShowPlus(int goldCount)
    {
        foreach (MetaPointView point in points)
        {
            PlayerDataMetaPoint playerPointData = player.MetaPoints.FirstOrDefault(p => p.ID == point.Data.ID);
            int level = playerPointData is not null ? playerPointData.Level : 0;
            var pointData = point.Data;

            if (pointData.Cost.Length > level)
            {
                int pointCost = pointData.Cost[level];
                point.ShowPlus(pointCost <= player.GoldCoins);
            }
            else
            {
                point.ShowPlus(false);
            }
        }
    }

    private void ShowInfo(MetaPointData pointData)
    {
        MetaPointInfo info = new();
        info.Name = pointData.Name;
        info.Discription = pointData.Discription;

        PlayerDataMetaPoint playerPointData = player.MetaPoints.FirstOrDefault(p => p.ID == pointData.ID);

        info.Level = "0";
        int level = 0;
        if (playerPointData is not null)
        {
            level = playerPointData.Level;
            info.Level = level.ToString();
        }
        
        info.MaxLevel = pointData.MaxLevel.ToString();
        
        info.Effect = pointData.Type switch
        {
            MetaPointType.RecipeCount => player.Stats.RecipeGemCount.ToString(),
            MetaPointType.InRiverGemsCount => player.Stats.InRiverGemCount.ToString(),
            MetaPointType.InBoxGemsCount => player.Stats.InBoxGemCount.ToString(),
            
            MetaPointType.GemShapeCount => player.Stats.ShapeCount.ToString(),
            MetaPointType.GemColorCount => player.Stats.ColorCount.ToString(),
            MetaPointType.GemSpoilCount => player.Stats.SpoilCount.ToString(),
            MetaPointType.GemDuoColorCount => player.Stats.DuoColorCount.ToString(),
            MetaPointType.GemDuoShapeCount => player.Stats.DuoShapeCount.ToString(),
            
            MetaPointType.NoGem => player.Stats.EmptyDirt.ToString(format: "F0"),
            MetaPointType.NoDirt => player.Stats.NoDirt.ToString(),
            MetaPointType.Light2D => player.Stats.WithLight2D.ToString(),
            MetaPointType.Tail => player.Stats.WithTail.ToString(),
            MetaPointType.NoBlinks => player.Stats.NoBlink.ToString(),
            
            MetaPointType.Invisible => player.Stats.InvisibleCount.ToString(),
            MetaPointType.Moveble => player.Stats.MovebleCount.ToString(),
            MetaPointType.Jumpble => player.Stats.JumpbleCount.ToString(),
            MetaPointType.ChangingColor => player.Stats.ChangingColor.ToString(),
            
            MetaPointType.Light => player.Stats.LightRadius.ToString(),
            
            _ => throw new ArgumentOutOfRangeException()
        };

        info.NextEffect = (Int32.Parse(info.Effect) + pointData.Value).ToString();
        
        if(level < pointData.MaxLevel)
            info.Cost = pointData.Cost[Int32.Parse(info.Level)].ToString();

        SetUpButtonsSubscription(pointData);
        RefreshInfoView();
        infoPanel.Show(info);
    }

    private void SetUpButtonsSubscription(MetaPointData pointData)
    {
        PlayerDataMetaPoint playerPointData = player.MetaPoints.FirstOrDefault(p => p.ID == pointData.ID);
        
        int level = 0;
        if (playerPointData is not null)
        {
            level = playerPointData.Level;
        }
        
        UpLevelButton.interactable = level < pointData.MaxLevel;
        DeLevelButton.interactable = level > 0;
        
        UpLevelButton.onClick.RemoveAllListeners();
        DeLevelButton.onClick.RemoveAllListeners();
        
        UpLevelButton.onClick.AddListener(() => AddPointLevel(pointData));
        DeLevelButton.onClick.AddListener(() => RemovePointLevel(pointData));
    }
    
    private void PointClick(MetaPointView point)
    {
        selectedPoint = point;
        AimFrame.enabled = true;
        AimFrame.transform.localPosition = point.transform.localPosition;
        ShowInfo(point.Data);
    }

    private void AddPointLevel(MetaPointData point)
    {
        PlayerDataMetaPoint playerPoint = player.MetaPoints.FirstOrDefault(p => p.ID == point.ID);
        int level = playerPoint is not null ? playerPoint.Level : 0;
        int pointCost = point.Cost[level];
        if(!player.TrySpendGold(pointCost))
        {
            //Audio.Error;
            return;
        }
        
        if (playerPoint is not null)
        {
            playerPoint.Level++;
        }
        else
        {
            player.MetaPoints.Add(new()
            {
                ID = point.ID,
                Level = 1
            });
        }
        level++;
        
        SaveLoadSystem<ProgressData>.Save(player);
        
        foreach (MetaPointData unlock in point.Unlocks)
        {
            MetaPointView unlockPoint = points.First(p => p.Data.ID == unlock.ID);
            unlockPoint.gameObject.SetActive(true);
            unlockPoint.Initialize();
        }
        
        player.InitializeStats(PointsData.Select(p => p.Data));
        //OnNewPointLearned?.Invoke();
        
        ShowInfo(point);
    }
    private void RemovePointLevel(MetaPointData point)
    {
        PlayerDataMetaPoint playerPoint = player.MetaPoints.FirstOrDefault(p => p.ID == point.ID);
        int level = playerPoint.Level;
        if(level <= 0)
            return;
        
        int pointCost = point.Cost[level-1];

        player.AddGold(pointCost);
        playerPoint.Level--;
        
        SaveLoadSystem<ProgressData>.Save(player);

        player.InitializeStats(PointsData.Select(p => p.Data));
        
        ShowInfo(point);
    }

    private void RefreshInfoView()
    {
        PlayerDataMetaPoint playerPoint = player.MetaPoints.FirstOrDefault(p => p.ID == selectedPoint.Data.ID);
        int level = 0;
        if(playerPoint is not null)
            level = playerPoint.Level;
        
        UpLevelButton.interactable = level < selectedPoint.Data.MaxLevel;
        DeLevelButton.interactable = level > 0;
        
        if (level >= selectedPoint.Data.MaxLevel)
        {
            selectedPoint.ToMaxFrame(MaxFrame);
        }
        else if (level == 0)
        {
            selectedPoint.ToDisableFrame();
            selectedPoint.ShowPlus(selectedPoint.Data.Cost[level] <= player.GoldCoins);
        }
        else
        {
            selectedPoint.ToLevelFrame(LevelFrame);
            selectedPoint.ShowPlus(selectedPoint.Data.Cost[level] <= player.GoldCoins);
        }
    }

    private void EnablePointsBy()
    {
        points[0].Initialize();
        
        foreach (PlayerDataMetaPoint point in player.MetaPoints)
        {
            MetaPointView treePoint = points.First(p => p.Data.ID == point.ID);
            
            foreach (MetaPointData unlock in treePoint.Data.Unlocks)
            {
                MetaPointView unlockPoint = points.First(p => p.Data.ID == unlock.ID);
                unlockPoint.gameObject.SetActive(true);
                unlockPoint.Initialize();
            }
        }
    }

    private void DisableAllPoints()
    {
        foreach (var point in points)
        {
            point.gameObject.SetActive(false);
        }
    }    
    
    public void HideInfo()
    {
        infoPanel.Hide();
    }

    private void OnDestroy()
    {
        player.OnGoldChange -= PointsShowPlus;
        foreach (var point in points)
        {
            point.Button.onClick.RemoveAllListeners();
        }
    }
}