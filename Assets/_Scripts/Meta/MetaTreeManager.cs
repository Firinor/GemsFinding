using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MetaTreeManager : MonoBehaviour
{
    [SerializeField] 
    private List<MetaPointView> points;

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
        }
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
        
        point.SetText(level);
        
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
            treePoint.SetText(point.Level);
            
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
            point.Button.onClick.RemoveAllListeners();
        }
    }
}