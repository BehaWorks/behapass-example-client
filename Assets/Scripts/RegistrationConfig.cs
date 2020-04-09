using UnityEngine;

public sealed class RegistrationConfig : MonoBehaviour
{
    public string userName;
    public string userId;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
