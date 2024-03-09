using UnityEngine;

public class GameManager : MonoBehaviour
{

	private static GameManager instance = null;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			SetInitialResolution();
			InitializeVolume();
		}
		else if (instance != this)
			Destroy(gameObject);
	}

	private void SetInitialResolution()
	{
		int defaultResolutionIndex = 0;
		int resolutionIndex = PlayerPrefs.GetInt("ResolutionSetting", defaultResolutionIndex);

		switch (resolutionIndex)
		{
			case 0:
				Screen.SetResolution(1920, 1080, true);
				Debug.Log("initial resolution 1920");
				break;
			case 1:
				Screen.SetResolution(1600, 900, true);
				Debug.Log("initial resolution 1600");
				break;
			case 2:
				Screen.SetResolution(720, 480, true);
				Debug.Log("initial resolution 720");
				break;
		}
	}

	private void InitializeVolume()
	{
		float savedVolume = PlayerPrefs.GetFloat("MainVolume", 0.5f);
		AudioListener.volume = savedVolume;
	}
}
