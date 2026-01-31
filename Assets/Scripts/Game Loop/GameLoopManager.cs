using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameLoopManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int totalRounds = 10;
    [SerializeField] private int minDifficulty = 0;
    [SerializeField] private int maxDifficulty = 5;
    [SerializeField] private float delayBetweenDialogues = 1f;

    [Header("Entity Settings")]
    [SerializeField] private List<EntityInfo> entityInfoPool = new List<EntityInfo>();
    [SerializeField] private EntityTextService textService;

    [Header("Score Settings")]
    [SerializeField] private int correctAnswerScore = 10;
    [SerializeField] private int wrongAnswerPenalty = -5;
    [SerializeField] private int victoryThreshold = 50;

    [Header("Events")]
    public UnityEvent<GameState> onGameStateChanged;
    public UnityEvent<DialogueState> onDialogueStateChanged;
    public UnityEvent<Entity> onEntitySpawned;
    public UnityEvent<int> onScoreChanged;
    public UnityEvent<GameResult> onGameEnded;

    // State
    private GameState currentGameState = GameState.NotStarted;
    private DialogueState currentDialogueState = DialogueState.WaitingInQueue;
    private Queue<Entity> entityQueue = new Queue<Entity>();
    private Entity currentEntity;
    private int currentScore = 0;
    private int correctAnswers = 0;
    private int wrongAnswers = 0;
    private int currentRound = 0;

    // Properties
    public GameState CurrentGameState => currentGameState;
    public DialogueState CurrentDialogueState => currentDialogueState;
    public Entity CurrentEntity => currentEntity;
    public int CurrentScore => currentScore;
    public int CurrentRound => currentRound;
    public int TotalRounds => totalRounds;
    public int CorrectAnswers => correctAnswers;
    public int WrongAnswers => wrongAnswers;
    public int RemainingEntities => entityQueue.Count;

    private void Start()
    {
      
    }

    public void StartGame()
    {
        if (currentGameState != GameState.NotStarted && currentGameState != GameState.GameEnded)
        {
            Debug.LogWarning("Game is already running!");
            return;
        }

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        // Initialize Game
        ChangeGameState(GameState.InitiliazingGame);
        InitializeGame();
        yield return null;

        // Spawn Dialogues (Create Entity Queue)
        ChangeGameState(GameState.SpawningDialogues);
        SpawnAllEntities();
        yield return null;

        // Process Queue
        ChangeGameState(GameState.ProcessingQueue);
        yield return StartCoroutine(ProcessEntityQueue());

        // All Dialogues Complete
        ChangeGameState(GameState.AllDialoguesComplete);
        yield return new WaitForSeconds(0.5f);

        // Calculate Score
        ChangeGameState(GameState.CalculatingScore);
        CalculateFinalScore();
        yield return new WaitForSeconds(0.5f);

        // Check Victory
        ChangeGameState(GameState.CheckingVictory);
        GameResult result = DetermineGameResult();
        yield return new WaitForSeconds(0.5f);

        // Show Result
        ChangeGameState(GameState.ShowingResult);
        onGameEnded?.Invoke(result);
        yield return new WaitForSeconds(1f);

        // Game Ended
        ChangeGameState(GameState.GameEnded);
    }

    private void InitializeGame()
    {
        entityQueue.Clear();
        currentEntity = null;
        currentScore = 0;
        correctAnswers = 0;
        wrongAnswers = 0;
        currentRound = 0;

        Debug.Log("Game Initialized");
    }

    private void SpawnAllEntities()
    {
        for (int i = 0; i < totalRounds; i++)
        {
            Entity entity = CreateRandomEntity(i);
            entityQueue.Enqueue(entity);
        }

        Debug.Log($"Spawned {entityQueue.Count} entities in queue");
    }

    private Entity CreateRandomEntity(int roundIndex)
    {
        // Randomly select entity state
        EntityState[] states = (EntityState[])Enum.GetValues(typeof(EntityState));
        EntityState randomState = states[UnityEngine.Random.Range(0, states.Length)];

        // Create entity with unique ID
        Entity entity = new Entity(randomState, roundIndex + 1);

        // Get random entity info from pool
        if (entityInfoPool.Count > 0)
        {
            EntityInfo randomInfo = entityInfoPool[UnityEngine.Random.Range(0, entityInfoPool.Count)];
            entity.SetVariables(randomInfo, minDifficulty, maxDifficulty);
        }
        else
        {
            // Create default entity info if pool is empty
            EntityInfo defaultInfo = new EntityInfo
            {
                name = "Unknown",
                age = UnityEngine.Random.Range(18, 65),
                gender = UnityEngine.Random.Range(0, 2) == 0 ? Gender.Male : Gender.Female
            };
            entity.SetVariables(defaultInfo, minDifficulty, maxDifficulty);
        }

        return entity;
    }

    private IEnumerator ProcessEntityQueue()
    {
        while (entityQueue.Count > 0)
        {
            currentRound++;
            currentEntity = entityQueue.Dequeue();

            // Entity is now active
            ChangeDialogueState(DialogueState.Active);
            Debug.Log($"Processing Entity {currentEntity.id} - Round {currentRound}/{totalRounds}");
            onEntitySpawned?.Invoke(currentEntity);

            yield return new WaitForSeconds(0.3f);

            // Wait for player to make a choice
            ChangeDialogueState(DialogueState.PlayerChoosing);
            yield return StartCoroutine(WaitForPlayerChoice());

            // Dialogue completed
            ChangeDialogueState(DialogueState.Completed);
            yield return new WaitForSeconds(delayBetweenDialogues);

            // Back to waiting for next entity
            if (entityQueue.Count > 0)
            {
                ChangeDialogueState(DialogueState.WaitingInQueue);
            }
        }

        currentEntity = null;
    }

    private IEnumerator WaitForPlayerChoice()
    {
        // Wait until player makes a choice
        // This is controlled by SubmitPlayerChoice() method
        bool choiceMade = false;
        float timeout = 30f; // 30 second timeout
        float elapsed = 0f;

        while (!choiceMade && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
            
            // Check if choice was made (this will be set by SubmitPlayerChoice)
            if (currentDialogueState == DialogueState.Completed)
            {
                choiceMade = true;
            }
        }

        if (!choiceMade)
        {
            Debug.LogWarning("Player choice timeout - treating as wrong answer");
            wrongAnswers++;
            AddScore(wrongAnswerPenalty);
        }
    }

    public void SubmitPlayerChoice(bool playerThinksFalse)
    {
        if (currentEntity == null)
        {
            Debug.LogWarning("No current entity to evaluate!");
            return;
        }

        if (currentDialogueState != DialogueState.PlayerChoosing)
        {
            Debug.LogWarning("Not in player choosing state!");
            return;
        }

        // Check if player was correct
        bool actuallyHasFalseInfo = currentEntity.falseInfo.Count > 0;
        bool isCorrect = playerThinksFalse == actuallyHasFalseInfo;

        DialogueResult result;
        if (isCorrect)
        {
            correctAnswers++;
            AddScore(correctAnswerScore);
            result = DialogueResult.Success;
            Debug.Log($"Correct! +{correctAnswerScore} points");
        }
        else
        {
            wrongAnswers++;
            AddScore(wrongAnswerPenalty);
            result = DialogueResult.Continue;
            Debug.Log($"Wrong! {wrongAnswerPenalty} points");
        }

        // Move to completed state
        ChangeDialogueState(DialogueState.Completed);
    }

    private void AddScore(int points)
    {
        currentScore += points;
        currentScore = Mathf.Max(0, currentScore); // Prevent negative scores
        onScoreChanged?.Invoke(currentScore);
    }

    private void CalculateFinalScore()
    {
        // Additional score calculations can be done here
        // For example: bonus for perfect rounds, time bonuses, etc.
        
        // Accuracy bonus
        if (correctAnswers == totalRounds)
        {
            int perfectBonus = 50;
            AddScore(perfectBonus);
            Debug.Log($"Perfect game! Bonus: +{perfectBonus}");
        }

        Debug.Log($"Final Score: {currentScore}");
        Debug.Log($"Correct: {correctAnswers}, Wrong: {wrongAnswers}");
    }

    private GameResult DetermineGameResult()
    {
        float accuracy = totalRounds > 0 ? (float)correctAnswers / totalRounds : 0f;

        if (currentScore >= victoryThreshold && accuracy >= 0.7f)
        {
            Debug.Log("Victory!");
            return GameResult.Victory;
        }
        else
        {
            Debug.Log("Defeat!");
            return GameResult.Defeat;
        }
    }

    private void ChangeGameState(GameState newState)
    {
        currentGameState = newState;
        onGameStateChanged?.Invoke(newState);
        Debug.Log($"Game State: {newState}");
    }

    private void ChangeDialogueState(DialogueState newState)
    {
        currentDialogueState = newState;
        onDialogueStateChanged?.Invoke(newState);
        Debug.Log($"Dialogue State: {newState}");
    }

    public void RestartGame()
    {
        StopAllCoroutines();
        currentGameState = GameState.NotStarted;
        currentDialogueState = DialogueState.WaitingInQueue;
        StartGame();
    }

    public void QuitGame()
    {
        StopAllCoroutines();
        ChangeGameState(GameState.GameEnded);
        Debug.Log("Game Quit");
    }

    // Helper methods to get text for current entity
    public string GetEntityText(TextContext context)
    {
        if (currentEntity == null || textService == null)
        {
            return string.Empty;
        }

        return textService.GetText(currentEntity, context);
    }

    public List<string> GetEntityTexts(TextContext context, int count)
    {
        if (currentEntity == null || textService == null)
        {
            return new List<string>();
        }

        return textService.GetTexts(currentEntity, context, count);
    }

    // Debug helper
    public void PrintCurrentState()
    {
        Debug.Log($"=== Game State ===");
        Debug.Log($"Game: {currentGameState}");
        Debug.Log($"Dialogue: {currentDialogueState}");
        Debug.Log($"Round: {currentRound}/{totalRounds}");
        Debug.Log($"Score: {currentScore}");
        Debug.Log($"Queue: {entityQueue.Count} remaining");
    }
}
