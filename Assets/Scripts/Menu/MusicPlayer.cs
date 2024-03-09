using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	private void Awake()
	{
		GameObject[] otherInstances = GameObject.FindGameObjectsWithTag("MusicPlayer");
		if (otherInstances.Length > 1)
			Destroy(this.gameObject);
		else
			DontDestroyOnLoad(this.gameObject);
	}
}
