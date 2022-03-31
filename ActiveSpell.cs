using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSpell : MonoBehaviour
{
    //[Header("Wielder Info")]
    EquipmentHelper EH;
    EnemyController enemyController;

    [HideInInspector] public Rigidbody parentRb; //The wielder's rb
    [HideInInspector] public LayerMask targetLayers; //damageable targets
    [HideInInspector] public CharacterStats characterStats;
    [HideInInspector] public CharacterCombat characterCombat;
    [HideInInspector] public EquipmentSlot activeEquipSlot;
    [HideInInspector] public bool playerEquipped = false;
    protected AnimationHelper animHelper;
    [Header("Weapon Properties")]
    [Tooltip("Leave Empty for Projectile")]
    public Spell spell;

    protected virtual void Start()
    {
        EH = EquipmentHelper.instance;

        #region - Player Equip -
        PlayerStats playStats = GetComponentInParent<PlayerStats>();
        if (playStats != null) //This weapon is equipped by the player
        {
            spell.equipSlot = EquipmentManager.instance.lastEquippedWeapon; //ain't pretty but it works

            playerEquipped = true;
            if (spell.equipSlot == EquipmentSlot.Right_Hand)
            {
                activeEquipSlot = EquipmentSlot.Right_Hand;
                Transform target = EquipmentHelper.instance.RH_Retargeter;
                transform.position = target.position;
                transform.rotation = target.rotation;
                transform.parent = target;
            }
            else
            {
                activeEquipSlot = EquipmentSlot.Left_Hand;
                Transform target = EquipmentHelper.instance.LH_Retargeter;
                transform.position = target.position;
                transform.rotation = target.rotation;
                transform.parent = target;
            }
            
            if (spell.bothHands)
            {
                GameObject leftHand = Instantiate<GameObject>(spell.handFX_Secondary);
                Transform leftTarget = EquipmentHelper.instance.LH_Retargeter;
                leftHand.transform.position = leftTarget.position;
                leftHand.transform.rotation = leftTarget.rotation;
                leftHand.transform.parent = leftTarget;
                EquipmentManager.instance.currentWeapons[1] = leftHand;
            }
        }
        else
        {
            enemyController = GetComponentInParent<EnemyController>();
        }
        #endregion

        CharacterStats charStats = GetComponentInParent<CharacterStats>();
        if (charStats != null) //This weapon is currently equipped
        {
            characterStats = GetComponentInParent<CharacterStats>();
            characterCombat = GetComponentInParent<CharacterCombat>();
            animHelper = GetComponentInParent<AnimationHelper>();
            targetLayers = characterStats.enemyLayers;
            parentRb = characterStats.rb;
            SpellSettings();
            gameObject.layer = characterStats.gameObject.layer;
        }
        else //This weapon is not currently equipped
        {
            Debug.Log("ERROR: SPELL EFFECT NOT EQUIPPED");
            Destroy(this);
        }
    }

    public virtual void SpellSettings()
    {
        if (spell.equipSlot == EquipmentSlot.Right_Hand)
        {
            characterCombat.rightHand = HandState.Magic;
            characterStats.animator.SetFloat("weaponSpeed_1", spell.castingSpeed);
            characterStats.animator.SetInteger("weaponType_1", 5);
        }
        else
        {
            characterCombat.leftHand = HandState.Magic;
            characterStats.animator.SetFloat("weaponSpeed_2", spell.castingSpeed);
            characterStats.animator.SetInteger("weaponType_2", 5);
        }

        if (spell.bothHands)
        {
            characterCombat.leftHand = HandState.Occupied;
        }
    }

    public void ReleaseSpell()
    {
        switch (spell.spellShape)
        {
            case SpellShape.Projectile:
                {
                    FireProjectile();
                    break;
                }
            case SpellShape.A_o_E:
                {
                    CastAoE();
                    break;
                }
            case SpellShape.Nova:
                {
                    UnleashNova();
                    break;
                }
        }
    }

    //If spellShape is projectile
    public void FireProjectile()
    {
        if (playerEquipped)
        {
            GameObject newProjectile = Instantiate(spell.spellPrefab, EH.arrowSpawn.position, EH.arrowSpawn.rotation);

            SpellProjectile projectileStats = newProjectile.GetComponent<SpellProjectile>();
            projectileStats.characterCombat = characterCombat;
            projectileStats.characterStats = characterStats;
            projectileStats.targetLayers = targetLayers;
            projectileStats.spell = spell;
            projectileStats.parentRb = parentRb;

            PlayerStats.instance.SpendMana(spell.manaCost);
        }
        else
        {
            Vector3 direction = (enemyController.playerTargetCenter.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            GameObject newProjectile = Instantiate(spell.spellPrefab, transform.position, lookRotation);

            SpellProjectile projectileStats = newProjectile.GetComponent<SpellProjectile>();
            projectileStats.characterCombat = characterCombat;
            projectileStats.characterStats = characterStats;
            projectileStats.targetLayers = targetLayers;
            projectileStats.spell = spell;
        }
    }

    public void UnleashNova()
    {
        GameObject newNova = Instantiate(spell.spellPrefab, transform.position, transform.rotation);
        newNova.transform.parent = characterStats.transform;
        newNova.transform.rotation = new Quaternion(0, 0, 0, 0);


        if (playerEquipped)
        {
            PlayerStats.instance.SpendMana(spell.manaCost);

            GameObject oldSpell = null;
            foreach (GameObject activeSpell in EquipmentManager.instance.currentlyActiveSpells)
            {
                if (activeSpell.name == newNova.name)
                {
                    oldSpell = activeSpell;

                }
            }
            if (oldSpell != null)
            {
                Destroy(oldSpell);
                EquipmentManager.instance.currentlyActiveSpells.Remove(oldSpell);
            }

            EquipmentManager.instance.currentlyActiveSpells.Add(newNova);
        }

        SpellNova novaStats = newNova.GetComponent<SpellNova>();
        novaStats.characterCombat = characterCombat;
        novaStats.characterStats = characterStats;
        novaStats.targetLayers = targetLayers;
        novaStats.spell = spell;
        novaStats.parentRb = parentRb;
    }

    public void CastAoE()
    {
        GameObject newSpell = Instantiate(spell.spellPrefab, transform.position, transform.rotation);
        newSpell.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        if (playerEquipped && characterCombat is PlayerCombat playerCombat)
        {
            PlayerStats.instance.SpendMana(spell.manaCost);
            newSpell.transform.position = playerCombat.spellMarker.transform.position;
            playerCombat.showSpellMarker = false;
        }

        SpellNova novaStats = newSpell.GetComponent<SpellNova>();
        novaStats.characterCombat = characterCombat;
        novaStats.characterStats = characterStats;
        novaStats.targetLayers = targetLayers;
        novaStats.spell = spell;
        novaStats.parentRb = parentRb;
    }
}