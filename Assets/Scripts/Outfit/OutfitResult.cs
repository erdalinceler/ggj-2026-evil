using System.Collections.Generic;
using UnityEngine;

public struct OutfitResult
{
    public bool isLayered;
    public List<OutfitLayer> layers;
    public Sprite singleSprite;

    public static OutfitResult Empty => new OutfitResult
    {
        isLayered = false,
        layers = null,
        singleSprite = null
    };

    public static OutfitResult FromLayers(List<OutfitLayer> layers)
    {
        return new OutfitResult
        {
            isLayered = true,
            layers = layers,
            singleSprite = null
        };
    }

    public static OutfitResult FromSingle(Sprite sprite)
    {
        return new OutfitResult
        {
            isLayered = false,
            layers = null,
            singleSprite = sprite
        };
    }
}
