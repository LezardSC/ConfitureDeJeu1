using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayButton()
    {
		SceneManager.LoadScene("MainScene");
		Debug.Log("Play button pressed!");
    }

    public void OptionsButton()
    {
		Debug.Log("Option button pressed!");
        SceneManager.LoadScene("OptionsMenu");
	}

    public void CreditsButton()
    {
        Debug.Log("Credit button pressed!");
		SceneManager.LoadScene("CreditsMenu");
	}

    public void QuitMenu()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
    }
}
