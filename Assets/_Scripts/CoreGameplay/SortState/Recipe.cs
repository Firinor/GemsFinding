using System;
using System.Collections;
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
    private AnimationCurve curve;
    private float animationTimer;
    [SerializeField]
    private SoundManager sound;
    
    [SerializeField]
    private Image image;
    [SerializeField]
    private Color grey;
    private List<GemInRecipe> gems = new();

    private bool isPlayerHandOwerRecipe;
    public bool IsPlayerOnRecipe => isPlayerHandOwerRecipe;

    private Vector3 normalScale;
    private Vector3 targetScale;
    
    public ParticleSystem successParticleSystem;
    public ParticleSystem errorParticleSystem;
    
    public event Action RecipeIsComplete;
    public event Action WrongIngridient;

    void Awake()
    {
        image.color = grey;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPlayerHandOwerRecipe = true;
        image.color = Color.white;
        StopAllCoroutines();
        StartCoroutine(ToMaxSize());
    }

    private IEnumerator ToMaxSize()
    {
        while (animationTimer < 1)
        {
            yield return null;
            animationTimer += Time.deltaTime;
            float curveValue = curve.Evaluate(animationTimer);
            recipeParent.localScale = Vector3.one + Vector3.one/2*curveValue;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPlayerHandOwerRecipe = false;
        image.color = grey;
        StopAllCoroutines();
        StartCoroutine(ToMinSize());
    }

    private IEnumerator ToMinSize()
    {
        while (animationTimer > 0)
        {
            yield return null;
            animationTimer -= Time.deltaTime;
            float curveValue = curve.Evaluate(animationTimer);
            recipeParent.localScale = Vector3.one + Vector3.one/2*curveValue;
        }
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
            if (gems.Count == 0)
            {
                RecipeIsComplete?.Invoke();
            }
            Particles(gem.transform.position, success: true);
            sound.PlayCurrectGem();
            return true;
        }

        sound.PlayErrorGem();
        WrongIngridient?.Invoke();
        Particles(playerGem.Position, false);
        return false;
    }

    internal void Particles(Vector3 position, bool success)
    {
        ParticleSystem particleSystem = success ? successParticleSystem : errorParticleSystem;

        Instantiate(particleSystem, position, Quaternion.identity, transform);

        particleSystem.Play();
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