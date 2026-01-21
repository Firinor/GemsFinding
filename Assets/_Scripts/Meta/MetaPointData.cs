using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/" + nameof(MetaPointData))]
public class MetaPointData : ScriptableObject
{
    public Sprite Icon;
    public MetaPointType Type;
    public int Value;
    public int MaxLevel;

    public int[] Cost;

    public MetaPointData[] Unlocks;
}