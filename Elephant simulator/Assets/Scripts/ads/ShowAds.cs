using System;
using UnityEngine;

public class ShowAds : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;
    [SerializeField]
    private Transform playerTransform;
    public void ShowBannerAd()
    {
        AdManager.Instance.ShowBanner();
    }

    public void HideBannerAd()
    {
        AdManager.Instance.HideBanner();
    }
    public void ShowInterstitialAd()
    {
        AdManager.Instance.ShowInterstitial();
    }
    public void ShowRewardedAd()
    {
        AdManager.Instance.ShowRewarded(() =>
        {
            Debug.Log("Reward granted");

            playerTransform = respawnPoint;

            Input.Instance.caught = false;

            GameManager.Instance.HideEndScreen();


        });
    }
}
