using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    #region - Singleton - 

    public static HUDManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of HUD Manager found");
            return;
        }
        instance = this;
    }

    #endregion

    PlayerStats playerStats;
    EquipmentManager equipmentManager;
    public Slider healthBar;
    public Slider manaBar;
    public Slider healthLagBar;
    [SerializeField] float smoothing = 5;
    [SerializeField] float delay = 5f;

    public TMP_Text ammoText;

    IEnumerator currentDelayCoroutine;

    void Start()
    {
        playerStats = PlayerStats.instance;
        equipmentManager = EquipmentManager.instance;
        healthLagBar.maxValue = playerStats.maxHealth;
        healthLagBar.value = playerStats.currentHealth;
        ammoText.text = "";
    }

    #region - HP & MP -
    //Call this function anytime the player takes damage or is healed
    public void OnHitPointChange()
    {
        bool healthGained = false;
        if (playerStats.currentHealth > healthBar.value)
        {
            healthGained = true;
        }

        healthBar.maxValue = playerStats.maxHealth;
        healthBar.value = playerStats.currentHealth;

        if (healthGained == true)
        {
            healthLagBar.value = healthBar.value;
        }
        else
        {
            if (currentDelayCoroutine != null)
            {
                StopCoroutine(currentDelayCoroutine);
            }
            currentDelayCoroutine = HealthLagDelay();
            StartCoroutine(HealthLagDelay());
        }
    }

    //Call this function anytime the player spends or gains mana
    public void OnManaChange()
    {
        manaBar.maxValue = playerStats.maxMana;
        manaBar.value = playerStats.currentMana;
    }

    IEnumerator HealthLagDelay()
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(HealthLag());
    }

    IEnumerator HealthLag()
    {
        while (healthLagBar.value != playerStats.currentHealth)
        {
            healthLagBar.value = Mathf.MoveTowards(healthLagBar.value, playerStats.currentHealth, smoothing * Time.deltaTime);
            yield return null;
        }
    }
    #endregion

    public void OnAmmunitionChange(Item item)
    {
        if (item != null)
        {
            ammoText.text = item.name + ": " + item.itemQuantity;
        }
        else
        {
            ammoText.text = "";
        }
    }
}