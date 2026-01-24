using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MetaTreeManager : MonoBehaviour
{
    [SerializeField] 
    private List<MetaPointView> points;
    [SerializeField] 
    private InfoPanel infoPanel;
    
    private ProgressData player;
    
    public void Initialize(MetaContext metaContex)
    {
        player = metaContex.Player;
        
        DisableAllPoints();
        EnablePointsBy(metaContex);
        SubscribeButtons();
    }

    private void SubscribeButtons()
    {
        foreach (MetaPointView point in points)
        {
            point.Button.onClick.AddListener(() => PointClick(point));
            point.OnPointerEnterAction += ShowInfo;
            point.OnPointerExitAction += HideInfo;
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
        int level;
        if (playerPoint is not null)
        {
            playerPoint.Level++;
            level = playerPoint.Level;
        }
        else
        {
            level = 1;
            player.MetaPoints.Add(new()
            {
                ID = point.Data.ID,
                Level = 1
            });
        }
        
        SaveLoadSystem<ProgressData>.Save(player);

        if (level == point.Data.MaxLevel)
        {
            point.ToMaxFrame();
            point.Button.onClick.RemoveAllListeners();
        }
        else
            point.ToLevelFrame();
        
        foreach (MetaPointData unlock in point.Data.Unlocks)
        {
            MetaPointView unlockPoint = points.First(p => p.Data.ID == unlock.ID);
            unlockPoint.gameObject.SetActive(true);
            unlockPoint.Initialize();
        }
    }

    private void EnablePointsBy(MetaContext metaContex)
    {
        points[0].Initialize();
        
        foreach (PlayerDataMetaPoint point in metaContex.Player.MetaPoints)
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
        foreach (var point in points)
        {
            point.OnPointerEnterAction -= ShowInfo;
            point.OnPointerExitAction -= HideInfo;
            point.Button.onClick.RemoveAllListeners();
        }
    }
}