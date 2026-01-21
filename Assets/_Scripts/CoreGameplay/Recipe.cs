using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Recipe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private FindObjectManager puzzleManager;
    
    [SerializeField]
    private GemInRecipe recipeIngredientPrefab;
    [SerializeField]
    private RectTransform recipeParent;
    
    [SerializeField]
    private Image image;
    [SerializeField]
    private Color grey;
    private List<GemInRecipe> gems = new();

    private bool isPlayerHandOwerRecipe;
    
    public event Action RecipeIsComplete;

    void Awake()
    {
        image.color = grey;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPlayerHandOwerRecipe = true;
        image.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPlayerHandOwerRecipe = false;
        image.color = grey;
    }
    internal void SetResipe(List<Gem> gems)
    {
        foreach (Gem gem in gems)
        {
            GemInRecipe newRecipeIngridient
                = Instantiate(recipeIngredientPrefab, recipeParent);

            newRecipeIngridient.SetView(gem.Sprite.sprite, gem.Sprite.color);
            this.gems.Add(newRecipeIngridient);
        }
    }
    internal bool CheckGem(GemData playerGem)
    {
        if (!isPlayerHandOwerRecipe)
            return false;
        
        foreach (var gem in gems)
        {
            if (gem != playerGem) 
                continue;
            
            Destroy(gem.gameObject);
            gems.Remove(gem);
            if(gems.Count == 0)
                RecipeIsComplete?.Invoke();
            return true;
        }
        
        return false;
    }

    public void Clear()
    {
        gems = new();
        
        if(recipeParent.childCount == 0)
            return;
        
        for (int i = recipeParent.childCount - 1; i >= 0; i--)
            Destroy(recipeParent.GetChild(i).gameObject);
    }
}