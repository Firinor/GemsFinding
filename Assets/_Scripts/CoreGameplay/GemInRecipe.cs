using System;
using UnityEngine;
using UnityEngine.UI;

public class GemInRecipe : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetView(Sprite sprite, Color color, Material material = null)
    {
        image.sprite = sprite;
        image.SetNativeSize();
        image.color = color;
        image.raycastTarget = false;

        if (material is not null)
            image.material = material;

        enabled = false;
    }
    
    public static bool operator ==(GemInRecipe gemInRecipe, GemData gem)
    {
        if (gemInRecipe is null)
            return false;

        bool isSprite = gemInRecipe.image.sprite == gem.Sprite;
        bool isColor = gemInRecipe.image.color == gem.Color;
        bool isMaterial = gemInRecipe.image.material == gem.Material;
        
        return isSprite && isColor;
    }
    public static bool operator !=(GemInRecipe gemInRecipe, GemData gem) => !(gemInRecipe == gem);
    private bool Equals(GemInRecipe other) => Equals(image.sprite, other.image.sprite) && image.color.Equals(other.image.color) && Equals(image.material, other.image.material);
    public override bool Equals(object obj) => Equals((GemInRecipe)obj);
    public override int GetHashCode() => HashCode.Combine(image.sprite, image.color, image.material);
}