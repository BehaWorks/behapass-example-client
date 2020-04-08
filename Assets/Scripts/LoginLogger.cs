using System;
using System.Collections.Generic;
using System.Net;
using LoggerServer;
using LoggerServer.Models;
using UnityEngine;

public sealed class LoginLogger : MonoBehaviourWithPrint
{
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

        var response = LoggerServerAPI.Lookup(
            gestureToSend,
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
                        Print("User not found.");
                        return false;
                    }
                }
            },
            exception => Print(exception.Message, LogType.Error));

        if (response == null)
        {
            return;
        }

        Print($"Welcome user with ID {response.UserId}.");
    }
}