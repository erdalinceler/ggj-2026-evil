using System.Collections.Generic;
using UnityEngine;

public class EntityTextService : MonoBehaviour
{
    public EntityTextPoolRegistry registry;

    public string GetText(Entity entity, TextContext context)
    {
        EntityTextPool pool = ResolvePool(entity, context);
        if (pool == null)
        {
            return string.Empty;
        }

        return pool.GetText(entity);
    }

    public List<string> GetTexts(Entity entity, TextContext context, int count)
    {
        List<string> results = new List<string>();
        if (count <= 0)
        {
            return results;
        }

        EntityTextPool specialPool = ResolveSpecialPool(entity, context);
        EntityTextPool npcPool = registry != null ? registry.GetNpcPool(context) : null;

        HashSet<string> used = new HashSet<string>();
        AddUniqueFromPool(specialPool, results, used, count);
        if (results.Count < count)
        {
            AddUniqueFromPool(npcPool, results, used, count);
        }

        return results;
    }

    private EntityTextPool ResolvePool(Entity entity, TextContext context)
    {
        EntityTextPool specialPool = ResolveSpecialPool(entity, context);
        if (specialPool != null)
        {
            return specialPool;
        }

        return registry != null ? registry.GetNpcPool(context) : null;
    }

    private EntityTextPool ResolveSpecialPool(Entity entity, TextContext context)
    {
        if (entity == null || registry == null)
        {
            return null;
        }

        if (entity.id == Entity.NPC_ID)
        {
            return null;
        }

        return registry.GetSpecialPool(entity.id, context);
    }

    private static void AddUniqueFromPool(EntityTextPool pool, List<string> results, HashSet<string> used, int count)
    {
        if (pool == null || pool.texts == null || pool.texts.Count == 0)
        {
            return;
        }

        List<string> candidates = new List<string>(pool.texts.Count);
        for (int i = 0; i < pool.texts.Count; i++)
        {
            string text = pool.texts[i];
            if (!string.IsNullOrEmpty(text) && !used.Contains(text))
            {
                candidates.Add(text);
            }
        }

        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = candidates[i];
            candidates[i] = candidates[j];
            candidates[j] = temp;
        }

        for (int i = 0; i < candidates.Count && results.Count < count; i++)
        {
            string selected = candidates[i];
            used.Add(selected);
            results.Add(selected);
        }
    }
}
