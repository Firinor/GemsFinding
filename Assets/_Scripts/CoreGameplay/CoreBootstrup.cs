using System.Collections;
using MirraGames.SDK;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class CoreBootstrup : MonoBehaviour
{
    [SerializeField]
    private Animation StartAnimation;
    
    [SerializeField] 
    private Settings settings;
    [SerializeField] 
    private FindObjectManager mainManager;
    [SerializeField] 
    private ADSManager adsManager;
    [SerializeField] 
    private GameObject startBlackScreen;

    [SerializeField]
    private bool isDebugMode;
    [SerializeField]
    private Stats stats;

    private static bool isSkipStartAnimations;
    
    private ProgressData player;

    private void Awake()
    {
        if (isSkipStartAnimations)
        {
            AwakeAfterMirra();
            mainManager.Initialize(player);
        }
        else
        {
            StartAnimation.enabled = true;
            if(MirraSDK.IsInitialized)
                AwakeAfterMirra();
            else
                MirraSDK.WaitForProviders(AwakeAfterMirra);
        }
    }

    private void AwakeAfterMirra()
    {
        settings.Initialize();
        LoadPlayerData(out player);
        adsManager.Initialize();
        startBlackScreen.SetActive(false);
    }
    
    public void StartPuzzle()
    {
        isSkipStartAnimations = true;
        if(MirraSDK.IsInitialized)
            StartCoroutine(AwaitPlayer());
        else
            MirraSDK.WaitForProviders(()=>StartCoroutine(AwaitPlayer()));
    }

    private IEnumerator AwaitPlayer()
    {
        while (player is null)
        {
            yield return null;
        }
        mainManager.Initialize(player);
    }
    
    private void LoadPlayerData(out ProgressData data)
    {
#if UNITY_EDITOR
        if (isDebugMode)
        {
            data = new()
            {
                Stats = stats,
            };
            data.Stats.isDebug = true;
            return;
        }
#endif
        data = MirraSaveLoadSystem<ProgressData>.Load("Player", Default: new()
        {
            Stats = new(),
        });
        data.Stats.isDebug = false;
    }
}
