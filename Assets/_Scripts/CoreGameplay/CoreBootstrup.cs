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
    private bool isDebugMode;
    [SerializeField]
    private Stats stats;

    private static bool isSkipStartAnimations;
    
    private ProgressData player;

    private void Awake()
    {
        MirraSDK.WaitForProviders(AwakeAfterMirra);
    }

    private void AwakeAfterMirra()
    {
        settings.Initialize();
        LoadPlayerData(out player);
        if (isSkipStartAnimations)
        {
            
            mainManager.Initialize(player);
        }
        else
        {
            StartAnimation.enabled = true;
        }
    }
    
    public void StartPuzzle()
    {
        isSkipStartAnimations = true;
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
