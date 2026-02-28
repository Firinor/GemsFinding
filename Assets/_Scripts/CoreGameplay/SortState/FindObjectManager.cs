using System;
using System.Collections;
using UnityEngine;
using FirMath;
using System.Collections.Generic;
using System.Threading.Tasks;
using MirraGames.SDK;
using Unity.Mathematics;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class FindObjectManager : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private SpriteAtlas spriteAtlas;
    private Sprite[] sprites;
    
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
    private ADSManager adsManager;
    [SerializeField]
    private float spawnDistance = 20f;
    
    private ProgressData player;
    private Stats contex;
    
    public ParticleSystem completeParticleSystem;

    private float levelTime;
    #endregion

    public void Initialize(ProgressData player)
    {
        this.player = player;
        contex = player.Stats;

        levelTime = 0;
        enabled = true;
             
        int spriteCount = spriteAtlas.spriteCount;
        sprites = new Sprite[spriteCount];
        spriteAtlas.GetSprites(sprites);
        
        canvas.Recipe.RecipeIsComplete += SuccessfullySolvePuzzle;
        canvas.Recipe.WrongIngridient += WrongIngridient;
        
        StartCoroutine(StartPuzzle());
        adsManager.Initialize();
        MirraSDK.Analytics.GameIsReady();
    }

    private void WrongIngridient()
    {
        for (int i = 0; i < allIngredients.Count; i++)
        {
            allIngredients[i].SetRandomImpulse(forceToIngredient);
        }
    }

    private void Update()
    {
        levelTime += Time.deltaTime;
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

        RecalculateStatsByLevel();
        
        float timer = 0;
        float yieldDelay = spawnTotalTime/contex.InBoxGemCount;
        
        List<int> gemAtlas = GameMath.AFewCardsFromTheDeck(contex.InBoxGemCount, spriteAtlas.spriteCount);
        for (int i = 0; i < contex.InBoxGemCount; i++)
        {
            Gem newGem = pool.Get();
            
            float direction = Random.value * 360 * Mathf.Deg2Rad;
            newGem.transform.position = spawnZone.position
                                        + new Vector3(math.cos(direction), 0, math.sin(direction)) * spawnDistance;
            
            newGem.SetView(sprites[gemAtlas[i]]);
            newGem.SetRandomImpulse(forceToIngredient);
            
            allIngredients.Add(newGem);
        }
        
        CreateNewRecipe(gemAtlas.Count);
        canvas.LevelText.text = contex.PlayerLevel.ToString();
        canvas.NoAdsButtonAnim.Play();
        canvas.RecipeAnim.Play();
        canvas.LevelAnim.Play();
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

    private void RecalculateStatsByLevel()
    {
        if(contex.isDebug)
            return;

        if (contex.PlayerLevel == 1)
        {
            contex.InPoolCount = 8;
            contex.InBoxGemCount = 8;
            contex.RecipeGemCount = 2;
            return;
        }
        
        if (contex.PlayerLevel == 2)
        {
            contex.InPoolCount = 16;
            contex.InBoxGemCount = 16;
            contex.RecipeGemCount = 3;
            return;
        }
        
        if (contex.PlayerLevel == 3)
        {
            contex.InPoolCount = 32;
            contex.InBoxGemCount = 32;
            contex.RecipeGemCount = 4;
            return;
        }
        
        contex.InPoolCount = (int)Mathf.Min(70f + contex.PlayerLevel*2, spriteAtlas.spriteCount);
        contex.InBoxGemCount = (int)Mathf.Min(60f + contex.PlayerLevel*2, spriteAtlas.spriteCount);
        contex.RecipeGemCount = (int)Mathf.Min(4.7f + contex.PlayerLevel/6f, spriteAtlas.spriteCount);

        //Debug.Log("pool: " + contex.InPoolCount + " box: " + contex.InBoxGemCount + " recipe: " + contex.RecipeGemCount);
    }

    private void SuccessfullySolvePuzzle()
    {
        StartCoroutine(SuccessfullySolvePuzzleCoroutine());
    }

    private IEnumerator SuccessfullySolvePuzzleCoroutine()
    {
        completeParticleSystem.gameObject.SetActive(true);
        completeParticleSystem.Play();

        player.Stats.PlayerLevel++;
        //player.AddGold(100500);
        MirraSaveLoadSystem<ProgressData>.Save("Player", player);
        canvas.WinScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlayVictory();
        yield return new WaitForSeconds(0.5f);
        canvas.WinAnim.Play();
        yield return new WaitForSeconds(1f);
        canvas.ContinueAnim.Play();
        enabled = false;
        TimeSpan timer = TimeSpan.FromSeconds(levelTime);
        string timerText = $"{timer.Minutes:D1}:{timer.Seconds:D2}";
        canvas.LevelInfoText.text = $"{timerText}"
                                    + Environment.NewLine + $"{contex.InBoxGemCount}"
                                    + Environment.NewLine + $"{contex.RecipeGemCount}";
        canvas.InfoLeftAnim.Play();
        canvas.InfoRightAnim.Play();
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

    internal void RemoveIngredient(Gem gem)
    {
        allIngredients.Remove(gem);
    }

    private void OnDestroy()
    {
        canvas.Recipe.RecipeIsComplete -= SuccessfullySolvePuzzle;
    }
}