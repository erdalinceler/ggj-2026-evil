public enum GameState
{
    NotStarted,
    InitiliazingGame,
    SpawningDialogues,
    ProcessingQueue,
    AllDialoguesComplete,
    CalculatingScore,
    CheckingVictory,
    ShowingResult,
    GameEnded
}


public enum DialogueState
{
    WaitingInQueue,
    Active,
    PlayerChoosing,
    Completed,
}

public enum DialogueResult
{
    Continue,
    Success,
}

public enum SceneState
{
    InMainMenu,
    IsPlaying,
    IsPaused,
}

public enum GameResult
{
    Victory,
    Defeat
}



