using System;
using System.Collections.Generic;

[Serializable]
public class ProgressData
{
    public int GoldCoins;

    public List<PlayerDataMetaPoint> MetaPoints = new();

    public Stats Stats = new Stats();
}

public class Stats
{
    public int InBoxGemCount = 7;
    public int RecipeGemCount = 1;

    public int ColorCount = 1;
    public int ShapeCount = 7;
    public int SpoilCount;
    public int DuoColorCount;
    public int DuoShapeCount;
    
    public int InvisibleCount;
    public int MovebleCount;
    public int JumpbleCount;
    public int ChangingColor;
    
    public int LightRadius = 4;
}