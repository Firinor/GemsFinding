using System;
using System.Collections;
using UnityEngine;
using FirMath;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class FindObjectManager : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private FindObjectPuzzleConfig puzzleConfig;
    
    private List<Gem> allIngredients;
    [SerializeField]
    private GemPool pool;
    [SerializeField] 
    private CanvasView canvas;
    [SerializeField]
    private float forceToIngredient;
    [SerializeField]
    private float spawnTotalTime = 5f;
    [SerializeField]
    private BoxCollider2D spawnZone;

    private ProgressData player;
    private Stats contex;
    
    //public ParticleSystem successParticleSystem;
    //public ParticleSystem errorParticleSystem;
    
    #endregion

    public void Initialize(ProgressData player)
    {
        this.player = player;
        contex = player.Stats;
        
        StartCoroutine(StartPuzzle());
    }

    private void CreateNewRecipe(int gemCount)
    {
        canvas.Recipe.Clear();
        
        List<int> recipeIntList = GameMath.AFewCardsFromTheDeck(contex.RecipeGemCount, gemCount);

        List<Gem> recipeGems = new();
        foreach (var i in recipeIntList)
            recipeGems.Add(allIngredients[i]);
        
        canvas.Recipe.SetResipe(recipeGems);
    }

    [ContextMenu("StartPuzzle")]
    public IEnumerator StartPuzzle()
    {
        canvas.WinScreen.SetActive(false);
        pool.ClearAll();
        
        allIngredients = new List<Gem>();
        
        float timer = 0;
        float yieldDelay = spawnTotalTime/contex.InBoxGemCount;
        
        List<int> gemAtlas = GameMath.AFewCardsFromTheDeck(contex.InBoxGemCount, contex.RecipeGemCount);
        
        for (int i = 0; i < contex.InBoxGemCount; i++)
        {
            Gem newGem = pool.Get();
            newGem.transform.localPosition = spawnZone.bounds.center;
            newGem.SetView(puzzleConfig.GemsSprites[i]);
            
            allIngredients.Add(newGem);
        }
        
        CreateNewRecipe(gemAtlas.Count);

        for (int i = 0; i < contex.InBoxGemCount; i++)
        {
            timer -= yieldDelay;
            while (timer < 0)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Respawn(allIngredients[i]);
        }
    }

    private void Respawn(Gem gem)
    {
        float x = (spawnZone.bounds.max.x - spawnZone.bounds.min.x) * Random.value;
        x += spawnZone.bounds.min.x;
        float y = (spawnZone.bounds.max.y - spawnZone.bounds.min.y) * Random.value;
        y += spawnZone.bounds.min.y;
        gem.transform.localPosition = new Vector3(x, y, 0);
    }

    private void SuccessfullySolvePuzzle()
    {
        canvas.ToCachButton.gameObject.SetActive(false);

        canvas.RewardText.text = $"ПОЗДРАВЛЯЮ!";
        
        player.AddGold(100500);
        SaveLoadSystem<ProgressData>.Save(player);
        canvas.WinScreen.SetActive(true);
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
        canvas.Recipe.RecipeIsComplete -= SuccessfullySolvePuzzle;
    }
}