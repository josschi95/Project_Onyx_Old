using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    #region - Singleton -

    public static PlayerStats instance;

    public override void Awake()
    {
        base.Awake();

        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerStats found");
            return;
        }
        instance = this;
    }

    #endregion

    HUDManager hudManager;

    [Space]
    public Stat carryCapacity;

    float nextDamageTime;
    float damageCooldown = 0.3f;

    void Start()
    {
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
        hudManager = HUDManager.instance;
    }

    public override void TakeDamage(int damage, DamageType damageType, bool critResult, Vector3 impactPosition)
    {
        if (Time.time >= nextDamageTime)
        {
            base.TakeDamage(damage, damageType, critResult, impactPosition);
            hudManager.OnHitPointChange();
            nextDamageTime = Time.time + damageCooldown;
        }

    }

    #region - HP & MP -
    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        hudManager.OnHitPointChange();
    }

    public bool CheckManaCost(int manaCost)
    {
        if (manaCost > currentMana)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void SpendMana(int manaAmount)
    {
        base.SpendMana(manaAmount);

        hudManager.OnManaChange();
    }

    public override void RestoreMana(int manaAmount)
    {
        base.RestoreMana(manaAmount);
        hudManager.OnManaChange();
    }

    public override void FullRestore()
    {
        base.FullRestore();
        hudManager.OnHitPointChange();
        hudManager.OnManaChange();
    }
    #endregion

    #region - Stat Mods -
    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (newItem != null)
        {
            armor.AddModifier(newItem.armorModifier);
            power.AddModifier(newItem.powerModifier);
        }

        if (oldItem != null)
        {
            armor.RemoveModifier(oldItem.armorModifier);
            power.RemoveModifier(oldItem.powerModifier);
        }
    }

    public void BoostStat(Stat stat, int bonusToStat, float duration)
    {
        stat.AddModifier(bonusToStat);

        StartCoroutine(StatBoostDuration(stat, bonusToStat, duration));
    }

    IEnumerator StatBoostDuration(Stat stat, int bonusToStat, float duration)
    {
        yield return new WaitForSeconds(duration);

        stat.RemoveModifier(bonusToStat);
    }
    #endregion

    public override void Die()
    {
        base.Die();
        animator.Play("Die", 0);
        animator.Play("Die", 1);
        PlayerManager.instance.KillPlayer();
    }
}
