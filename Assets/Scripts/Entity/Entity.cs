using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Entity
{
    public int id;

    public const int NPC_ID = 1;

    public EntityInfo entityInfo;
    public EntityState state;

    public HashSet<string> falseInfo = new HashSet<string>();
    public Dictionary<string, object> fakeValues = new Dictionary<string, object>();

    // Cache reflection results to avoid repeated GetFields/GetField calls
    private static readonly FieldInfo[] entityInfoFields;
    private static readonly Dictionary<string, FieldInfo> entityInfoFieldsByName;

    static Entity()
    {
        // Initialize both collections in static constructor for thread-safe initialization
        entityInfoFields = typeof(EntityInfo).GetFields(BindingFlags.Public | BindingFlags.Instance);
        entityInfoFieldsByName = new Dictionary<string, FieldInfo>();
        
        // Populate the field name lookup dictionary
        foreach (FieldInfo field in entityInfoFields)
        {
            entityInfoFieldsByName[field.Name] = field;
        }
    }

    public Entity(EntityState state)
    {
        this.state = state;
        id = NPC_ID;
    }

    public Entity(EntityState state, int id)
    {
        this.state = state;
        this.id = id;
    }

    public void SetVariables(EntityInfo info, int minDifficulty, int maxDifficulty)
    {
        entityInfo.name = info.name;
        entityInfo.age = info.age;
        entityInfo.gender = info.gender;
        falseInfo.Clear();
        fakeValues.Clear();

        List<FieldInfo> eligible = new List<FieldInfo>();

        foreach (FieldInfo field in entityInfoFields)
        {
            FalsableAttribute attribute = (FalsableAttribute)Attribute.GetCustomAttribute(field, typeof(FalsableAttribute));
            if (attribute == null)
            {
                continue;
            }

            if (attribute.difficultyScore < minDifficulty || attribute.difficultyScore > maxDifficulty)
            {
                continue;
            }

            eligible.Add(field);
        }

        for (int i = eligible.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            FieldInfo temp = eligible[i];
            eligible[i] = eligible[j];
            eligible[j] = temp;
        }

        foreach (FieldInfo field in eligible)
        {
            falseInfo.Add(field.Name);
            object realValue = field.GetValue(entityInfo);
            fakeValues[field.Name] = GenerateFakeValue(field, realValue);
        }
    }

    public (object, bool) GetDisplayValue(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return (null, false);
        }

        if (!entityInfoFieldsByName.TryGetValue(name, out FieldInfo field))
        {
            return (null, false);
        }

        if (fakeValues.TryGetValue(name, out object fakeValue))
        {
            return (fakeValue, true);
        }

        return (field.GetValue(entityInfo), false);
    }

    public (T, bool) GetVariable<T>(string name) where T: class
    {
        if (string.IsNullOrEmpty(name))
        {
            return (null, false);
        }

        if (!entityInfoFieldsByName.TryGetValue(name, out FieldInfo field))
        {
            return (null, false);
        }

        return (field.GetValue(entityInfo) as T, falseInfo.Contains(name));
    }

    private object GenerateFakeValue(FieldInfo field, object realValue)
    {
        Type fieldType = field.FieldType;
        FalsableAttribute attribute = (FalsableAttribute)Attribute.GetCustomAttribute(field, typeof(FalsableAttribute));
        int intMinDelta = attribute != null ? attribute.intMinDelta : 3;
        int intMaxDelta = attribute != null ? attribute.intMaxDelta : 10;
        float floatMinDelta = attribute != null ? attribute.floatMinDelta : 1f;
        float floatMaxDelta = attribute != null ? attribute.floatMaxDelta : 5f;
        double doubleMinDelta = attribute != null ? attribute.doubleMinDelta : 1d;
        double doubleMaxDelta = attribute != null ? attribute.doubleMaxDelta : 5d;
        int stringMinChanges = attribute != null ? attribute.stringMinChanges : 1;
        int stringMaxChanges = attribute != null ? attribute.stringMaxChanges : 2;

        if (intMinDelta < 0)
        {
            intMinDelta = 0;
        }

        if (intMaxDelta < intMinDelta)
        {
            intMaxDelta = intMinDelta;
        }

        if (floatMinDelta < 0f)
        {
            floatMinDelta = 0f;
        }

        if (floatMaxDelta < floatMinDelta)
        {
            floatMaxDelta = floatMinDelta;
        }

        if (doubleMinDelta < 0d)
        {
            doubleMinDelta = 0d;
        }

        if (doubleMaxDelta < doubleMinDelta)
        {
            doubleMaxDelta = doubleMinDelta;
        }

        if (stringMinChanges < 0)
        {
            stringMinChanges = 0;
        }

        if (stringMaxChanges < stringMinChanges)
        {
            stringMaxChanges = stringMinChanges;
        }

        if (fieldType == typeof(string))
        {
            string value = realValue as string ?? "Unknown";
            if (value.Length < 2)
            {
                char suffix = (char)('a' + UnityEngine.Random.Range(0, 26));
                return value + suffix;
            }

            char[] chars = value.ToCharArray();
            int changeCount = UnityEngine.Random.Range(stringMinChanges, stringMaxChanges + 1);
            if (changeCount <= 0)
            {
                changeCount = 1;
            }

            for (int i = 0; i < changeCount; i++)
            {
                int index = UnityEngine.Random.Range(0, chars.Length);
                char original = chars[index];
                char replacement = original;
                int safety = 0;
                while (replacement == original && safety < 10)
                {
                    replacement = (char)('a' + UnityEngine.Random.Range(0, 26));
                    safety++;
                }
                chars[index] = replacement;
            }

            string mutated = new string(chars);
            if (mutated == value)
            {
                int index = UnityEngine.Random.Range(0, chars.Length);
                chars[index] = (char)('a' + UnityEngine.Random.Range(0, 26));
                mutated = new string(chars);
            }

            return mutated;
        }

        if (fieldType == typeof(int))
        {
            int value = realValue != null ? (int)realValue : 0;
            int delta = UnityEngine.Random.Range(intMinDelta, intMaxDelta + 1);
            int sign = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            int fake = value + (delta * sign);
            fake = Math.Max(0, fake);
            if (fake == value)
            {
                fake = Math.Max(0, value + delta);
            }
            return fake;
        }

        if (fieldType == typeof(float))
        {
            float value = realValue != null ? (float)realValue : 0f;
            float delta = UnityEngine.Random.Range(floatMinDelta, floatMaxDelta);
            float sign = UnityEngine.Random.Range(0, 2) == 0 ? -1f : 1f;
            float fake = value + (delta * sign);
            if (Mathf.Approximately(fake, value))
            {
                fake = value + delta;
            }
            return fake;
        }

        if (fieldType == typeof(double))
        {
            double value = realValue != null ? (double)realValue : 0d;
            double delta = UnityEngine.Random.Range((float)doubleMinDelta, (float)doubleMaxDelta);
            double sign = UnityEngine.Random.Range(0, 2) == 0 ? -1d : 1d;
            double fake = value + (delta * sign);
            if (Math.Abs(fake - value) < 0.0001d)
            {
                fake = value + delta;
            }
            return fake;
        }

        if (fieldType == typeof(bool))
        {
            bool value = realValue != null && (bool)realValue;
            return !value;
        }

        if (fieldType.IsEnum)
        {
            Array values = Enum.GetValues(fieldType);
            if (values.Length <= 1)
            {
                return realValue;
            }

            object current = realValue ?? values.GetValue(0);
            int currentIndex = Array.IndexOf(values, current);
            int index = UnityEngine.Random.Range(0, values.Length - 1);
            if (index >= currentIndex)
            {
                index += 1;
            }
            return values.GetValue(index);
        }

        return realValue;
    }
}
