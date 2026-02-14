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
        canvas.ToSortButton.gameObject.SetActive(true);
        StartCoroutine(StartPuzzle());
    }

    public void ToCachState()
    {
        GemBox.ToCachMode();
        cameraController.ToCach();
        canvas.ToCachButton.gameObject.SetActive(false);
        canvas.ToSortButton.gameObject.SetActive(true);
    }
    
    public void ToSortState()
    {
        bool toSort = GemBox.ToSotrMode();
        if(!toSort)    
            return;
        
        StartCoroutine(GemBox.MoveToSortPoint(onComplete: () =>
        {
            if(GemBox.Limit > 0)
                canvas.ToCachButton.gameObject.SetActive(true);
        }));
        cameraController.ToSort();
        canvas.ToSortButton.gameObject.SetActive(false);
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
        
        List<int> emptyDirtIndexes = GameMath.AFewCardsFromTheDeck(contex.InRiverGemCount - gemCount, contex.InRiverGemCount);
        List<int> noDirtIndexes = GameMath.AFewCardsFromTheDeck(contex.NoDirt, gemCount);
        List<int> tailIndexes = GameMath.AFewCardsFromTheDeck(contex.WithTail,  gemCount);
        List<int> light2DIndexes = GameMath.AFewCardsFromTheDeck(contex.WithLight2D, gemCount);
        emptyDirtIndexes?.Sort();
        noDirtIndexes?.Sort();
        tailIndexes?.Sort();
        light2DIndexes?.Sort();
        int gemAtlasIndex = 0;
        int emptyDirtIndex = 0;
        int gemIndex = 0; 
        int noDirtIndex = 0;
        int tailIndex = 0;
        int light2DIndex = 0;

        List<Gem> allEntity = new(contex.InRiverGemCount);
        
        for (int i = 0; i < contex.InRiverGemCount; i++)
        {
            Gem newGem = pool.Get();
            newGem.transform.localPosition = spawnZone.bounds.center;
            newGem.NoGravity();
            newGem.OnEdge += Respawn;

            if (emptyDirtIndexes is not null 
                && i == emptyDirtIndexes[emptyDirtIndex])
            {
                //NoGem
                newGem.Sprite.enabled = false;
                newGem.RemoveTail();
                newGem.RemoveLight2D();
                emptyDirtIndex++;
                if (emptyDirtIndex >= emptyDirtIndexes.Count)
                    emptyDirtIndexes = null;
                allEntity.Add(newGem);
                continue;
            }
            
            int s = gemAtlas[gemAtlasIndex++];
            int spriteIndex = s / contex.ColorCount;
            int colorIndex = s % contex.ColorCount;
            
            newGem.SetView(puzzleConfig.GemsSprites[spriteIndex], puzzleConfig.GemsColors[colorIndex]);
            //NoDirt
            if (noDirtIndexes is not null
                && gemIndex == noDirtIndexes[noDirtIndex])
            {
                noDirtIndex++;
                if (noDirtIndex >= noDirtIndexes.Count)
                    noDirtIndexes = null;
                newGem.RemoveDirt();
            }
            gemIndex++;
            //Tail
            if (tailIndexes is not null)
            {
                if(i != tailIndexes[tailIndex])
                    newGem.RemoveTail();
                else
                {
                    tailIndex++;
                    if (tailIndex >= tailIndexes.Count)
                        tailIndexes = null;
                }
            }
            else
                newGem.RemoveTail();
            //Light2D
            if (light2DIndexes is not null)
            {
                if(i != light2DIndexes[light2DIndex])
                    newGem.RemoveLight2D();
                else
                {
                    light2DIndex++;
                    if (light2DIndex >= light2DIndexes.Count)
                        light2DIndexes = null;
                }
            }
            else 
                newGem.RemoveLight2D();
            
            allEntity.Add(newGem);
            allIngredients.Add(newGem);
        }
        
        CreateNewRecipe(gemCount);

        for (int i = 0; i < contex.InRiverGemCount; i++)
        {
            timer -= yieldDelay;
            while (timer < 0)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Respawn(allEntity[i]);
        }
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
        canvas.ToCachButton.gameObject.SetActive(false);
        
        int reward = player.Stats.ShapeCount * player.Stats.ColorCount * player.Stats.RecipeGemCount + player.Stats.InRiverGemCount;
        canvas.RewardText.text = $"ПОЗДРАВЛЯЮ!";
        canvas.RewardCurrencyText.text = $"ТВОЙ ПРИЗ {reward}";
        canvas.RewardInfoText.text = $"Формы: {player.Stats.ShapeCount}" +
                                     $"\nЦвета: {player.Stats.ColorCount}" +
                                     $"\nРецепт: {player.Stats.RecipeGemCount}" +
                                     $"\nКоличество: {player.Stats.InRiverGemCount}";
        canvas.RewardFormulaText.text 
            = $"Итого: {player.Stats.ShapeCount}*{player.Stats.ColorCount}*{player.Stats.RecipeGemCount} + {player.Stats.InRiverGemCount} = {reward}";

        
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