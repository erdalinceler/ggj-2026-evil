using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityTextPool", menuName = "Text/Entity Text Pool")]
public class EntityTextPool : ScriptableObject, IEntityTextPool
{
    public TextContext context;

    [TextArea(2, 6)]
    public List<string> texts = new List<string>();

    public string GetText(Entity entity)
    {
        if (texts == null || texts.Count == 0)
        {
            return string.Empty;
        }

        int index = Random.Range(0, texts.Count);
        string selected = texts[index];
        return selected ?? string.Empty;
    }
}
