using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MainMenu : MonoBehaviour
{
    private void Start()
    {
        var player = GameObject.FindWithTag("Player");
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(GameObject.Find("Directional Light"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoginScene();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            RegistrationScene();
        }
    }

    public void LoginScene()
    {
        SceneManager.LoadScene("Login Logger");
    }

    public void RegistrationScene()
    {
        SceneManager.LoadScene("Registration Keyboard");
    }
}
