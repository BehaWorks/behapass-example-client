using System.Collections.Generic;
using LoggerServer.Models;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovementsGenerator : MonoBehaviour
{
    public Material material;
    public float partMaxDurationInSeconds = 0.2f;
    public float partMaxSize = 1;
    public float gestureScale = 5f;
    public int countToGenerate = 25;

    private GameObject _holder;
    private TrailRenderer _trail;
    private List<MovementModel> _movements;
    private int _index;
    private float _partCurrentDurationInSeconds;

    private void Update()
    {
        if (_index == 0)
        {
            Destroy(_holder);

            _movements = GenerateMovements();
            _holder = new GameObject("Simulations Bundle");
            _holder.transform.parent = transform;
            _partCurrentDurationInSeconds = partMaxDurationInSeconds;

            _trail = _holder.AddComponent<TrailRenderer>();
            _trail.material = material;

            _trail.widthMultiplier = 0.15f;
            _trail.time = 0.1f;
        }

        var pos = transform.position;
        var cam = Camera.main.transform.position;

        var from = _movements[_index];
        var fromPosition = new Vector3((float)from.X + pos.x, (float)from.Y + cam.y, (float)from.Z);

        var to = _movements[_index + 1];
        var toPosition = new Vector3((float)to.X + pos.x, (float)to.Y + cam.y, (float)to.Z);

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

            _index = (_index + 1) % (_movements.Count - 1);
            _partCurrentDurationInSeconds = 0;
        }

        _partCurrentDurationInSeconds += Time.deltaTime;
    }

    private Color ColorFromPosition(Vector3 position)
    {
        var pos = transform.position;
        var cam = Camera.main.transform.position;

        return new Color(
            Mathf.Lerp(-gestureScale / 2, gestureScale / 2, (position.x - pos.x) / gestureScale / 2 + 0.5f),
            Mathf.Lerp(-gestureScale / 2, gestureScale / 2, (position.y - cam.y) / gestureScale / 2 + 0.5f),
            Mathf.Lerp(-gestureScale / 2, gestureScale / 2, position.z / gestureScale / 2 + 0.5f));
    }

    private List<MovementModel> GenerateMovements()
    {
        var movements = new List<MovementModel>();

        var firstMovement = new MovementModel
        {
            X = Random.Range(-gestureScale / 2, gestureScale / 2),
            Y = Random.Range(-gestureScale / 2, gestureScale / 2),
            Z = Random.Range(-gestureScale / 2, gestureScale / 2)
        };
        movements.Add(firstMovement);

        for (var index = 1; index < countToGenerate; index++)
        {
            var previousMovement = movements[index - 1];

            var newMovement = new MovementModel
            {
                X = Mathf.Clamp((float)previousMovement.X + Random.Range(-partMaxSize / 2, partMaxSize / 2), -gestureScale / 2, gestureScale / 2),
                Y = Mathf.Clamp((float)previousMovement.Y + Random.Range(-partMaxSize / 2, partMaxSize / 2), -gestureScale / 2, gestureScale / 2),
                Z = Mathf.Clamp((float)previousMovement.Z + Random.Range(-partMaxSize / 2, partMaxSize / 2), -gestureScale / 2, gestureScale / 2)
            };

            movements.Add(newMovement);
        }

        return movements;
    }
}
