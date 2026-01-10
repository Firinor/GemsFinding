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
    private Image box;
    
    [SerializeField]
    private GemPool pool;
    
    private List<Gem> allIngredients;
    [SerializeField]
    private GemInRecipe recipeIngredientPrefab;
    [SerializeField]
    private RectTransform recipeParent;
    [SerializeField]
    private Recipe recipe;
    [SerializeField]
    private int recipeIngredientCount = 5;
    [SerializeField] 
    private BoxCollider2D GemZone;
    [SerializeField]
    private int ingredientInBoxCount = 250; 
    [SerializeField]
    private float forceToIngredient;
    [SerializeField]
    private float spawnDistance;
    private float acceleration = 0.025f;

    //public ParticleSystem successParticleSystem;
    //public ParticleSystem errorParticleSystem;

    [HideInInspector]
    public bool PointerOnRecipe;
    #endregion

    public void RetryPuzzle()
    {
        OnEnable();
    }

    protected void OnEnable()
    {
        CreateNewRecipe();
        StartPuzzle();
    }

    private void CreateNewRecipe()
    {
        int gemsCount = puzzleConfig.GemsSprites.Length;
        int gemsColors = puzzleConfig.GemsColors.Length;
        
        //ingredientInBoxCount = gemsCount * gemsColors;
        List<int> recipeIntList = GenerateNewRecipe(recipeIngredientCount, ingredientInBoxCount);

        foreach (int i in recipeIntList)
        {
            GemInRecipe newRecipeIngridient
                = Instantiate(recipeIngredientPrefab, recipeParent);
            int spriteIndex = i / gemsColors;
            
            int colorIndex = i % gemsColors;
            
            newRecipeIngridient.SetView(puzzleConfig.GemsSprites[spriteIndex], puzzleConfig.GemsColors[colorIndex]);
            //recipe.Add(newRecipeIngridient);
        }
        //recipe.SetResipe(recipeList);
    }

    [ContextMenu("StartPuzzle")]
    public void StartPuzzle()
    {
        pool.ClearAll();
        //OpenBox();

        allIngredients = new List<Gem>();

        int gemsCount = puzzleConfig.GemsSprites.Length;
        int gemsColors = puzzleConfig.GemsColors.Length;
        
        //ingredientInBoxCount = gemsCount * gemsColors;
        
        for (int i=0; i<ingredientInBoxCount; i++)
        {
            Gem newGem = pool.Get();

            float direction = Random.value * 360 * Mathf.Deg2Rad;
            newGem.transform.localPosition = new Vector3(math.cos(direction), math.sin(direction), 0) * spawnDistance;
            
            int spriteIndex = i / gemsColors;
            int colorIndex = i % gemsColors;
            
            newGem.SetView(puzzleConfig.GemsSprites[spriteIndex], puzzleConfig.GemsColors[colorIndex]);
            newGem.SetRandomImpulse(forceToIngredient);
            
            allIngredients.Add(newGem);
        }
        allIngredients[0].SetBounds(GemZone.bounds);
    }
    
    public async void SuccessfullySolvePuzzle()
    {
        await HarvestAllIngredients();
        CloseBox();

        await Task.Delay(500);
        //base.SuccessfullySolvePuzzle();
    }

    private void CloseBox()
    {
        box.sprite = puzzleConfig.ClosedChest;
        box.SetNativeSize();
    }
    private void OpenBox()
    {
        box.sprite = puzzleConfig.OpenedChest;
        box.SetNativeSize();
        box.GetComponent<Button>().enabled = false;
    }

    private async Task HarvestAllIngredients()
    {
        float border = 0;
        float force = 0;

        List<Gem> ingredientsToDestroy = new List<Gem>();

        while (allIngredients != null && allIngredients.Count > 0)
        {
            for(int i = 0; i < 30 && i < allIngredients.Count; i++)
            {
                if (allIngredients[i].OnBox(border))
                {
                    allIngredients[i].SetImpulse(0, toZeroPoint: true);
                    ingredientsToDestroy.Add(allIngredients[i]);
                }
                else
                {
                    allIngredients[i].SetImpulse(force, toZeroPoint: true);
                }
            }
            foreach (Gem ingredient in ingredientsToDestroy)
            {
                allIngredients.Remove(ingredient);
            }
            border += acceleration;
            force += acceleration;

            await Task.Yield();
        }
        foreach (Gem ingredient in ingredientsToDestroy)
        {
            Destroy(ingredient.gameObject);
        }
    }

    private List<int> GenerateNewRecipe(int recipeIngredientCount, int length)
    {
        return GameMath.AFewCardsFromTheDeck(recipeIngredientCount, length);
    }

    internal void ActivateIngredient(int keyIngredientNumber)
    {
        bool TheRecipeIsReady = recipe.ActivateIngredient(keyIngredientNumber);
        if (TheRecipeIsReady)
        {
            SuccessfullySolvePuzzle();
        }
    }

    /*private bool BrakingField(ref Vector3 pos)
    {
        return pos.y > recipeOffset;
    }*/

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

    public void SkipPuzzle()
    {
        SuccessfullySolvePuzzle();
    }
}