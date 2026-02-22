using System;

[Serializable]
public class ProgressData
{
    public int GoldCoins;
    public Stats Stats;
    
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
}

[Serializable]
public class Stats
{
    public int PlayerLevel = 1;
    
    public int InPoolCount = 60;
    public int InBoxGemCount = 30;
    public int RecipeGemCount = 1;

    public bool isDebug;
}