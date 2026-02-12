using UnityEngine;

public class GemBuilder
{
    private Gem gem;

    public GemBuilder New(Gem gem)
    {
        this.gem = gem;
        return this;
    }

    public GemBuilder SetView(Sprite sprite, Color color)
    {
        gem.SetView(sprite, color);
        return this;
    }
    
    public GemBuilder NoGem()
    {
        gem.Sprite.enabled = false;
        return this.NoTail().NoLight2D();
    }
    public GemBuilder NoDirt()
    {
        gem.RemoveDirt();
        return this;
    }

    public GemBuilder NoTail()
    {
        gem.RemoveTail();
        return this;
    }
    public GemBuilder NoLight2D()
    {
        gem.RemoveLight2D();
        return this;
    }
    
    public Gem Build()
    {
        return gem;
    }
}