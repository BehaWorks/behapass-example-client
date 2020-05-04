using System;
using System.Collections.Generic;
using System.Linq;
using LoggerServer;
using LoggerServer.Models;
using UnityEngine;
using Random = UnityEngine.Random;

internal class UserGestures
{
    public readonly string UserId;

    public List<MovementModel> CurrentGesture
    {
        get
        {
            try
            {
                return _gestures[_currentGestureIndex++];
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    private int _currentGestureIndex;

    private readonly List<List<MovementModel>> _gestures;

    public UserGestures(string userId)
    {
        UserId = userId;
        _gestures = LoadGestures(userId);
    }

    private static List<List<MovementModel>> LoadGestures(string userId)
    {
        var movements = LoggerServerAPI.GetUserMovements(
            userId,
            null,
            exception => Debug.LogError(exception.Message));

        if (movements != null)
        {
            return movements
                .GroupBy(movement => movement.SessionId)
                .Select(group => group.ToList())
                .OrderBy(gesture => Random.Range(0f, 1f))
                .ToList();
        }

        return new List<List<MovementModel>>();
    }
}