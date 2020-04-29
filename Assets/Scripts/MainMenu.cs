using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenu : MonoBehaviour
{
    public Toggle simulatedToggle;

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
        if (!simulatedToggle)
        {
            throw new UnityException("Missing link to toggle!");
        }

        SceneManager.LoadScene(simulatedToggle.isOn ? "Login Simulation" : "Login Logger");
    }

    public void RegistrationScene()
    {
        SceneManager.LoadScene("Registration Keyboard");
    }
}
