using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class healthBar : NetworkBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public PlayerHP playerHP;            // Húzd be az inspectorba, vagy hagyd üresen, akkor FindObjectOfType-ot használunk.
    public float lerpSpeed = 8f;         // nagyobb = gyorsabb "ease" követés

    void Start()
    {
        if (playerHP == null)
        {
            playerHP = FindObjectOfType<PlayerHP>(); // ha nem adtad meg az Inspectorban
        }

        if (playerHP == null)
        {
            Debug.LogWarning("PlayerHP nem található! healthBar nem tud frissíteni.");
            return;
        }

        // állítsd be a slider határait a player maxHealth értékére
        healthSlider.maxValue = playerHP.maxHealth;
        easeHealthSlider.maxValue = playerHP.maxHealth;

        // kezdeti értékek
        healthSlider.value = playerHP.currentHealth;
        easeHealthSlider.value = playerHP.currentHealth;
    }

    void FixUpdate()
    {
        if (playerHP == null) return;

        float target = playerHP.currentHealth;

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
