using System;
using System.Collections.Generic;
using LoggerServer.Models;
using UnityEngine;
using UnityEngine.XR;

internal sealed class ControllerLogger
{
    private Session _session;
    private readonly List<MovementModel> _gesture = new List<MovementModel>();

    private static readonly InputDevice _hmd = InputDevices.GetDeviceAtXRNode(XRNode.Head);
    private const string HmdId = "hmd";

    private InputDevice _controller;
    private readonly string _controllerId;
    private readonly string _userId;

    public ControllerLogger(bool rightHand, string userId)
    {
        _controller = InputDevices.GetDeviceAtXRNode(rightHand ? XRNode.RightHand : XRNode.LeftHand);
        _controllerId = rightHand ? "controller-1" : "controller-2";
        _userId = userId;
    }

    public void TryLog(Action<List<MovementModel>> sendGestureAction)
    {
        var triggerPressed = IsTriggerDown();

        if (triggerPressed)
        {
            if (_session == null)
            {
                _session = new Session(!string.IsNullOrEmpty(_userId));
            }

            var timeStamp = _session.GetTimeStamp;
            TryLogDeviceData(_controller, _controllerId, timeStamp);
            TryLogDeviceData(_hmd, HmdId, timeStamp);
        }
        else if (_session != null)
        {
            _session = null;
            sendGestureAction(_gesture);
            _gesture.Clear();
        }
    }

    private bool IsTriggerDown()
    {
        return _controller.TryGetFeatureValue(CommonUsages.triggerButton, out var triggerPressed) && triggerPressed;
    }

    private void TryLogDeviceData(InputDevice device, string deviceId, double timeStamp)
    {
        var positionGetSuccess = device.TryGetFeatureValue(CommonUsages.deviceRotation, out var p);
        var rotationGetSuccess = device.TryGetFeatureValue(CommonUsages.deviceRotation, out var q);

        if (positionGetSuccess && rotationGetSuccess)
        {
            var r = q.eulerAngles.normalized;

            var part = new MovementModel
            {
                UserId = _userId,
                Timestamp = timeStamp,
                DeviceId = deviceId,
                SessionId = _session.Id,
                X = p.x,
                Y = p.y,
                Z = p.z,
                RotationX = r.x,
                RotationY = r.y,
                RotationZ = r.z,
                Roll = GetRoll(q),
                Pitch = GetPitch(q),
                Yaw = GetYaw(q)
            };

            _gesture.Add(part);
        }
    }

    private static float GetRoll(Quaternion q)
    {
        return CalculateClampedRotation(Mathf.Atan, 2 * (q.w * q.x + q.y * q.z) / (1 - 2 * (q.x * q.x + q.y * q.y)));
    }

    private static float GetPitch(Quaternion q)
    {
        return CalculateClampedRotation(Mathf.Asin, 2 * (q.w * q.y - q.z * q.x));
    }

    private static float GetYaw(Quaternion q)
    {
        return CalculateClampedRotation(Mathf.Atan, 2 * (q.w * q.z + q.x * q.y) / (1 - 2 * (q.y * q.y + q.z * q.z)));
    }

    private static float CalculateClampedRotation(Func<float, float> function, float body)
    {
        var result = function(body);

        return float.IsNaN(result) ?
            Mathf.Sign(body) * 90 :
            Mathf.Rad2Deg * result;
    }
}