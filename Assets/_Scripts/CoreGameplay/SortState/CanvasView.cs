using System;
using CartoonFX;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FirAnimations;

[Serializable]
public class CanvasView
{
    public GameObject WinScreen;
    public TextMeshProUGUI LevelText;
    public FirAnimation RecipeAnim;
    public FirAnimation LevelAnim;
    public FirAnimation WinAnim;
    public FirAnimation InfoLeftAnim;
    public FirAnimation InfoRightAnim;
    public FirAnimation ContinueAnim;
    public TextMeshProUGUI LevelInfoText;
    public Recipe Recipe;
}