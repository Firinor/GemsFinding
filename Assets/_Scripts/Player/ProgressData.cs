using System;
using System.Collections.Generic;

[Serializable]
public class ProgressData
{
    public int GoldCoins;

    public List<PlayerDataMetaPoint> MetaPoints = new();
}