using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.EditorModels;
using UnityEngine;
using UnityEngine.UI;
using LoginResult = PlayFab.ClientModels.LoginResult;
using PlayFabError = PlayFab.PlayFabError;


public class PlayFabLogin : MonoBehaviour
{
    [SerializeField] private Button _playFabLoginButton;
    [SerializeField] private Button _playFabLogoutButton;
    [SerializeField] private Text _infoText;

    private string _userId = "GeekBrainsLesson3";

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "FE6D6";
        }

        _playFabLoginButton.onClick.AddListener(LogIn);
    }

    private void LogIn()
    {
        var request = new LoginWithCustomIDRequest { CustomId = _userId, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made successful API call!");
        _infoText.text = "PlayFab: success login!";
        _infoText.color = Color.green;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
        
        _infoText.text = "PlayFab: error login!\n" + error;
        _infoText.color = Color.red;
    }
}
