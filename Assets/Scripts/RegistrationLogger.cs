using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LoggerServer;
using LoggerServer.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class RegistrationLogger : MonoBehaviourWithPrint
{
    public double shortestGestureMiliseconds = 200;

    private ControllerLogger _rightController;
    private ControllerLogger _leftController;

    private string _userName;
    private string _userId;
    private bool _finished;

    private void Start()
    {
        _printToGui = _printToConsole = true;

        Print("Loading loggers.");
        var loggersLoadSuccess = LoadLoggers();

        if (!loggersLoadSuccess)
        {
            Print("Loggers loading failed.", LogType.Error);
            _finished = true;
            return;
        }

        Print("Loggers loaded successfully.");
        Print($"Loggers started for {_userName}.");
    }

    private bool LoadLoggers()
    {
        var config = FindObjectOfType<RegistrationConfig>();

        SceneManager.MoveGameObjectToScene(config.gameObject, SceneManager.GetActiveScene());

        _userName = config?.userName;
        _userId = config?.userId;

        if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_userId))
        {
            return false;
        }

        _rightController = new ControllerLogger(true, _userId);
        _leftController = new ControllerLogger(false, _userId);

        return true;
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
        Print("Gesture capturing finished.");

        if (gestureToSend.Count < 1 && gestureToSend.Last().Timestamp < shortestGestureMiliseconds)
        {
            Print($"Gesture is too short. Please try again and hold trigger longer than {shortestGestureMiliseconds}ms.", LogType.Warning);

            return;
        }

        Print("Sending gesture to server.");

        var response = LoggerServerAPI.PostUserMovements(
            _userId,
            gestureToSend,
            new Dictionary<HttpStatusCode, Func<bool>>
            {
                {
                    HttpStatusCode.Accepted,
                    () => true
                },
                {
                    HttpStatusCode.OK,
                    () =>
                    {
                        _finished = true;
                        return true;
                    }
                }
            },
            exception => Print(exception.Message, LogType.Error));

        if (response == null)
        {
            return;
        }

        Print($"Gesture sent successfully for {_userName}.");
        Print(
            _finished ?
                "Server has enough gestures, logging finished successfully." :
                "Please send more gestures.");
    }
}