using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private SpellCastingController spellCastingController;
    [SerializeField] private DropCollector dropCollector;

    [SerializeField] private Transform spellParent;
    [SerializeField] private Image spellIcon;
    [SerializeField] private Image spellActive;
    [SerializeField] private TMPro.TMP_Text spellCooldownText;


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

        spellIcon.sprite = spellCastingController.SimpleAttackSpellDescription.SpellIcon;
        dropCollectedText.text = "";

        dropCollector.DropsInRangeChanged += OnDropsInRangeChanged;
        dropCollector.DropCollected += OnDropCollected;
        spellCastingController.SpellCast += OnSpellCast;
    }



    private void OnDropCollected(Drop obj)
    {
        if (dropCollectedEffectRoutine != null)
            StopCoroutine(dropCollectedEffectRoutine);

        dropCollectedEffectRoutine = StartCoroutine(DropCollectedUIRoutine(obj));
    }

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
    }

    private void OnDropsInRangeChanged()
    {
        collectUIObject.SetActive(dropCollector.DropsInRangeCount > 0);
    }

    private void OnSpellCast(SpellDescription spell)
    {
        StartCoroutine(SpellCastUIRoutine(spell));
    }

    private IEnumerator SpellCastUIRoutine(SpellDescription spell)
    {
        spellActive.enabled = true;
        Vector3 maxSize = new Vector3(spell.UIInCastMaxSize, spell.UIInCastMaxSize, spell.UIInCastMaxSize);

        //Scale up
        for (float t = 0; t < spell.Duration;)
        {

            spellParent.localScale = Vector3.Lerp(Vector3.one, maxSize, t / spell.Duration);
            yield return null;
            t += Time.deltaTime;
        }
        spellParent.localScale = maxSize;
        spellActive.enabled = false;

        //Shrink
        for (float t = 0; t < spell.UIShrinkDuration;)
        {
            spellParent.localScale = Vector3.Lerp( maxSize, Vector3.one, t / spell.UIShrinkDuration);
            yield return null;
            t += Time.deltaTime;
        }
        spellParent.localScale = Vector3.one;
    }

    private void Update()
    {
        float cooldown = spellCastingController.GetSimpleAttackCooldown();
        if (cooldown > 0)
        {
            spellCooldownText.text = cooldown.ToString("0.0");
            spellIcon.color = new Color(0.25f, 0.25f, 0.25f, 1);
        }
        else
        {
            spellCooldownText.text = "";
            spellIcon.color = Color.white;
        }
    }
}
