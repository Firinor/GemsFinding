using System;
using System.Linq;
using MirraGames.SDK;
using MirraGames.SDK.Common;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ADSManager : MonoBehaviour
{
    public float adsInterval = 60;
    private static float timer = 20;
    private bool isReadyForAds;
    private bool isNoAds;

    public Button NoAds;

    public void Initialize()
    {
        MirraSDK.Payments.RestorePurchases((restoreData) =>
        {
            string[] allPurchases = restoreData.AllPurchases;

            string noAds = allPurchases.FirstOrDefault(x=> x == "NoAds");

            if (!string.IsNullOrEmpty(noAds))
            {
                InputSystem.actions.FindAction("Click").performed -= ShowAdsInterstitial;
                if(MirraSDK.Ads.IsBannerVisible)
                    MirraSDK.Ads.DisableBanner();
                enabled = false;
                isNoAds = true;
                Destroy(NoAds.gameObject);
                Destroy(gameObject);
            }
        });
        
        if(!enabled)
            return;
        
        ProductData productData = MirraSDK.Payments.GetProductData("NoAds");
        
        NoAds.onClick.AddListener(PurchaseNoAds);
        NoAds.GetComponentInChildren<TextMeshProUGUI>().text = productData.GetFullPriceFloat();
        
        isReadyForAds = false;
    }

    private void PurchaseNoAds()
    {
        MirraSDK.Payments.Purchase(
            productTag: "NoAds",
            onSuccess: () => {
                InputSystem.actions.FindAction("Click").performed -= ShowAdsInterstitial;
                if(MirraSDK.Ads.IsBannerVisible)
                    MirraSDK.Ads.DisableBanner();
                enabled = false;
                isNoAds = true;
                Destroy(NoAds.gameObject);
                Destroy(gameObject);
            }
        );
    }

    /*void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && !isReadyForAds)
        {
            isReadyForAds = true;
            InputSystem.actions.FindAction("Click").performed += ShowAdsInterstitial;
        }
    }*/

    private void OnApplicationPause(bool pauseStatus)
    {
        if(!pauseStatus)
            MirraSDK.Time.Scale = 1f;
    }

    public void ShowAdsInterstitial(Action callback = null)
    {
        isReadyForAds = false;
        
        if (MirraSDK.Ads.IsInterstitialVisible)
        {
            return;
        }
        
        if (!(MirraSDK.Ads.IsInterstitialReady
              && MirraSDK.Ads.IsInterstitialAvailable))
        {
            callback?.Invoke();
            return;
        }
        
        if(isNoAds)
        {
            callback?.Invoke();
            return;
        }
        
        MirraSDK.Time.Scale = 0f;
        MirraSDK.Ads.InvokeInterstitial(
            onClose: (isSuccess) =>
            {
                MirraSDK.Time.Scale = 1f;
                timer = adsInterval;
                callback?.Invoke();
            });
    }

    private void ShowAdsInterstitial(InputAction.CallbackContext obj)
    {
        InputSystem.actions.FindAction("Click").performed -= ShowAdsInterstitial;

        ShowAdsInterstitial();
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Click").performed -= ShowAdsInterstitial;
    }

    private void OnDestroy()
    {
        isReadyForAds = false;
        InputSystem.actions.FindAction("Click").performed -= ShowAdsInterstitial;
    }

    public void DeleteAllSaves()
    {
        MirraSDK.Data.DeleteAll();
    }
}
