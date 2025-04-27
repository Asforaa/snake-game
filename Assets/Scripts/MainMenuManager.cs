using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public void PlayGame()
	{
		// Make sure the GameManager exists and can load the scene
		if (GameManager.Instance != null)
		{
			GameManager.Instance.LoadGameScene();
		}
		else
		{
			// Fallback in case GameManager isn't ready (shouldn't happen with DontDestroyOnLoad)
			Debug.LogError("[MainMenuManager] GameManager instance not found! Cannot load Game scene.");
			// SceneManager.LoadScene("Game"); // Less ideal fallback
		}
	}

	// placeholder for QuitGame method later, too lazy to implement rn
	// public void QuitGame()
	// {
	//     Application.Quit();
	// }
}
