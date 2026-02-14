using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CanvasView
{
    public GameObject WinScreen;
    public TextMeshProUGUI RewardText;
    public TextMeshProUGUI RewardCurrencyText;
    public TextMeshProUGUI RewardInfoText;
    public TextMeshProUGUI RewardFormulaText;
    public TextMeshProUGUI CountText;
    public Button ToCachButton;
    public Button ToSortButton;
    public Recipe Recipe;
}