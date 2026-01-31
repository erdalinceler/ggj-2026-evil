using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OutfitRegistry", menuName = "Outfit/Outfit Registry")]
public class OutfitRegistry : ScriptableObject
{
    public List<NpcOutfitData> npcOutfits = new List<NpcOutfitData>();
    public List<SpecialEntitySpriteEntry> specialSprites = new List<SpecialEntitySpriteEntry>();

    public NpcOutfitData GetRandomNpcOutfit(Gender gender)
    {
        if (npcOutfits == null || npcOutfits.Count == 0)
        {
            return null;
        }

        List<NpcOutfitData> matches = null;
        for (int i = 0; i < npcOutfits.Count; i++)
        {
            NpcOutfitData outfit = npcOutfits[i];
            if (outfit == null)
            {
                continue;
            }

            if (outfit.gender != gender)
            {
                continue;
            }

            if (matches == null)
            {
                matches = new List<NpcOutfitData>();
            }

            matches.Add(outfit);
        }

        if (matches == null || matches.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, matches.Count);
        return matches[index];
    }

    public Sprite GetSpecialSprite(int entityId)
    {
        if (specialSprites == null)
        {
            return null;
        }

        for (int i = 0; i < specialSprites.Count; i++)
        {
            SpecialEntitySpriteEntry entry = specialSprites[i];
            if (entry.entityId == entityId)
            {
                return entry.sprite;
            }
        }

        return null;
    }
}
