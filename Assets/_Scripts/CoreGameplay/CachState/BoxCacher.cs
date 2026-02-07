using UnityEngine;

public class BoxCacher : MonoBehaviour
{
    [SerializeField]
    private GemBox box;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Gem gem = other.GetComponent<Gem>(); 
        if(gem is null)
            return;

        bool fullBox = box.AddGem(gem);
        
        if(fullBox)
            return;
        
        box.OnMove += gem.BoxMove;
        Rigidbody2D rigidbody2D = gem.GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
        rigidbody2D.linearDamping = .8f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Gem gem = other.GetComponent<Gem>(); 
        if(gem is null)
            return;
        
        box.RemoveGem(gem);
        box.OnMove -= gem.BoxMove;
        Rigidbody2D rigidbody2D = gem.GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 1;
        rigidbody2D.linearDamping = .1f;
    }
}
