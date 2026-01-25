using System;
using UnityEngine;
using UnityEngine.UI;
using FirMath;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        
        List<int> recipeIntList = GameMath.AFewCardsFromTheDeck(contex.RecipeGemCount, contex.ShapeCount * contex.ColorCount);

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
        player.AddGold(200);
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