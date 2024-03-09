using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
	public Slider volumeSlider;

	public void Start()
	{
		volumeSlider.value = PlayerPrefs.GetFloat("MainVolume", 0.5f);
	}

	public void SetVolume()
	{
		AudioListener.volume = volumeSlider.value;
		PlayerPrefs.SetFloat("MainVolume", volumeSlider.value);
		PlayerPrefs.Save();
	}
}
