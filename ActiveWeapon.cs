using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    //[Header("Wielder Info")]
    [HideInInspector] public Rigidbody parentRb; //The wielder's rb
    [HideInInspector] public LayerMask targetLayers; //damageable targets
    [HideInInspector] public CharacterStats characterStats;
    [HideInInspector] public CharacterCombat characterCombat;
    protected AnimationHelper animHelper;
    protected bool playerEquipped = false;

    [Header("Weapon Properties")]
    protected ItemPickup itemPickup; //default script to allow the item to be picked up
    protected Rigidbody rb; //rigidbody on this item
    protected Collider[] colliders; //colliders on this item
    public Weapon weapon;
    public ParticleSystem weaponFX;

    EquipmentHelper EH;
    EnemyController enemyController;
    [HideInInspector] public Arrows arrow;


    protected virtual void Start()
    {
        #region - Player Equip -
        PlayerStats playStats = GetComponentInParent<PlayerStats>();
        if (playStats != null) //This weapon is equipped by the player
        {
            EH = EquipmentHelper.instance;
            weapon.equipSlot = EquipmentManager.instance.lastEquippedWeapon; //ain't pretty but it works
            playerEquipped = true;
            if (weapon.equipSlot == EquipmentSlot.Left_Hand)
            {
                Transform target = EquipmentHelper.instance.LH_Retargeter;
                Transform parent = gameObject.transform.parent;
                parent.transform.position = target.position;
                parent.transform.rotation = target.rotation;
                parent.transform.parent = target;
                //Testing
                transform.localPosition = weapon.secondaryPosition;
                transform.localRotation = Quaternion.Euler(weapon.secondaryRotation);
            }
            else if (weapon.equipSlot == EquipmentSlot.Right_Hand)
            {
                Transform target = EquipmentHelper.instance.RH_Retargeter;
                Transform parent = gameObject.transform.parent;
                parent.transform.position = target.position;
                parent.transform.rotation = target.rotation;
                parent.transform.parent = target;

            }

            if (weapon.weaponType == WeaponType.Bow)
            {
                
                if (EquipmentManager.instance.currentEquipment[10] != null && EquipmentManager.instance.currentEquipment[10] is Arrows newArrow)
                {
                    arrow = newArrow;
                }
                
                else
                {
                    foreach (Item item in Inventory.instance.weapons)
                    {
                        if (item is Arrows foundArrow)
                        {
                            foundArrow.Use();
                            break;
                        }
                    }
                }
                
            }
        }
        else
        {
            enemyController = GetComponentInParent<EnemyController>();
        }
        #endregion

        colliders = GetComponents<Collider>();
        rb = GetComponent<Rigidbody>();
        itemPickup = GetComponentInParent<ItemPickup>();

        //If this weapon is equipped
        CharacterStats charStats = GetComponentInParent<CharacterStats>();
        if (charStats != null) //This weapon is currently equipped
        {
            characterStats = GetComponentInParent<CharacterStats>();
            characterCombat = GetComponentInParent<CharacterCombat>();
            animHelper = GetComponentInParent<AnimationHelper>();
            targetLayers = characterStats.enemyLayers;
            parentRb = characterStats.rb;
            WeaponSettings();
            gameObject.layer = characterStats.gameObject.layer;

            foreach (Collider coll in colliders)
            {
                coll.isTrigger = true;
                coll.enabled = true;
            }
            rb.isKinematic = true;
            rb.useGravity = false;
            itemPickup.enabled = false;
        }
        else //This weapon is not currently equipped
        {
            foreach (Collider coll in colliders)
            {
                coll.isTrigger = false;
                coll.enabled = true;
            }
            rb.isKinematic = false;
            rb.useGravity = true;
            itemPickup.enabled = true;
            this.enabled = false;
        }
    }

    //Trigger for striking an enemy
    protected virtual void OnTriggerEnter(Collider other)
    {
        ActiveWeapon weapon = other.GetComponent<ActiveWeapon>();
        if (weapon != null) return; //this weapon struck a weapon, not a creature

        //this weapon strikes a character
        CharacterStats target = other.GetComponentInParent<CharacterStats>();
        if (target != null && ((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            if (!characterCombat.targetsToIgnore.Contains(target) && animHelper.canDamage == true)
            {
                DealDamage(other, target);
            }
        }
    }

    //Deal damage to struck target
    protected virtual void DealDamage(Collider other, CharacterStats target)
    {
        characterCombat.targetsToIgnore.Add(target);

        if (weaponFX != null) weaponFX.Play(true);

        int totalDamage = characterStats.power.GetValue() + weapon.damage;

        #region - Critical Hit -
        bool critResult = CriticalHit();
        EnemyController enemy = other.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            if (enemy.playerDetected == false)
            {
                critResult = true;
            }
        }
        if (critResult)
        {
            totalDamage = totalDamage * characterStats.critMultiplier.GetValue();
        }
        #endregion

        //Bows ondly do half damage if bashing
        if (weapon.weaponType == WeaponType.Bow) totalDamage = Mathf.RoundToInt(totalDamage * 0.5f);

        CinemachineShake.instance.ShakeCamera(3f, 0.1f); //Add screen shake
        Vector3 impactPosition = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        if (target.GetComponent<CharacterCombat>().isBlocking)
        {
            other.GetComponentInParent<CharacterCombat>().animator.SetTrigger("shieldStruck");
            //play sword hitting shield sound effect
        }

        target.TakeDamage(totalDamage, weapon.damageType, critResult, impactPosition);

        if (weapon.knockbackForce > 0) target.Knockback(parentRb.transform.position, weapon.knockbackForce);
    }

    //Fire arrow/thrown weapon
    public void FireProjectile()
    {
        GameObject newProjectile = null;
        if (playerEquipped)
        {
            newProjectile = Instantiate(arrow.arrowPrefab, EH.arrowSpawn.position, EH.arrowSpawn.rotation);
            Inventory.instance.RemoveItem(arrow);
            HUDManager.instance.OnAmmunitionChange(arrow);
        }
        else
        {
            //Vector3 target = Random.insideUnitSphere * 2 + enemyController.playerTargetCenter.position;
            Vector3 target = enemyController.playerTargetCenter.position;
            Vector3 direction = (target - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            newProjectile = Instantiate(enemyController.enemyCombat.arrows, transform.position, lookRotation);
        }

        Projectile projectileStats = newProjectile.GetComponent<Projectile>();
        projectileStats.characterCombat = characterCombat;
        projectileStats.characterStats = characterStats;
        projectileStats.targetLayers = targetLayers;
        projectileStats.weapon = weapon;
        projectileStats.parentRb = parentRb;
    }

    //Determine if the strike was a critical hit
    protected virtual bool CriticalHit()
    {
        float critRoll = Random.Range(1f, 100f);
        if (critRoll <= (weapon.critChance + characterStats.critChance.GetValue())) return true;
        else return false;
    }

    public virtual void WeaponSettings()
    {
        if (weapon.weaponType == WeaponType.Shield) characterCombat.leftHand = HandState.Shield;

        else if (weapon.weaponType == WeaponType.Bow)
        {
            characterCombat.leftHand = HandState.Weapon;
            characterCombat.rightHand = HandState.Occupied;
            //
            //
            characterCombat.hasBow = true;
            characterStats.animator.SetFloat("weaponSpeed_1", weapon.weaponSpeed);
            characterStats.animator.SetInteger("weaponType_1", (int)weapon.weaponType);
        }
        else if (weapon.isTorch)
        {
            characterCombat.leftHand = HandState.Occupied;
            //
            //
            characterStats.animator.SetLayerWeight(2, 1);
        }
        else
        {
            if (weapon.equipSlot == EquipmentSlot.Right_Hand)
            {
                characterCombat.rightHand = HandState.Weapon;

                //The weapon is a heavy or a polearm
                if (weapon.bothHands)
                {
                    characterCombat.leftHand = HandState.Occupied;
                }
                //
                //
                characterStats.animator.SetFloat("weaponSpeed_1", weapon.weaponSpeed);
                characterStats.animator.SetInteger("weaponType_1", (int)weapon.weaponType);
            }
            else
            {
                characterCombat.leftHand = HandState.Weapon;
                //
                //
                characterStats.animator.SetFloat("weaponSpeed_2", weapon.weaponSpeed);
                characterStats.animator.SetInteger("weaponType_2", (int)weapon.weaponType);
            }
        }
    }
}