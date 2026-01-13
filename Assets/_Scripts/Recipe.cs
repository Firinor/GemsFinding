using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Recipe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private FindObjectManager puzzleOperator;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Color grey;
    private List<Gem> recipe;
    private int ingredientCount;

    void Awake()
    {
        image.color = grey;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("RecipeOperator pointer enter");
        image.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("RecipeOperator pointer exit");
        image.color = grey;
        puzzleOperator.PointerOnRecipe = false;
    }
    internal void SetResipe(List<Gem> recipe)
    {
        this.recipe = recipe;
        ingredientCount = recipe.Count;
    }
    internal bool ActivateIngredient(int keyIngredientNumber)
    {
        Gem blackIngredient = recipe[keyIngredientNumber - 1];
        //blackIngredient.Success();

        //recipe.Remove(blackIngredient);
        ingredientCount--;
        return ingredientCount == 0;
    }
}