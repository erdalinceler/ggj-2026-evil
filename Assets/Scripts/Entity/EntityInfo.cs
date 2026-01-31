public struct EntityInfo
{
    [Falsable(difficultyScore: 0)]
    public string name;

    [Falsable(difficultyScore: 0)]
    public int age;

    [Falsable(difficultyScore: 20)]
    public bool hearthBlue;

    [Falsable(difficultyScore: 0)]
    public Gender gender;
}
