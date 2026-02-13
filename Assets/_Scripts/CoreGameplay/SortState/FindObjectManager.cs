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
    private CameraController cameraController;
    [SerializeField] 
    private CanvasView canvas;
    [SerializeField] 
    private GemBox GemBox;
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
        
        Gem.box = GemBox;
        Gem.riverZone = spawnZone;
        
        GemBox.Initialize(player.Stats.InBoxGemCount, player.Stats.InRiverGemCount);
        GemBox.OnFull += ToSortState;   
        canvas.Recipe.RecipeIsComplete += SuccessfullySolvePuzzle;
        canvas.ToCachButton.gameObject.SetActive(false);
        StartCoroutine(StartPuzzle());
    }

    public void ToCachState()
    {
        GemBox.ToCachMode();
        cameraController.ToCach();
        canvas.ToCachButton.gameObject.SetActive(false);
    }
    
    private void ToSortState()
    {
        GemBox.ToSotrMode();
        StartCoroutine(GemBox.MoveToSortPoint(onComplete: () =>
        {
            if(GemBox.Limit > 0)
                canvas.ToCachButton.gameObject.SetActive(true);
        }));
        cameraController.ToSort();
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
        float yieldDelay = spawnTotalTime/contex.InRiverGemCount;
        
        int gemCount = (int)(contex.InRiverGemCount * contex.EmptyDirt / 100);
        
        List<int> gemAtlas = GameMath.AFewCardsFromTheDeck(gemCount, contex.ColorCount * contex.ShapeCount);
        
        List<int> noDirtIndexes = GameMath.AFewCardsFromTheDeck(contex.NoDirt, gemCount);
        List<int> emptyDirtIndexes = GameMath.AFewCardsFromTheDeck(gemCount - contex.NoDirt, contex.InRiverGemCount - gemCount);
        List<int> tailIndexes = GameMath.AFewCardsFromTheDeck(contex.WithTail,  gemCount);
        List<int> light2DIndexes = GameMath.AFewCardsFromTheDeck(contex.WithLight2D, gemCount);
        noDirtIndexes.Sort();
        emptyDirtIndexes.Sort();
        tailIndexes.Sort();
        light2DIndexes.Sort();
        int gemAtlasIndex = 0;
        int noDirtIndex = 0;
        int emptyDirtIndex = 0;
        int tailIndex = 0;
        int light2DIndex = 0;
        
        for (int i = 0; i < contex.InRiverGemCount; i++)
        {
            timer -= yieldDelay;
            while (timer < 0)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            Gem newGem = pool.Get();
            newGem.OnEdge += Respawn;
            
            int colorIndex;
            int spriteIndex = i / contex.ColorCount;
            
            //if (contex.ColorCount < 3)
            //    colorIndex = i % puzzleConfig.GemsColors.Length;
            //else
            colorIndex = i % contex.ColorCount;

            GemBuilder builder = new GemBuilder().New(newGem)
                .SetView(puzzleConfig.GemsSprites[spriteIndex], puzzleConfig.GemsColors[colorIndex]);
            if (true)
                builder.NoGem();
            if (true)
                builder.NoTail();
            if (true)
                builder.NoLight2D();

            Respawn(newGem);
            
            allIngredients.Add(newGem);
        }
        
        CreateNewRecipe(gemCount);
    }

    private void Respawn(Gem gem)
    {
        float x = (spawnZone.bounds.max.x - spawnZone.bounds.min.x) * Random.value;
        x += spawnZone.bounds.min.x;
        float y = (spawnZone.bounds.max.y - spawnZone.bounds.min.y) * Random.value;
        y += spawnZone.bounds.min.y;
        gem.transform.localPosition = new Vector3(x, y, 0);
        gem.ResetTail();
        gem.ResetPhysics();
        gem.SetRandomImpulse(forceToIngredient);
    }

    private void SuccessfullySolvePuzzle()
    {
        int reward = player.Stats.ShapeCount * player.Stats.ColorCount * player.Stats.RecipeGemCount + player.Stats.InBoxGemCount;
        canvas.RewardInfoText.text = $"Формы: {player.Stats.ShapeCount}" +
                              $"\nЦвета: {player.Stats.ColorCount}" +
                              $"\nРецепт: {player.Stats.RecipeGemCount}" +
                              $"\nКоличество: {player.Stats.InBoxGemCount}" +
                              "\n" +
                              $"\nИтого: {player.Stats.ShapeCount}*{player.Stats.ColorCount}*{player.Stats.RecipeGemCount} + {player.Stats.InBoxGemCount} = {reward}$";

        canvas.RewardText.text = $"ПОЗДРАВЛЯЮ!\nТВОЙ ПРИЗ\n{reward}$";
        
        player.AddGold(reward);
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

        foreach (Gem gem in allIngredients)
        {
            gem.OnEdge -= Respawn;
        }
    }
}