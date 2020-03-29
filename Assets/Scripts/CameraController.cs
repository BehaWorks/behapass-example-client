using UnityEngine;

public class CameraController : MonoBehaviourWithPrint
{
    public float moveSensitivity = 0.02f;
    public float rotateSensitivity = 3;
    public float zoomSensitivity = 5;
    public bool showInstructions;

    private bool _isEnabled = true;
    private Transform _transform;

    private void Start()
    {
        _printToConsole = false;
        _printToGui = showInstructions;
        _transform = transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            _isEnabled = !_isEnabled;
        }

        if (_isEnabled)
        {
            var up = _transform.up;
            var right = _transform.right;

            TryMove(KeyCode.W, up);
            TryMove(KeyCode.A, -right);
            TryMove(KeyCode.S, -up);
            TryMove(KeyCode.D, right);

            TryRotate();

            TryZoom();
        }
    }

    private void TryMove(KeyCode keyCode, Vector3 direction)
    {
        if (Input.GetKey(keyCode))
        {
            transform.position += direction * moveSensitivity;
        }
    }

    private void TryRotate()
    {
        var forward = _transform.forward;

        TryMove(KeyCode.E, forward);
        TryMove(KeyCode.Q, -forward);

        if (Input.GetMouseButton(2))
        {
            transform.Rotate(Input.GetAxis("Mouse X") * rotateSensitivity * -_transform.up);
            transform.Rotate(Input.GetAxis("Mouse Y") * rotateSensitivity * _transform.right);
        }
    }

    private void TryZoom()
    {
        var mouseScrollWheelAxisChange = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(mouseScrollWheelAxisChange) > 0)
        {
            _transform.position += mouseScrollWheelAxisChange * zoomSensitivity * transform.forward;
        }
    }
}