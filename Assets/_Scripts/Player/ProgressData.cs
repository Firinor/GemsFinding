using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ProgressData
{
    public int GoldCoins;
    public List<PlayerDataMetaPoint> MetaPoints = new();
    
    [NonSerialized] public Stats Stats;

#region Methods
    public event Action<int> OnGoldChange;
    
    public void AddGold(int count)
    {
        GoldCoins += count;
        OnGoldChange?.Invoke(GoldCoins);
    }
    public bool TrySpendGold(int count)
    {
        if (GoldCoins < count)
            return false;

        GoldCoins -= count;
        OnGoldChange?.Invoke(GoldCoins);
        return true;
    }
    public int GetPointsLevel() => MetaPoints.Sum(point => point.Level);
    public void InitializeStats(IEnumerable<MetaPointData> points)
    {
        Stats = new Stats();
    
        foreach (MetaPointData point in points)
        {
            PlayerDataMetaPoint playerPoint = MetaPoints.FirstOrDefault(p => p.ID == point.ID);
            
            if(playerPoint is null)
                continue;
            
            switch(point.Type)
            {
                case MetaPointType.RecipeCount:
                    Stats.RecipeGemCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.InRiverGemsCount:
                    Stats.InRiverGemCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.InBoxGemsCount:
                    Stats.InBoxGemCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.GemShapeCount:
                    Stats.ShapeCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.GemColorCount:
                    Stats.ColorCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.GemSpoilCount:
                    Stats.SpoilCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.GemDuoColorCount:
                    Stats.DuoColorCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.GemDuoShapeCount:
                    Stats.DuoShapeCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.Invisible:
                    Stats.InvisibleCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.Moveble:
                    Stats.MovebleCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.Jumpble:
                    Stats.JumpbleCount += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.ChangingColor:
                    Stats.ChangingColor += point.Value * playerPoint.Level;
                    break;
                case MetaPointType.Light:
                    Stats.LightRadius += point.Value * playerPoint.Level;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        if (Stats.InBoxGemCount > Stats.ColorCount * Stats.ShapeCount)
            Stats.InBoxGemCount = Stats.ColorCount * Stats.ShapeCount;
    }
#endregion
}

[Serializable]
public class Stats
{
    public int InRiverGemCount = 20;
    public int InBoxGemCount = 7;
    public int RecipeGemCount = 1;
    [Space]
    public int ColorCount = 1;
    public int ShapeCount = 7;
    public int SpoilCount;
    public int DuoColorCount;
    public int DuoShapeCount;
    [Space]
    public float EmptyDirt = 90;
    public int NoDirt;
    public int WithTail;
    public int WithLight2D;
    public int NoBlink;
    [Space]
    public int InvisibleCount;
    public int MovebleCount;
    public int JumpbleCount;
    public int ChangingColor;
    [Space]
    public float LightRadius = 4;
}