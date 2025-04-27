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

	// Scenes
	private const string MainMenuSceneName = "MainMenu";
	private const string GameSceneName = "Game";


	void Awake()
	{
		//Basic Singleton pattern implementation
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // Keep GameManager across scene loads
		}
		else
		{
			Destroy(gameObject); // Destroy duplicate instances
		}
	}

	void Start()
	{
		score = 0;
		Time.timeScale = 1f; // Ensure time is running when game scene starts
	}

	public void IncreaseScore(int amount)
	{
		score += amount;
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
		// Notify listeners of the reset
		OnScoreChanged?.Invoke(score);
	}

	public void GameOver()
	{

		Time.timeScale = 0f; // Pause the game
		OnGameOver?.Invoke(); // Trigger the game over event

		// we could save high score here
	}

	// --- Scene Loading Methods ---

	public void LoadGameScene()
	{
		// Reset score before loading the scene to ensure it's 0 when Game starts
		ResetScore();
		// Ensure time is running before loading the new scene
		Time.timeScale = 1f;
		SceneManager.LoadScene(GameSceneName);
	}


	public void LoadMainMenuScene()
	{
		Debug.Log($"[GameManager] Loading scene: {MainMenuSceneName}");
		// Ensure time is running normally when returning to menu
		Time.timeScale = 1f;
		SceneManager.LoadScene(MainMenuSceneName);
	}

	// we could add other game management logic here (pause, resume, etc.)
}
