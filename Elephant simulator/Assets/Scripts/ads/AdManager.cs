using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private bool initialized = false;

    // Test IDs
    private string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";

    private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";

    private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

    //original IDs
    //private string bannerAdUnitId = "ca-app-pub-1216562962219777/2593949295";

    //private string interstitialAdUnitId = "ca-app-pub-1216562962219777/7009007887";

    //private string rewardedAdUnitId = "ca-app-pub-1216562962219777/8556722416";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeAds();

        ShowBanner();   
    }

    private void InitializeAds()
    {
        if (initialized) return;

        MobileAds.Initialize(initStatus =>
        {
            initialized = true;

            LoadBannerAd();
            LoadInterstitialAd();
            LoadRewardedAd();

            Debug.Log("AdMob Initialized");
        });
    }

    #region Banner

    public void LoadBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        bannerView = new BannerView(
            bannerAdUnitId,
            AdSize.Banner,
            AdPosition.Bottom);

        bannerView.LoadAd(new AdRequest());
    }

    public void ShowBanner()
    {
        if (bannerView != null)
            bannerView.Show();
    }

    public void HideBanner()
    {
        if (bannerView != null)
            bannerView.Hide();
    }

    #endregion

    #region Interstitial

    public void LoadInterstitialAd()
    {
        InterstitialAd.Load(
            interstitialAdUnitId,
            new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.Log("Interstitial load failed");
                    return;
                }

                interstitialAd = ad;
                Debug.Log("Interstitial loaded");
            });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null &&
            interstitialAd.CanShowAd())
        {
            interstitialAd.Show();

            interstitialAd = null;

            LoadInterstitialAd();
        }
        else
        {
            Debug.Log("Interstitial not ready");
        }
    }

    #endregion

    #region Rewarded

    public void LoadRewardedAd()
    {
        RewardedAd.Load(
            rewardedAdUnitId,
            new AdRequest(),
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.Log("Rewarded load failed");
                    return;
                }

                rewardedAd = ad;
                Debug.Log("Rewarded loaded");
            });
    }

    public void ShowRewarded(System.Action onRewardEarned)
    {
        if (rewardedAd != null &&
            rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                onRewardEarned?.Invoke();
            });

            rewardedAd = null;

            LoadRewardedAd();
        }
        else
        {
            Debug.Log("Rewarded ad not ready");
        }
    }

    #endregion
}