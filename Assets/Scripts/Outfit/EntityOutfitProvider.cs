using UnityEngine;

public class EntityOutfitProvider : MonoBehaviour
{
    [SerializeField] private OutfitRegistry registry;

    public OutfitResult GetOutfit(Entity entity)
    {
        if (entity == null || registry == null)
        {
            return OutfitResult.Empty;
        }

        if (entity.id == Entity.NPC_ID)
        {
            NpcOutfitData outfit = registry.GetRandomNpcOutfit(entity.entityInfo.gender);
            if (outfit == null)
            {
                return OutfitResult.Empty;
            }

            System.Collections.Generic.List<OutfitLayer> layers = BuildRandomLayers(outfit);
            return OutfitResult.FromLayers(layers);
        }

        Sprite sprite = registry.GetSpecialSprite(entity.id);
        if (sprite == null)
        {
            return OutfitResult.Empty;
        }

        return OutfitResult.FromSingle(sprite);
    }

    private static System.Collections.Generic.List<OutfitLayer> BuildRandomLayers(NpcOutfitData outfit)
    {
        System.Collections.Generic.List<OutfitLayer> layers = new System.Collections.Generic.List<OutfitLayer>(7);
        AddLayerIfSprite(layers, OutfitSlot.Hair, GetRandomSprite(outfit.hair));
        AddLayerIfSprite(layers, OutfitSlot.Eye, GetRandomSprite(outfit.eye));
        AddLayerIfSprite(layers, OutfitSlot.Body, GetRandomSprite(outfit.body));
        AddLayerIfSprite(layers, OutfitSlot.Cloth, GetRandomSprite(outfit.cloth));
        AddLayerIfSprite(layers, OutfitSlot.EyeBrown, GetRandomSprite(outfit.eyeBrown));

        if (outfit.gender == Gender.Female)
        {
            AddLayerIfSprite(layers, OutfitSlot.Earrings, GetRandomSprite(outfit.earrings));
        }
        else
        {
            AddLayerIfSprite(layers, OutfitSlot.Beard, GetRandomSprite(outfit.beard));
        }

        return layers;
    }

    private static void AddLayerIfSprite(System.Collections.Generic.List<OutfitLayer> layers, OutfitSlot slot, Sprite sprite)
    {
        if (sprite == null)
        {
            return;
        }

        layers.Add(new OutfitLayer
        {
            slot = slot,
            sprite = sprite
        });
    }

    private static Sprite GetRandomSprite(System.Collections.Generic.List<Sprite> sprites)
    {
        if (sprites == null || sprites.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, sprites.Count);
        return sprites[index];
    }
}
