using UnityEngine;

public class Cheats : MonoBehaviour
{
   private ProgressData player;

   public void Initialize(MetaContext metaContext)
   {
      player = metaContext.Player;
   }

   [ContextMenu(nameof(AddGold))]
   public void AddGold()
   {
      player.AddGold(9999);
   }
}
