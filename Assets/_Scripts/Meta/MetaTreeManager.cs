using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MetaTreeManager : MonoBehaviour
{
    public int AllProgressCount;
    public event Action OnNewPointLearned; 
    
    [SerializeField] 
    private List<MetaPointView> points;
    public List<MetaPointView> PointsData => points;
    [SerializeField] 
    private InfoPanel infoPanel;
    
    private ProgressData player;

    [ContextMenu(nameof(CalculateAllProgress))]
    private void CalculateAllProgress()
    {
        AllProgressCount = -1;
        foreach (MetaPointView point in points)
        {
            AllProgressCount += point.Data.MaxLevel;
        }
    }
    
    public void Initialize(MetaContext metaContex)
    {
        player = metaContex.Player;
        
        DisableAllPoints();
        EnablePointsBy();
        SubscribeButtons();
    }

    private void SubscribeButtons()
    {
        player.OnGoldChange += PointsShowPlus;
        PointsShowPlus(player.GoldCoins);
        
        foreach (MetaPointView point in points)
        {
            PlayerDataMetaPoint playerPoint = player.MetaPoints.FirstOrDefault(p => p.ID == point.Data.ID);
            int level = playerPoint is not null ? playerPoint.Level : 0;
            
            if (level == 0)
                point.Button.onClick.AddListener(() => PointClick(point));
            else if(level < point.Data.MaxLevel)
            {
                point.Button.onClick.AddListener(() => PointClick(point));
                point.ToLevelFrame();
            }
            else
                point.ToMaxFrame();
            
            point.OnPointerEnterAction += ShowInfo;
            point.OnPointerExitAction += HideInfo;
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
        
        infoPanel.Show(info);
    }

    private void HideInfo()
    {
        infoPanel.Hide();
    }

    private void PointClick(MetaPointView point)
    {
        PlayerDataMetaPoint playerPoint = player.MetaPoints.FirstOrDefault(p => p.ID == point.Data.ID);
        int level = playerPoint is not null ? playerPoint.Level : 0;

        int pointCost = point.Data.Cost[level];
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
                ID = point.Data.ID,
                Level = 1
            });
        }
        level++;
        
        SaveLoadSystem<ProgressData>.Save(player);

        if (level >= point.Data.MaxLevel)
        {
            point.ToMaxFrame();
            point.Button.onClick.RemoveAllListeners();
        }
        else
        {
            point.ToLevelFrame();
            point.ShowPlus(point.Data.Cost[level] <= player.GoldCoins);
        }
        
        foreach (MetaPointData unlock in point.Data.Unlocks)
        {
            MetaPointView unlockPoint = points.First(p => p.Data.ID == unlock.ID);
            unlockPoint.gameObject.SetActive(true);
            unlockPoint.Initialize();
        }
        
        OnNewPointLearned?.Invoke();
        
        ShowInfo(point.Data);
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

    private void OnDestroy()
    {
        player.OnGoldChange -= PointsShowPlus;
        foreach (var point in points)
        {
            point.OnPointerEnterAction -= ShowInfo;
            point.OnPointerExitAction -= HideInfo;
            point.Button.onClick.RemoveAllListeners();
        }
    }
}