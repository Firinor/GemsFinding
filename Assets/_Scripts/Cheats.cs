using UnityEngine;

public class Cheats : MonoBehaviour
{
   private ProgressData player;

   public void Initialize(ProgressData player)
   {
      this.player = player;
   }

   [ContextMenu(nameof(AddGold))]
   public void AddGold()
   {
      player.AddGold(9999);
   }
}
