using System;
using System.Collections;
using UnityEngine;
using FirMath;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class FindObjectManager : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private FindObjectPuzzleConfig puzzleConfig;
    [SerializeField]
    private PlayerHandManager playerHandManager;
    
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
    private Transform spawnZone;
    [SerializeField]
    private float spawnDistance = 20f;
    
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

        Gem.centerZone = spawnZone;
        
        float timer = 0;
        float yieldDelay = spawnTotalTime/contex.InBoxGemCount;
        
        List<int> gemAtlas = GameMath.AFewCardsFromTheDeck(contex.InBoxGemCount, contex.RecipeGemCount);
        
        for (int i = 0; i < contex.InBoxGemCount; i++)
        {
            Gem newGem = pool.Get();
            
            float direction = Random.value * 360 * Mathf.Deg2Rad;
            newGem.transform.position = spawnZone.position
                                        + new Vector3(math.cos(direction), 0, math.sin(direction)) * spawnDistance;
            
            newGem.SetView(puzzleConfig.GemsSprites[i]);
            newGem.SetRandomImpulse(forceToIngredient);
            
            allIngredients.Add(newGem);
        }
        
        CreateNewRecipe(gemAtlas.Count);
        playerHandManager.Initialize();

        for (int i = 0; i < contex.InBoxGemCount; i++)
        {
            timer -= yieldDelay;
            while (timer < 0)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            allIngredients[i].enabled = true;
        }
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