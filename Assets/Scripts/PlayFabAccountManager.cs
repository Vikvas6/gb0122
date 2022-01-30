using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayFabAccountManager : MonoBehaviour
{  
    [SerializeField] private Text _titleLabel;
    private const string AuthGuidKey = "authorization-guid";

    private void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnFailure);
    }

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        _titleLabel.text = $"Welcome back, Player ID {result.AccountInfo.PlayFabId}\nYour user was created {(DateTime.Now - result.AccountInfo.Created).Days} days ago on {result.AccountInfo.Created}";
    }

    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
    }

    public void ForgetPlayerPref()
    {
        if (PlayerPrefs.HasKey(AuthGuidKey))
        {
            PlayerPrefs.DeleteKey(AuthGuidKey);
        }

        SceneManager.LoadScene(0);
    }
}
