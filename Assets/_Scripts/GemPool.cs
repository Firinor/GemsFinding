using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GemPool : MonoBehaviour
{
    [SerializeField]
    private Gem ingredientPrefab;
    [SerializeField]
    private Transform ingredientParent;
    
    public List<Gem> Gems;

    public Gem Get()
    {
        Gem result =
           Gems.FirstOrDefault(gem => !gem.gameObject.activeSelf);

        if (result is null)
        {
            result = Instantiate(ingredientPrefab, ingredientParent);
            Gems.Add(result);
        }

        result.ResetPhysics();
        result.gameObject.SetActive(true);
        
        return result;
    }
    
    public void Return(Gem gem)
    {
        gem.gameObject.SetActive(false);
    }

    public void ClearAll()
    {
        foreach (var gem in Gems)
        {
            Return(gem);
        }
    }
}
