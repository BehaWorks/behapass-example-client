using UnityEngine;
using System.Collections;
using LoggerServer;
using LoggerServer.Models;
using UnityEngine.SceneManagement;
using VRKeys;

public class RegistrationKeyboard : MonoBehaviour
{
    public Keyboard keyboard;

    private void Update()
    {
        var shifted = Input.GetKey(KeyCode.LeftShift);

        for (var keyCode = KeyCode.A; keyCode <= KeyCode.Z; keyCode++)
        {
            if (Input.GetKeyDown(keyCode))
            {
                var character = keyCode.ToString();

                keyboard.AddCharacter(shifted ? character.ToUpper() : character.ToLower());
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            HandleSubmit(keyboard.text);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            keyboard.SetText("");
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            keyboard.Backspace();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            keyboard.AddCharacter(" ");
        }
    }

    private void OnEnable()
    {
        var canvas = keyboard.canvas.GetComponent<Canvas>();
        canvas.worldCamera = canvas.worldCamera ?? Camera.main;

        keyboard.Enable();
        keyboard.SetPlaceholderMessage("Please enter your user name");

        keyboard.OnUpdate.AddListener(HandleUpdate);
        keyboard.OnSubmit.AddListener(HandleSubmit);
    }

    private void OnDisable()
    {
        keyboard.OnUpdate.RemoveListener(HandleUpdate);
        keyboard.OnSubmit.RemoveListener(HandleSubmit);

        keyboard.Disable();
    }

    public void HandleUpdate(string text)
    {
        keyboard.HideValidationMessage();
    }

    public void HandleSubmit(string userName)
    {
        keyboard.DisableInput();

        if (string.IsNullOrEmpty(userName))
        {
            keyboard.ShowValidationMessage("Please enter your user name");
            keyboard.EnableInput();
            return;
        }

        StartCoroutine(SubmitUserName(userName));
    }

    private IEnumerator SubmitUserName(string userName)
    {
        var response = LoggerServerAPI.PostUser(new UserRequestModel { UserName = userName },
            code =>
            {
                keyboard.ShowInfoMessage($"Server returned: {code}");
                return true;
            },
            exception => keyboard.ShowValidationMessage(exception.Message));

        if (response == null)
        {
            yield break;
        }

        var configHolder = new GameObject("Registration Config");
        var config = configHolder.AddComponent<RegistrationConfig>();

        config.userName = userName;
        config.userId = response.UserId;

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Registration Logger");
    }
}