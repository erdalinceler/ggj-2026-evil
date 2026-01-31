using UnityEngine;

public class EntityOutfitApplier : MonoBehaviour
{
    [SerializeField] private EntityOutfitProvider provider;
    [SerializeField] private GameLoopManager gameLoop;
    [SerializeField] private SpriteRenderer specialRenderer;

    [Header("Layered Renderers")]
    [SerializeField] private SpriteRenderer hairRenderer;
    [SerializeField] private SpriteRenderer eyeRenderer;
    [SerializeField] private SpriteRenderer noseRenderer;
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer clothRenderer;
    [SerializeField] private SpriteRenderer earringsRenderer;
    [SerializeField] private SpriteRenderer beardRenderer;

    private void OnEnable()
    {
        EnsureReferences();
        if (gameLoop != null)
        {
            gameLoop.onEntitySpawned.AddListener(HandleEntitySpawned);
        }
    }

    private void OnDisable()
    {
        if (gameLoop != null)
        {
            gameLoop.onEntitySpawned.RemoveListener(HandleEntitySpawned);
        }
    }

    private void EnsureReferences()
    {
        if (provider == null)
        {
            provider = FindObjectOfType<EntityOutfitProvider>();
        }

        if (gameLoop == null)
        {
            gameLoop = FindObjectOfType<GameLoopManager>();
        }
    }

    private void HandleEntitySpawned(Entity entity)
    {
        OutfitResult result = provider != null ? provider.GetOutfit(entity) : OutfitResult.Empty;

        if (result.isLayered)
        {
            ApplyLayered(result.layers);
            SetSpecialActive(false);
        }
        else
        {
            ApplySpecial(result.singleSprite);
            SetLayeredActive(false);
        }
    }

    private void ApplyLayered(System.Collections.Generic.List<OutfitLayer> layers)
    {
        ClearLayeredSprites();
        SetLayeredActive(true);

        if (layers == null)
        {
            return;
        }

        for (int i = 0; i < layers.Count; i++)
        {
            OutfitLayer layer = layers[i];
            SpriteRenderer renderer = GetRendererForSlot(layer.slot);
            if (renderer != null)
            {
                renderer.sprite = layer.sprite;
                renderer.enabled = layer.sprite != null;
            }
        }
    }

    private void ApplySpecial(Sprite sprite)
    {
        if (specialRenderer == null)
        {
            return;
        }

        specialRenderer.sprite = sprite;
        specialRenderer.enabled = sprite != null;
    }

    private void ClearLayeredSprites()
    {
        ClearRenderer(hairRenderer);
        ClearRenderer(eyeRenderer);
        ClearRenderer(noseRenderer);
        ClearRenderer(bodyRenderer);
        ClearRenderer(clothRenderer);
        ClearRenderer(earringsRenderer);
        ClearRenderer(beardRenderer);
    }

    private static void ClearRenderer(SpriteRenderer renderer)
    {
        if (renderer == null)
        {
            return;
        }

        renderer.sprite = null;
        renderer.enabled = false;
    }

    private void SetLayeredActive(bool active)
    {
        SetRendererActive(hairRenderer, active);
        SetRendererActive(eyeRenderer, active);
        SetRendererActive(noseRenderer, active);
        SetRendererActive(bodyRenderer, active);
        SetRendererActive(clothRenderer, active);
        SetRendererActive(earringsRenderer, active);
        SetRendererActive(beardRenderer, active);
    }

    private void SetSpecialActive(bool active)
    {
        if (specialRenderer == null)
        {
            return;
        }

        specialRenderer.enabled = active && specialRenderer.sprite != null;
    }

    private static void SetRendererActive(SpriteRenderer renderer, bool active)
    {
        if (renderer == null)
        {
            return;
        }

        renderer.enabled = active && renderer.sprite != null;
    }

    private SpriteRenderer GetRendererForSlot(OutfitSlot slot)
    {
        switch (slot)
        {
            case OutfitSlot.Hair:
                return hairRenderer;
            case OutfitSlot.Eye:
                return eyeRenderer;
            case OutfitSlot.Nose:
                return noseRenderer;
            case OutfitSlot.Body:
                return bodyRenderer;
            case OutfitSlot.Cloth:
                return clothRenderer;
            case OutfitSlot.Earrings:
                return earringsRenderer;
            case OutfitSlot.Beard:
                return beardRenderer;
            default:
                return null;
        }
    }
}
