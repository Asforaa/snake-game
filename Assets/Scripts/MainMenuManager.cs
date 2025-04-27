using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public void PlayGame()
	{
		// Make sure the GameManager exists and can load the scene
		if (GameManager.Instance != null)
		{
			Debug.Log("[MainMenuManager] Play button clicked. Loading Game scene...");
			GameManager.Instance.LoadGameScene();
		}
		else
		{
			// Fallback in case GameManager isn't ready (shouldn't happen with DontDestroyOnLoad)
			Debug.LogError("[MainMenuManager] GameManager instance not found! Cannot load Game scene.");
			// SceneManager.LoadScene("Game"); // Less ideal fallback
		}
	}

	// Optional: Add a QuitGame method if needed
	// public void QuitGame()
	// {
	//     Debug.Log("[MainMenuManager] Quit button clicked.");
	//     Application.Quit();
	// }
}
