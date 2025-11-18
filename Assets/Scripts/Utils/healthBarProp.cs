using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBarProp : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public PropHP propHP;            // Húzd be az inspectorba, vagy hagyd üresen, akkor FindObjectOfType-ot használunk.
    public float lerpSpeed = 8f;         // nagyobb = gyorsabb "ease" követés

    void Start()
    {
        if (propHP == null)
        {
            propHP = FindObjectOfType<PropHP>(); // ha nem adtad meg az Inspectorban
        }

        if (propHP == null)
        {
            Debug.LogWarning("PropHP nem található! healthBar nem tud frissíteni.");
            return;
        }

        // állítsd be a slider határait a player maxHealth értékére
        healthSlider.maxValue = propHP.maxHealth;
        easeHealthSlider.maxValue = propHP.maxHealth;

        // kezdeti értékek
        healthSlider.value = propHP.currentHealth;
        easeHealthSlider.value = propHP.currentHealth;
    }

    void FixUpdate()
    {
        if (propHP == null) return;

        float target = propHP.currentHealth;

        // azonnali csík (ha szükséges)
        if (healthSlider.value != target)
        {
            healthSlider.value = target;
        }

        // "ease" csík sima követése (framefüggetlen)
        if (!Mathf.Approximately(easeHealthSlider.value, target))
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, target, lerpSpeed * Time.deltaTime);
        }
    }
}
