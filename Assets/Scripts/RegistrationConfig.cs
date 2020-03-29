using UnityEngine;

public class RegistrationConfig : MonoBehaviour
{
    public string userName;
    public string userId;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
