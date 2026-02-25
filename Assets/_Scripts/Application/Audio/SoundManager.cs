using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
   public static SoundManager Instance;
   [SerializeField]
   private AudioConfig config;
   [SerializeField]
   private List<AudioSource> audioPool;

   private void Awake()
   {
      Instance = this;
   }

   public void PlayVictory(Vector3 position = default)
   {
      Play(position, config.victory, isPriority: true);
   }
   public void PlayErrorGem(Vector3 position = default)
   {
      Play(position, config.errorGem, isPriority: true);
   }
   public void PlayCurrectGem(Vector3 position = default)
   {
      Play(position, config.correctGem, isPriority: true);
   }
   
   public void PlayButtonClick(Vector3 position = default)
   {
      Play(position, config.buttonClick, isPriority: true);
   }
   
   public void PlayGemTink(Vector3 position = default)
   {
      Play(position, config.gemTink);
   }

   private void Play(Vector3 position, ClipSettings clipData, bool isPriority = false)
   {
      AudioSource source = audioPool.FirstOrDefault(a => !a.gameObject.activeSelf);

      if (source is null)
      {
         if(!isPriority)
            return;
         source = audioPool[0];
      }
      
      source.gameObject.SetActive(true);
      source.transform.position = position;
      source.pitch = 1 + Random.Range(-0.05f, 0.05f);
      source.volume = clipData.Volume;
      
      source.PlayOneShot(clipData.Clip);

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
