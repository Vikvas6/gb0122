using System;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LoginResult = PlayFab.ClientModels.LoginResult;
using PlayFabError = PlayFab.PlayFabError;


public class PlayFabLogin : MonoBehaviour
{
    [SerializeField] private Text _createErrorLabel;
    [SerializeField] private Text _signInErrorLabel;
    [SerializeField] private GameObject _loadingIndicator;
    
    private string _username;
    private string _mail;
    private string _pass;

    private string _userId = "GeekBrainsLesson3";
    private const string AuthGuidKey = "authorization-guid";
    private Coroutine _loadingIndicatorRoutine;
    
    public void UpdateUsername(string username)
    {
        _username = username;
    }

    public void UpdateEmail(string mail)
    {
        _mail = mail;
    }

    public void UpdatePassword(string pass)
    {
        _pass = pass;
    }

    public void CreateAccount()
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _username,
            Email = _mail,
            Password = _pass,
            RequireBothUsernameAndEmail = true
        }, OnCreateSuccess, OnFailure);
        ShowLoadingIndicator();
    }
    
    public void SignIn()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _username,
            Password = _pass
        }, OnSignInSuccess, OnFailure);
        ShowLoadingIndicator();
    }


    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "FE6D6";
        }

        var needCreation = PlayerPrefs.HasKey(AuthGuidKey);
        var id = PlayerPrefs.GetString(AuthGuidKey, Guid.NewGuid().ToString());
        
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = id,
            CreateAccount = !needCreation
        }, success =>
        {
            PlayerPrefs.SetString(AuthGuidKey, id);
            SceneManager.LoadScene(1);
        }, OnFailure);
        ShowLoadingIndicator();
    }

    private void OnCreateSuccess(RegisterPlayFabUserResult result)
    {
        StopShowLoadingIndicator();
        Debug.Log($"Creation Success: {_username}");
        SceneManager.LoadScene(1);
    }

    private void OnSignInSuccess(LoginResult result)
    {
        StopShowLoadingIndicator();
        Debug.Log($"Sign In Success: {_username}");
        SceneManager.LoadScene(1);
    }

    private void OnFailure(PlayFabError error)
    {
        StopShowLoadingIndicator();
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
        _createErrorLabel.text = errorMessage;
        _signInErrorLabel.text = errorMessage;
    }
    
    public void Back()
    {
        _createErrorLabel.text = "";
        _signInErrorLabel.text = "";
    }

    private void ShowLoadingIndicator()
    {
        _loadingIndicator.SetActive(true);
        _loadingIndicatorRoutine = StartCoroutine(LoadingIndicator());
    }
    
    private void StopShowLoadingIndicator()
    {
        StopCoroutine(_loadingIndicatorRoutine);
        _loadingIndicator.SetActive(false);
    }

    IEnumerator LoadingIndicator()
    {
        while (true)
        {
            _loadingIndicator.transform.Rotate(Vector3.forward, -4.0f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
