using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityTextPoolRegistry", menuName = "Text/Entity Text Pool Registry")]
public class EntityTextPoolRegistry : ScriptableObject
{
    public List<EntityTextPool> npcPools = new List<EntityTextPool>();
    public List<SpecialEntityPools> specialPools = new List<SpecialEntityPools>();

    public EntityTextPool GetNpcPool(TextContext context)
    {
        return FindPool(npcPools, context);
    }

    public EntityTextPool GetSpecialPool(int entityId, TextContext context)
    {
        for (int i = 0; i < specialPools.Count; i++)
        {
            if (specialPools[i].entityId == entityId)
            {
                return FindPool(specialPools[i].contextPools, context);
            }
        }

        return null;
    }

    private static EntityTextPool FindPool(List<EntityTextPool> pools, TextContext context)
    {
        if (pools == null)
        {
            return null;
        }

        for (int i = 0; i < pools.Count; i++)
        {
            EntityTextPool pool = pools[i];
            if (pool != null && pool.context == context)
            {
                return pool;
            }
        }

        return null;
    }
}

[Serializable]
public class SpecialEntityPools
{
    public int entityId;
    public List<EntityTextPool> contextPools = new List<EntityTextPool>();
}
