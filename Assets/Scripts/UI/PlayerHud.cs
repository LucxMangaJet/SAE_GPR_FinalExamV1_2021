using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum SpellSlot
{
    Primary,
    Secondary
}

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private SpellCastingController spellCastingController;
    [SerializeField] private DropCollector dropCollector;

    [SerializeField] private SpellSlotUI primarySpell, secondarySpell;

    [Header("Drop Collection")]
    [SerializeField] private GameObject collectUIObject;
    [SerializeField] private TMPro.TMP_Text dropCollectedText;
    [SerializeField] private AnimationCurve dropCollectedSizeOverTime;
    [SerializeField] private AnimationCurve dropCollectedOpacityOverTime;
    [SerializeField] private float dropCollectedEffectDuration;

    private Coroutine dropCollectedEffectRoutine;


    private void Start()
    {
        Debug.Assert(spellCastingController != null, "SpellCastingController reference is null");
        Debug.Assert(dropCollector != null, "DropCollector reference is null");

        primarySpell.SetSprite(spellCastingController.PrimarySpell.SpellIcon);
        if (spellCastingController.SecondarySpell)
            secondarySpell.SetSprite(spellCastingController.SecondarySpell.SpellIcon);
        dropCollectedText.text = "";

        dropCollector.DropsInRangeChanged += OnDropsInRangeChanged;
        dropCollector.DropCollected += OnDropCollected;

        spellCastingController.SpellCast += OnSpellCast;
        spellCastingController.EquippedSpellChanged += OnSpellChanged;
    }


    private void OnDropCollected(Drop obj)
    {
        if (dropCollectedEffectRoutine != null)
            StopCoroutine(dropCollectedEffectRoutine);

        dropCollectedEffectRoutine = StartCoroutine(DropCollectedUIRoutine(obj));
    }


    /// TODO: fix: If this coroutine is cancelled mid way thorugh the color gets messed up
    private IEnumerator DropCollectedUIRoutine(Drop obj)
    {
        dropCollectedText.text = obj.PickupText;
        Color initalColor = dropCollectedText.color;
        Color changedColor = initalColor;
        for (float t = 0; t < dropCollectedEffectDuration;)
        {
            float opacity = dropCollectedOpacityOverTime.Evaluate(t);
            float size = dropCollectedSizeOverTime.Evaluate(t);
            changedColor.a = initalColor.a * opacity;
            dropCollectedText.color = changedColor;
            dropCollectedText.transform.localScale = new Vector3(size, size, size);
            yield return null;
            t += Time.deltaTime;
        }
        dropCollectedText.text = "";
        dropCollectedText.color = initalColor;
    }

    private void OnDropsInRangeChanged()
    {
        collectUIObject.SetActive(dropCollector.DropsInRangeCount > 0);
    }

    private void OnSpellCast(SpellSlot spellSlot, SpellDescription spell)
    {
        switch (spellSlot)
        {
            case SpellSlot.Primary:
                primarySpell.StartSpellCastUIEffect(spell);
                break;
            case SpellSlot.Secondary:
                secondarySpell.StartSpellCastUIEffect(spell);
                break;

            default:
                Debug.LogWarning($"Unimplemented UI effect for spellSlot {spellSlot}");
                break;
        }
    }


    private void OnSpellChanged(SpellSlot slot, SpellDescription spell)
    {
        SpellSlotUI ui = slot == SpellSlot.Primary ? primarySpell : secondarySpell;
        ui.SetSprite(spell.SpellIcon);
    }


    private void Update()
    {
        float cooldown = spellCastingController.GetPrimarySpellCooldown();
        primarySpell.UpdateCooldown(cooldown);

        cooldown = spellCastingController.GetSecondarySpellCooldown();
        secondarySpell.UpdateCooldown(cooldown);
    }
}
