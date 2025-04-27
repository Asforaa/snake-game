using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class GameManager : MonoBehaviour
{
	// Optional: Singleton pattern instance
	public static GameManager Instance { get; private set; }

	// Event for score changes
	public static event System.Action<int> OnScoreChanged;
	// Event for game over state
	public static event System.Action OnGameOver;

	private int score;

	// Scene names (match your scene assets)
	private const string MainMenuSceneName = "MainMenu";
	private const string GameSceneName = "Game";
	private const string GameOverSceneName = "GameOver"; // We might not use this scene anymore


	void Awake()
	{
		Debug.Log("[GameManager] Awake called.");
		// Optional: Basic Singleton pattern implementation
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // Keep GameManager across scene loads
			Debug.Log("[GameManager] Singleton instance created and marked DontDestroyOnLoad.");
		}
		else
		{
			Debug.LogWarning("[GameManager] Duplicate instance detected. Destroying this one.");
			Destroy(gameObject); // Destroy duplicate instances
		}
	}

	void Start()
	{
		Debug.Log("[GameManager] Start called.");
		// Initialize score or other game state when the game starts
		// ResetScore(); // ResetScore now invokes the event, potentially before HUD is ready.
		// It's safer to let the HUDManager pull the initial score in its OnEnable.
		score = 0; // Just set the initial value here.
		Time.timeScale = 1f; // Ensure time is running when game scene starts
	}

	public void IncreaseScore(int amount)
	{
		score += amount;
		Debug.Log($"[GameManager] Score increased by {amount}. New score: {score}");
		// Notify listeners (like the HUD) that the score has changed
		OnScoreChanged?.Invoke(score);
	}

	public int GetScore()
	{
		return score;
	}

	public void ResetScore()
	{
		score = 0;
		Debug.Log("[GameManager] Score reset to 0.");
		// Notify listeners of the reset
		OnScoreChanged?.Invoke(score);
	}

	public void GameOver()
	{
		Debug.Log($"[GameManager] GameOver triggered! Final Score: {score}");
		// Find the SnakeController and set its state if necessary (though it should call this method)
		// SnakeController snake = FindObjectOfType<SnakeController>();
		// if (snake != null) snake.SetAlive(false); // Example if needed

		Time.timeScale = 0f; // Pause the game
		OnGameOver?.Invoke(); // Trigger the game over event
							  // Optionally save high score here
							  // LoadGameOverScene(); // No longer loading a scene
	}

	// --- Scene Loading Methods ---

	public void LoadGameScene()
	{
		Debug.Log($"[GameManager] Loading scene: {GameSceneName}");
		// Reset score before loading the scene to ensure it's 0 when Game starts
		ResetScore();
		// Ensure time is running before loading the new scene
		Time.timeScale = 1f;
		SceneManager.LoadScene(GameSceneName);
	}

	public void LoadGameOverScene() // KEEPING this method in case needed elsewhere, but GameOver() won't call it.
	{
		Debug.LogWarning($"[GameManager] LoadGameOverScene called, but game over is now handled by UI panel.");
		// SceneManager.LoadScene(GameOverSceneName); // Commented out
	}

	public void LoadMainMenuScene()
	{
		Debug.Log($"[GameManager] Loading scene: {MainMenuSceneName}");
		// Ensure time is running normally when returning to menu
		Time.timeScale = 1f;
		SceneManager.LoadScene(MainMenuSceneName);
	}

	// Add other game management logic here (pause, resume, etc.)
}
