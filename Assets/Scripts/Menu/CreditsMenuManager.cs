using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenuManager : MonoBehaviour
{
	public void BackButton()
	{
		Debug.Log("Back button pressed!");
		SceneManager.LoadScene("MainMenu");
	}
}
