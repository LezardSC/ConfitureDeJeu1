using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionMenuManager : MonoBehaviour
{
	public TMP_Dropdown Resolution;

	public void Start()
	{
		LoadResolutionSetting();
	}

	public void LoadResolutionSetting()
	{
		if (PlayerPrefs.HasKey("ResolutionSetting"))
		{
			int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionSetting");

			Resolution.SetValueWithoutNotify(savedResolutionIndex);
			SetResolution(savedResolutionIndex);
		}
	}

	public void SetResolution(int resolutionIndex)
	{
		PlayerPrefs.SetInt("ResolutionSetting", resolutionIndex);
		PlayerPrefs.Save();

		switch (resolutionIndex)
		{
			case 0:
				Screen.SetResolution(1920, 1080, true);
				Debug.Log("set resolution 1920");
				break;
			case 1:
				Screen.SetResolution(1600, 900, true);
				Debug.Log("set resolution 1600");
				break;
			case 2:
				Screen.SetResolution(720, 480, true);
				Debug.Log("set resolution 720");
				break;
		}
	}

	public void BackButton()
	{
		Debug.Log("Back button pressed!");
		SceneManager.LoadScene("MainMenu");
	}
}
