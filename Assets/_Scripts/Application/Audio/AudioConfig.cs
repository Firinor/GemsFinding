using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Audio")]
public class AudioConfig : ScriptableObject
{
    [Header("Gem")] 
    public ClipSettings gemTink;
    [Header("Recipe")]
    public ClipSettings correctGem;
    public ClipSettings errorGem;
    [Header("Buttons")]
    public ClipSettings buttonClick;
    public ClipSettings mouseClick;
    [Header("Victory")]
    public ClipSettings victory;
}