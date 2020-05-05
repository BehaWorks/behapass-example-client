using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LoggerServer;
using LoggerServer.Models;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoginSimulation : MonoBehaviourWithPrint
{
    public Canvas menuHolder;
    public TextMeshProUGUI messageHolder;
    public Button continueButton;

    public Material material;
    public float partMaxDurationInSeconds = 1f;
    public float gestureScale = 5f;

    private static readonly string[] _userNames =
    {
        "h_matusk",
        "h_andrej",
        "h_ada",
        "h_zuzka",
        "h_martin",
        "h_vilo",
        "h_matus",
        "h_lukas",
        "h_janka"
    };

    private int _currentUserIndex;

    private GameObject _holder;
    private TrailRenderer _trail;
    private List<MovementModel> _allMovements;
    private List<MovementModel> _movements;
    private int _movementIndex;
    private float _partCurrentDurationInSeconds;

    private bool _simulating;
    private bool _finished = true;

    private readonly Dictionary<string, UserGestures> _userGestures = new Dictionary<string, UserGestures>();

    private void Start()
    {
        Repeat();
    }

    private void Update()
    {
        var currentUserName = _userNames[_currentUserIndex];

        if (_finished && Input.GetKeyDown(KeyCode.Mouse0))
        {
            _movements = _userGestures[currentUserName].CurrentGesture;

            if (_movements != null)
            {
                _allMovements = _movements.Where(movement => movement.X != 0 || movement.Y != 0 || movement.Z != 0).ToList();
                _movements = _allMovements.Where(movement => movement.DeviceId != "hmd").ToList();

                _holder = new GameObject("Simulations Bundle");
                _holder.transform.parent = transform;
                _partCurrentDurationInSeconds = partMaxDurationInSeconds;

                _trail = _holder.AddComponent<TrailRenderer>();
                _trail.material = material;

                _trail.widthMultiplier = 0.15f;
                _trail.time = 0.1f;

                _simulating = true;
                _finished = false;
            }
        }

        if (_simulating && (_movements == null || _movementIndex >= _movements.Count - 2))
        {
            TrySendToServer(_allMovements);
            Destroy(_holder);
            _simulating = false;
            _movementIndex = 0;
            return;
        }

        if (_simulating && _movements != null)
        {
            var from = _movements[_movementIndex];
            var fromPosition = new Vector3((float)from.X, (float)from.Y, (float)from.Z);

            var to = _movements[_movementIndex + 1];
            var toPosition = new Vector3((float)to.X, (float)to.Y, (float)to.Z);

            var currentPosition = _holder.transform.position = Vector3.Lerp(
                fromPosition,
                toPosition,
                _partCurrentDurationInSeconds / partMaxDurationInSeconds);

            _trail.startColor = ColorFromPosition(currentPosition);
            _trail.endColor = ColorFromPosition(currentPosition);

            if (_partCurrentDurationInSeconds >= partMaxDurationInSeconds)
            {
                var lineHolder = new GameObject($"Line from {from.X} | {from.Y} | {from.Z}");
                lineHolder.transform.parent = _holder.transform;

                var line = lineHolder.AddComponent<LineRenderer>();
                line.material = material;

                var positions = new[] { fromPosition, toPosition };
                line.SetPositions(positions);

                line.widthMultiplier = 0.1f;
                line.startColor = ColorFromPosition(fromPosition);
                line.endColor = ColorFromPosition(toPosition);

                _movementIndex++;
                _partCurrentDurationInSeconds = 0;
            }

            _partCurrentDurationInSeconds += Time.deltaTime;
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
                    () => true
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

    private void TryDownloadUserGestures(string userName)
    {
        if (!_userGestures.ContainsKey(userName))
        {
            _userGestures.Add(userName, new UserGestures(userName));
        }
    }

    private Color ColorFromPosition(Vector3 position)
    {
        return new Color(
            Mathf.Lerp(-gestureScale / 2, gestureScale / 2, position.x / gestureScale / 2 + 0.5f),
            Mathf.Lerp(-gestureScale / 2, gestureScale / 2, position.y / gestureScale / 2 + 0.5f),
            Mathf.Lerp(-gestureScale / 2, gestureScale / 2, position.z / gestureScale / 2 + 0.5f));
    }

    private void ShowMenu(string message, bool success)
    {
        menuHolder.gameObject.SetActive(true);
        messageHolder.text = message;
        continueButton.gameObject.SetActive(success);
    }

    public void Repeat()
    {
        _currentUserIndex = Random.Range(0, _userNames.Length);
        TryDownloadUserGestures(_userNames[_currentUserIndex]);
        _finished = true;
        menuHolder.gameObject.SetActive(false);
    }

    public void RegistrationScene()
    {
        SceneManager.LoadScene("Registration Keyboard");
    }

    public void Continue()
    {
        ShowMenu("Not implemented yet!", false); // TODO
    }
}
