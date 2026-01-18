using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
   [SerializeField]
   private AudioConfig config;
   [SerializeField]
   private List<AudioSource> audioPool;

   public void PlayGemTink(Vector3 position = default)
   {
      AudioSource source = audioPool.FirstOrDefault(a => !a.gameObject.activeSelf);

      if(source is null)
         return;
      
      source.gameObject.SetActive(true);
      source.transform.position = position;
      source.pitch = 1 + Random.Range(-0.05f, 0.05f);
      source.volume = config.gemTink.Volume;
      
      source.PlayOneShot(config.gemTink.Clip);

      StartCoroutine(DisableAudioSource(source));
   }

   private IEnumerator DisableAudioSource(AudioSource source)
   {
      while (source.isPlaying)
      {
         yield return null;
      }
      
      source.gameObject.SetActive(false);
   }
}
