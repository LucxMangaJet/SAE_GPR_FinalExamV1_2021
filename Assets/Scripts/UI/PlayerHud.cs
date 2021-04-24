using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private SpellCastingController spellCastingController;
    [SerializeField] private DropCollector dropCollector;

    [SerializeField] private Image spellIcon;
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
