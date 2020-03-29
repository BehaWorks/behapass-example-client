using UnityEngine;

public abstract class MonoBehaviourWithPrint : MonoBehaviour
{
    protected bool _printToConsole = true;
    protected bool _printToGui = false;

    private string _guiText;
    private Color _guiColor;

    protected virtual string LogPrefix
    {
        get { return gameObject.name; }
    }

    protected void Print(string message, LogType type = LogType.Log)
    {
        if (_printToConsole)
        {
            var fullMessage = $"{LogPrefix}: {message}";

            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                    {
                        Debug.LogError(fullMessage, gameObject);
                    }
                    break;
                case LogType.Assert:
                    {
                        Debug.LogAssertion(fullMessage, gameObject);
                    }
                    break;
                case LogType.Warning:
                    {
                        Debug.LogWarning(fullMessage, gameObject);
                    }
                    break;
                case LogType.Log:
                    {
                        Debug.Log(fullMessage, gameObject);
                    }
                    break;
            }
        }

        if (_printToGui)
        {
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                    {
                        _guiColor = Color.red;
                    }
                    break;
                case LogType.Assert:
                    {
                        _guiColor = Color.blue;
                    }
                    break;
                case LogType.Warning:
                    {
                        _guiColor = Color.yellow;
                    }
                    break;
                case LogType.Log:
                    {
                        _guiColor = Color.green;
                    }
                    break;
            }

            _guiText = message;
        }
    }

    private void OnGUI()
    {
        if (_printToGui)
        {
            PrintToGui(new Rect(10.0f, 10.0f, 600.0f, 400.0f));
        }
    }

    private void PrintToGui(Rect rect, int strength = 2, int fontSize = 20)
    {
        var style = new GUIStyle { fontSize = fontSize, normal = { textColor = new Color(0, 0, 0, 0.3f) } };
        int i;
        for (i = -strength; i <= strength; i++)
        {
            GUI.Label(new Rect(rect.x - strength, rect.y + i, rect.width, rect.height), _guiText, style);
            GUI.Label(new Rect(rect.x + strength, rect.y + i, rect.width, rect.height), _guiText, style);
        }
        for (i = -strength + 1; i <= strength - 1; i++)
        {
            GUI.Label(new Rect(rect.x + i, rect.y - strength, rect.width, rect.height), _guiText, style);
            GUI.Label(new Rect(rect.x + i, rect.y + strength, rect.width, rect.height), _guiText, style);
        }
        style.normal.textColor = _guiColor;
        GUI.Label(rect, _guiText, style);
    }
}
