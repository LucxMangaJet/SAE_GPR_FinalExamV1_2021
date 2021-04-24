using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlotUI : MonoBehaviour
{
    [SerializeField] private Transform spellParent;
    [SerializeField] private Image spellIcon;
    [SerializeField] private Image spellActive;
    [SerializeField] private TMPro.TMP_Text spellCooldownText;


    public void SetSprite(Sprite s)
    {
        spellIcon.sprite = s;
        spellActive.enabled = false;
    }

    public void StartSpellCastUIEffect(SpellDescription spell)
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
            spellParent.localScale = Vector3.Lerp(maxSize, Vector3.one, t / spell.UIShrinkDuration);
            yield return null;
            t += Time.deltaTime;
        }
        spellParent.localScale = Vector3.one;
    }

    public void UpdateCooldown(float cooldown)
    {
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
