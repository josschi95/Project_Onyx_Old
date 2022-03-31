using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool invincible = false;
    [HideInInspector] public bool isDead = false;

    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    [Space]
    public int maxMana;
    public int currentMana { get; private set; }

    public int maxFortitude = 3; //measure of a character's ability to take a blow without flinching
    public int currentFortitude { get; private set; }

    public Stat power; //measure of character's overall strength. Gives bonus to all attacks
    public Stat armor; //measure of character's defense and worn armor, decreases damage of all incoming attacks

    public Stat critChance; //Increases likelihood of an attack being a critical hit
    public Stat critMultiplier; //Damage increase when an attack is a critical hit

    #region - Resistances -
    [Header("Resistances")]
    public Stat bludgeoningResistance;
    public Stat piercingResistance;
    public Stat slashingResistance;
    public Stat fireResistance;
    public Stat holyResistance;
    public Stat iceResistance;
    public Stat lightningResistance;
    public Stat poisonResistance;
    #endregion

    [Space]
    public float freezeTime = 0.1f;
    [Space]
    public LayerMask enemyLayers;

    public delegate void OnDeath();
    public OnDeath onDeath;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    public virtual void TakeDamage(int damage, DamageType damageType, bool critResult, Vector3 impactPosition)
    {
        if (invincible || isDead)
            return;

        CharacterCombat character = GetComponent<CharacterCombat>();
        if (character.isBlocking)
        {
            ActiveWeapon shield = GetComponentInChildren<ActiveWeapon>();
            if (shield.weapon is Shield shield2)
            {
                float blockedDamage = damage * (1 - (shield2.blockPercent / 100));
                damage = Mathf.RoundToInt(blockedDamage);
            }
        }

        #region - Damage -

        damage -= armor.GetValue(); //Later change this to a percent calculation. A max armor value is necessary. Having Absolute max armor (ALL Equipment at best) decreases damage by 80%
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        //Apply bonuses or penalties to damage due to resistances
        switch (damageType)
        {
            case DamageType.Bludgeoning:
                {
                    //Debug.Log("Bludgeoning");
                    float bludgRes = bludgeoningResistance.GetValue();
                    float roughDamage = damage * (1 - (bludgRes / 100));

                    damage = Mathf.RoundToInt(roughDamage);
                    break;
                }
            case DamageType.Piercing:
                {
                    //Debug.Log("Piercing");
                    float piercRes = piercingResistance.GetValue();
                    float roughDamage = damage * (1 - (piercRes / 100));

                    damage = Mathf.RoundToInt(roughDamage);
                    break;
                }
            case DamageType.Slashing:
                {
                    //Debug.Log("Slashing");
                    float slashRes = slashingResistance.GetValue();
                    float roughDamage = damage * (1 - (slashRes / 100));

                    damage = Mathf.RoundToInt(roughDamage);
                    break;
                }
            case DamageType.Fire:
                {
                    //Debug.Log("Fire");
                    float fireRes = fireResistance.GetValue();
                    float roughDamage = damage * (1 - (fireRes / 100));

                    damage = Mathf.RoundToInt(roughDamage);
                    break;
                }
            case DamageType.Holy:
                {
                    //Debug.Log("Holy");
                    float holyRes = holyResistance.GetValue();
                    float roughDamage = damage * (1 - (holyRes / 100));

                    damage = Mathf.RoundToInt(roughDamage);
                    break;
                }
            case DamageType.Ice:
                {
                    //Debug.Log("Ice");
                    float iceRes = iceResistance.GetValue();
                    float roughDamage = damage * (1 - (iceRes / 100));

                    damage = Mathf.RoundToInt(roughDamage);
                    break;
                }
            case DamageType.Lightning:
                {
                    //Debug.Log("Lightning");
                    float lightRes = lightningResistance.GetValue();
                    float roughDamage = damage * (1 - (lightRes / 100));

                    damage = Mathf.RoundToInt(roughDamage);
                    break;
                }
            case DamageType.Poison:
                {
                    //Debug.Log("Poison");
                    float poisRes = poisonResistance.GetValue();
                    float roughDamage = damage * (1 - (poisRes / 100));

                    damage = Mathf.RoundToInt(roughDamage);
                    break;
                }
        } 

        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        currentHealth -= damage;

        #endregion

        #region - Fortitude and Animator -
        if (critResult)
        {
            animator.Play("Crit", 0);
            animator.Play("Crit", 1);
            currentFortitude -= 3;
        }
        else if (damage > 0)
        {
            currentFortitude--;
        }

        if (currentFortitude <= 0)
        {
            animator.Play("Damage", 0);
            animator.Play("Damage", 1);
            currentFortitude = maxFortitude;
        }
        #endregion

        //Blood
        //Later add a check here to only do this if the character has blood. e.g. not for skeletons, ghosts, etc.
        if ((int)damageType <= 2 && damage > 0) //Damage type was physical, and it was not 0
        {
            Instantiate(VFXManager.instance.bloodSplatter_small, impactPosition, gameObject.transform.rotation);
        }

        Debug.Log(transform.name + " takes " + damage + " " + damageType.ToString() + " damage");

        StartCoroutine(HitFreeze());

        if (currentHealth <= 0)
            {
                Die();
            }
    }

    #region - HP & MP -
    public virtual void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public virtual void FullHeal()
    {
        currentHealth = maxHealth;
    }

    #region - Mana -

    public virtual void SpendMana(int manaAmount)
    {
        currentMana -= manaAmount;
        if (currentMana < 0)
        {
            currentMana = 0;
        }
    }

    public virtual void RestoreMana(int manaAmount)
    {
        currentMana += manaAmount;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
    }
    #endregion

    public virtual void FullRestore()
    {
        FullHeal();
        currentMana = maxMana;
    }
    #endregion

    #region - Damage Response - 

    //Add knockback force to target, disable navmeshagent if NPC
    public virtual void Knockback(Vector3 attackOrigin, float knockbackForce)
    {
        Vector3 pushDirection = attackOrigin - rb.transform.position;
        rb.AddForce(pushDirection.normalized * -knockbackForce);
    }

    IEnumerator HitFreeze()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(freezeTime);
        Time.timeScale = 1;
    }

    public virtual void CriticalHitTaken()
    {
        //Meant to be overwritten
        //Longer animation
    }

    public virtual void Die()
    {
        isDead = true;

        if (onDeath != null)
        {
            onDeath.Invoke();
        }

        this.enabled = false;
        //Meant to be overwritten
    }

    #endregion
}

public enum CreatureType { Human, Beast, Undead }