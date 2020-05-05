using System;
using System.Collections.Generic;
using System.Net;
using LoggerServer;
using LoggerServer.Models;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class LoginLogger : MonoBehaviourWithPrint
{
    public Canvas menuHolder;
    public TextMeshProUGUI messageHolder;
    public Button continueButton;

    private ControllerLogger _rightController;
    private ControllerLogger _leftController;

    private bool _finished;

    private void Start()
    {
        _printToGui = _printToConsole = true;

        Print("Loading loggers.");
        LoadLoggers();

        Print("Loggers loaded successfully.");
        Print("Loggers started.");
    }

    private void LoadLoggers()
    {
        _rightController = new ControllerLogger(true, null);
        _leftController = new ControllerLogger(false, null);
    }

    private void Update()
    {
        if (!_finished)
        {
            _rightController.TryLog(TrySendToServer);
        }
        if (!_finished)
        {
            _leftController.TryLog(TrySendToServer);
        }
    }

    private void TrySendToServer(List<MovementModel> gestureToSend)
    {
        Print($"Gesture capturing finished.{Environment.NewLine}Sending gesture to server.");

        var model = new LoggerModel { Movements = gestureToSend, Buttons = new List<ButtonModel>() };

        var response = LoggerServerAPI.Lookup(
            model,
            new Dictionary<HttpStatusCode, Func<bool>>
            {
                {
                    HttpStatusCode.OK,
                    () =>
                    {
                        _finished = true;
                        return true;
                    }
                },
                {
                    HttpStatusCode.NotFound,
                    () =>
                    {
                        ShowMenu("User not found.", false);
                        return false;
                    }
                }
            },
            exception => Print(exception.Message, LogType.Error));

        if (response == null)
        {
            return;
        }

        ShowMenu($"Welcome user with ID {response.UserId}.", true);
    }

    private void ShowMenu(string message, bool success)
    {
        menuHolder.gameObject.SetActive(true);
        messageHolder.text = message;
        continueButton.gameObject.SetActive(success);
    }

    public void Repeat()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RegistrationScene()
    {
        SceneManager.LoadScene("Registration Keyboard");
    }

    public void Continue()
    {
        Print("Not implemented yet!"); // TODO
    }
}