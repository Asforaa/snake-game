using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Collections; // Needed if you add coroutines later

public class HUDManager : MonoBehaviour
{
	[Header("HUD Elements")]
	[Tooltip("Assign the TextMeshPro UI element for displaying the score here.")]
	[SerializeField] private TextMeshProUGUI scoreText; // Assign in Inspector

	[Header("Game Over Panel Elements")]
	[Tooltip("Assign the parent Panel GameObject for the Game Over UI.")]
	[SerializeField] private GameObject gameOverPanel; // Assign the parent panel
	[Tooltip("Assign the TextMeshPro UI element for displaying the final score.")]
	[SerializeField] private TextMeshProUGUI finalScoreText; // Assign the text inside the panel

	void Awake()
	{

		// Basic check on Awake to ensure assignments
		if (scoreText == null)
		{
			Debug.LogError("[HUDManager] ScoreText is not assigned in the Inspector!");
		}
		if (gameOverPanel == null)
		{
			Debug.LogError("[HUDManager] GameOverPanel is not assigned in the Inspector!");
		}
		if (finalScoreText == null)
		{
			Debug.LogError("[HUDManager] FinalScoreText is not assigned in the Inspector!");
		}

		// Ensure the Game Over panel is hidden at the start
		if (gameOverPanel != null)
		{
			gameOverPanel.SetActive(false);
		}
	}

	void OnEnable()
	{
		// Subscribe to events
		GameManager.OnScoreChanged += UpdateScoreUI;
		GameManager.OnGameOver += ShowGameOverPanel; // Subscribe to the new event

		// Immediately update the score in case the HUD was enabled after the game started
		if (GameManager.Instance != null)
		{
			UpdateScoreUI(GameManager.Instance.GetScore());
		}
		else
		{
			UpdateScoreUI(0); // Update with default if GameManager isn't ready yet
		}

		// Ensure panel is hidden on enable (e.g., after restarting)
		if (gameOverPanel != null && gameOverPanel.activeSelf)
		{
			gameOverPanel.SetActive(false);
		}
	}

	void OnDisable()
	{
		// Unsubscribe from events
		GameManager.OnScoreChanged -= UpdateScoreUI;
		GameManager.OnGameOver -= ShowGameOverPanel;
	}

	private void UpdateScoreUI(int newScore)
	{
		if (scoreText != null)
		{
			// Update the text displayed on the screen
			scoreText.text = "Score: " + newScore;
			// Debug.Log($"[HUDManager] Score UI updated to: {newScore}");
		}
	}

	private void ShowGameOverPanel()
	{
		if (gameOverPanel == null || finalScoreText == null)
		{
			Debug.LogError("[HUDManager] Cannot show Game Over panel, UI elements not assigned!");
			return;
		}

		// Update the final score text
		int finalScore = (GameManager.Instance != null) ? GameManager.Instance.GetScore() : 0;
		finalScoreText.text = "Score: " + finalScore;

		// Show the panel
		gameOverPanel.SetActive(true);
	}

	// --- Public Methods for Button OnClick Events ---

	public void HandleRestartButton()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.LoadGameScene();
		}
		else
		{
			Debug.LogError("[HUDManager] GameManager instance not found! Cannot restart.");
		}
	}

	public void HandleMainMenuButton()
	{
		// Ensure time scale is reset before going to main menu
		Time.timeScale = 1f;
		if (GameManager.Instance != null)
		{
			GameManager.Instance.LoadMainMenuScene();
		}
		else
		{
			Debug.LogError("[HUDManager] GameManager instance not found! Cannot load main menu.");
		}
	}
}
