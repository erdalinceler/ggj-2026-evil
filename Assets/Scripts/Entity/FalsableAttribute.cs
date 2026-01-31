using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class FalsableAttribute : Attribute
{
    public int difficultyScore;
    public int intMinDelta;
    public int intMaxDelta;
    public float floatMinDelta;
    public float floatMaxDelta;
    public double doubleMinDelta;
    public double doubleMaxDelta;
    public int stringMinChanges;
    public int stringMaxChanges;

    public FalsableAttribute(
        int difficultyScore = 0,
        int intMinDelta = 3,
        int intMaxDelta = 10,
        float floatMinDelta = 1f,
        float floatMaxDelta = 5f,
        double doubleMinDelta = 1d,
        double doubleMaxDelta = 5d,
        int stringMinChanges = 1,
        int stringMaxChanges = 2)
    {
        this.difficultyScore = difficultyScore;
        this.intMinDelta = intMinDelta;
        this.intMaxDelta = intMaxDelta;
        this.floatMinDelta = floatMinDelta;
        this.floatMaxDelta = floatMaxDelta;
        this.doubleMinDelta = doubleMinDelta;
        this.doubleMaxDelta = doubleMaxDelta;
        this.stringMinChanges = stringMinChanges;
        this.stringMaxChanges = stringMaxChanges;
    }
}
