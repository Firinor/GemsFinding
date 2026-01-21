using System;
using UnityEngine;

public class GemPool : MonoBehaviour
{
    [SerializeField]
    private Gem ingredientPrefab;
    [SerializeField]
    private Transform ingredientParent;
    
    [SerializeField]
    private SoundManager sound;

    [SerializeField]
    private int startLayer;
    private int currentLayerStep;
    
    public Transform GemParent => ingredientParent;
    
    private void Start()
    {
        for (int i = 0; i < ingredientParent.childCount - 1; i++)
        {
            ingredientParent.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = startLayer + currentLayerStep;
            currentLayerStep++;
            ingredientParent.GetChild(i).GetComponent<Gem>().OnBoundTink += sound.PlayGemTink;
        }
    }

    public Gem Get()
    {
        Gem result = null;
        for (int i = 0; i < ingredientParent.childCount - 1; i++)
        {
            if(ingredientParent.GetChild(i).gameObject.activeSelf)
               continue;

            result = ingredientParent.GetChild(i).GetComponent<Gem>();
            break;
        }
        
        if (result is null)
        {
            result = Instantiate(ingredientPrefab, ingredientParent);
            result.GetComponent<SpriteRenderer>().sortingOrder = startLayer + currentLayerStep;
            currentLayerStep++;
        }

        result.ResetPhysics();
        result.enabled = true;
        result.gameObject.SetActive(true);
        
        return result;
    }
    
    public void Return(Gem gem)
    {
        gem.gameObject.SetActive(false);
    }

    public void ClearAll()
    {
        for (int i = ingredientParent.childCount - 1; i >= 0; i--)
        {
            ingredientParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        for (int i = ingredientParent.childCount - 1; i >= 0; i--)
        {
            ingredientParent.GetChild(i).GetComponent<Gem>().OnBoundTink -= sound.PlayGemTink;
        }
    }
}
