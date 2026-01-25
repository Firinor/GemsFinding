using System;
using UnityEngine;

public class Cheats : MonoBehaviour
{
   private ProgressData player;

   public void Initialize(MetaContext metaContext)
   {
      player = metaContext.Player;
   }

   [ContextMenu(nameof(AddGold))]
   private void AddGold()
   {
      player.AddGold(450);
   }
}
