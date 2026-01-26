using System;
using UnityEngine;
using UnityEngine.UI;
using FirMath;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class FindObjectManager : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private FindObjectPuzzleConfig puzzleConfig;
    
    [SerializeField]
    private GemPool pool;
    
    private List<Gem> allIngredients;
    [SerializeField]
    private Recipe recipe;
    [SerializeField]
    private GameObject winScreen;
    [SerializeField] 
    private TextMeshProUGUI rewardText;
    [SerializeField]
    private TextMeshProUGUI rewardInfoText;
    [SerializeField] 
    private BoxCollider2D GemZone;
    [SerializeField]
    private float forceToIngredient;
    [SerializeField]
    private float spawnDistance;

    private ProgressData player;
    private Stats contex;
    
    //public ParticleSystem successParticleSystem;
    //public ParticleSystem errorParticleSystem;
    
    #endregion

    public void Initialize(ProgressData player)
    {
        this.player = player;
        contex = player.Stats;
        
        recipe.RecipeIsComplete += SuccessfullySolvePuzzle;
        StartPuzzle();
    }

    private void CreateNewRecipe()
    {
        recipe.Clear();

        int DeckLenght = Math.Min(contex.InBoxGemCount, contex.ShapeCount * contex.ColorCount);
        List<int> recipeIntList = GameMath.AFewCardsFromTheDeck(contex.RecipeGemCount, DeckLenght);

        List<Gem> recipeGems = new();
        foreach (var i in recipeIntList)
            recipeGems.Add(allIngredients[i]);
        
        recipe.SetResipe(recipeGems);
    }

    [ContextMenu("StartPuzzle")]
    public void StartPuzzle()
    {
        winScreen.SetActive(false);
        pool.ClearAll();
        
        allIngredients = new List<Gem>();
        
        for (int i = 0; i < contex.InBoxGemCount; i++)
        {
            Gem newGem = pool.Get();

            float direction = Random.value * 360 * Mathf.Deg2Rad;
            newGem.transform.localPosition = new Vector3(math.cos(direction), math.sin(direction), 0) * spawnDistance;

            int colorIndex;
            int spriteIndex = i / contex.ColorCount;
            if (contex.ColorCount < 3)
                colorIndex = i % puzzleConfig.GemsColors.Length;
            else
                colorIndex = i % contex.ColorCount;
            
            newGem.SetView(puzzleConfig.GemsSprites[spriteIndex], puzzleConfig.GemsColors[colorIndex]);
            newGem.SetRandomImpulse(forceToIngredient);
            
            allIngredients.Add(newGem);
        }
        Gem.Bounds = GemZone.bounds;
        
        CreateNewRecipe();
    }
    
    private void SuccessfullySolvePuzzle()
    {
        int reward = player.Stats.ShapeCount * player.Stats.ColorCount * player.Stats.RecipeGemCount + player.Stats.InBoxGemCount;
        rewardInfoText.text = $"Формы: {player.Stats.ShapeCount}" +
                              $"\nЦвета: {player.Stats.ColorCount}" +
                              $"\nРецепт: {player.Stats.RecipeGemCount}" +
                              $"\nКоличество: {player.Stats.InBoxGemCount}" +
                              "\n" +
                              $"\nИтого: {player.Stats.ShapeCount}*{player.Stats.ColorCount}*{player.Stats.RecipeGemCount} + {player.Stats.InBoxGemCount} = {reward}$";

        rewardText.text = $"ПОЗДРАВЛЯЮ!\nТВОЙ ПРИЗ\n{reward}$";
        
        player.AddGold(reward);
        SaveLoadSystem<ProgressData>.Save(player);
        winScreen.SetActive(true);
    }

    private void HarvestAllIngredients()
    {
        /*float border = 0;
        float force = 0;

        List<Gem> ingredientsToDestroy = new List<Gem>();
        
        foreach (Gem ingredient in ingredientsToDestroy)
        {
            Destroy(ingredient.gameObject);
        }*/
    }

    /*internal void Particles(Vector3 position, bool success)
    {
        ParticleSystem particleSystem = success ? successParticleSystem : errorParticleSystem;

        RectTransform rectTransform = particleSystem.GetComponent<RectTransform>();
        rectTransform.localPosition = position;

        particleSystem.Play();
    }*/

    internal void RemoveIngredient(Gem gem)
    {
        allIngredients.Remove(gem);
    }

    private void OnDestroy()
    {
        recipe.RecipeIsComplete -= SuccessfullySolvePuzzle;
    }
}