using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    [HideInInspector] public bool canDamage = false;
    CharacterCombat characterCombat;
    CharacterStats characterStats;
    CharacterController characterController;

    [Header("Arrows")]
    public GameObject basicArrow;

    private void Start()
    {
        characterCombat = GetComponentInParent<CharacterCombat>();
        characterStats = GetComponentInParent<CharacterStats>();
        characterController = GetComponentInParent<CharacterController>();
    }

    //enables colliders on weapons
    public void StartAttack()
    {
        characterCombat.ClearTargetList();
        canDamage = true;
    }

    //disables colliders on weapons
    public void EndAttack()
    {
        canDamage = false;
    }

    //Prevents character from taking damage
    public void StartInvincibility()
    {
        characterStats.invincible = true;
    }

    //Returns character to normal vulnerability
    public void EndInvincibility()
    {
        characterStats.invincible = false;
    }

    //Does not allow character to move
    public void NoInput()
    {
        characterController.acceptInput = false;
    }

    //Returns to normal character controls
    public void YesInput()
    {
        characterController.acceptInput = true;
    }

    //Adds a quick forward boost to character
    public void Lunge()
    {

        characterController.rb.AddForce(transform.forward * 600f);
    }

    public void FireProjectile()
    {
        if (characterCombat.hasBow)
        {
            ActiveWeapon bow = GetComponentInChildren<ActiveWeapon>();
            if (bow.weapon.weaponType == WeaponType.Bow)
            {
                bow.FireProjectile();
            }
        }
        else if (characterCombat.rightHand == HandState.Magic)
        {
            ActiveSpell[] preparedSpells = characterCombat.GetComponentsInChildren<ActiveSpell>();
            foreach (ActiveSpell spellBeingCast in preparedSpells)
            {
                if (spellBeingCast.activeEquipSlot == EquipmentSlot.Right_Hand)
                {
                    spellBeingCast.ReleaseSpell();
                }
            }
        }
    }

    public void FireProjectile_Secondary()
    {
        if (characterCombat.leftHand == HandState.Magic)
        {
            ActiveSpell[] preparedSpells = characterCombat.GetComponentsInChildren<ActiveSpell>();
            foreach (ActiveSpell spellBeingCast in preparedSpells)
            {
                if (spellBeingCast.activeEquipSlot == EquipmentSlot.Left_Hand)
                {
                    spellBeingCast.ReleaseSpell();
                }
            }
        }
    }

    public void ChargingAttack()
    {
        if (characterCombat.hasBow)
        {
            ActiveWeapon bow = GetComponentInChildren<ActiveWeapon>();
            if (bow.weapon.weaponType == WeaponType.Bow)
            {
                Animator newAnim = bow.GetComponentInParent<Animator>();
                newAnim.SetTrigger("Draw");
            }
        }
        else if (characterCombat.castingSpell_Primary) //This may need to change in the future based on how I implement dual casting
        {
            ActiveSpell[] preparedSpells = characterCombat.GetComponentsInChildren<ActiveSpell>();
            foreach (ActiveSpell spellBeingCast in preparedSpells)
            {
                if (spellBeingCast.activeEquipSlot == EquipmentSlot.Right_Hand)
                {
                    ParticleSystem spellParticles = spellBeingCast.spell.handFX_Primary.GetComponentInChildren<ParticleSystem>();
                    spellParticles.transform.localScale = new Vector3(2f, 2f, 2f);
                }
            }
        }
        else if (characterCombat.castingSpell_Secondary)
        {
            ActiveSpell[] preparedSpells = characterCombat.GetComponentsInChildren<ActiveSpell>();
            foreach (ActiveSpell spellBeingCast in preparedSpells)
            {
                if (spellBeingCast.activeEquipSlot == EquipmentSlot.Left_Hand)
                {
                    ParticleSystem spellParticles = spellBeingCast.spell.handFX_Secondary.GetComponentInChildren<ParticleSystem>();
                    spellParticles.transform.localScale = new Vector3(2f, 2f, 2f);
                }
            }
        }
    }

    public void ReleaseAttack()
    {
        if (characterCombat.hasBow)
        {
            ActiveWeapon bow = GetComponentInChildren<ActiveWeapon>();
            if (bow.weapon.weaponType == WeaponType.Bow)
            {
                Animator newAnim = bow.GetComponentInParent<Animator>();
                newAnim.SetTrigger("Release");
            }
        }
        else if (characterCombat.rightHand == HandState.Magic) //This may need to change in the future based on how I implement dual casting
        {
            ActiveSpell[] preparedSpells = characterCombat.GetComponentsInChildren<ActiveSpell>();
            if (preparedSpells != null)
            {
                foreach (ActiveSpell spellBeingCast in preparedSpells)
                {
                    if (spellBeingCast.spell.equipSlot == EquipmentSlot.Right_Hand)
                    {
                        ParticleSystem spellParticles = spellBeingCast.spell.handFX_Primary.GetComponentInChildren<ParticleSystem>();
                        spellParticles.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                }
            }
        }
        else if (characterCombat.leftHand == HandState.Magic)
        {
            ActiveSpell[] preparedSpells = characterCombat.GetComponentsInChildren<ActiveSpell>();
            if (preparedSpells != null)
            {
                foreach (ActiveSpell spellBeingCast in preparedSpells)
                {
                    if (spellBeingCast.spell.equipSlot == EquipmentSlot.Left_Hand)
                    {
                        ParticleSystem spellParticles = spellBeingCast.spell.handFX_Secondary.GetComponentInChildren<ParticleSystem>();
                        spellParticles.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                }
            }
        }
    }

    public void DisplaySpellMarker()
    {
        if (characterCombat is PlayerCombat playerCombat)
        {
            if (playerCombat.leftHand == HandState.Occupied && playerCombat.rightHand == HandState.Magic)
            {
                playerCombat.showSpellMarker = true;
                StartCoroutine(playerCombat.DisplaySpellMarker());
            }
        }
    }

    public void PauseGame()
    {
        GameMaster.instance.PauseGame();
    }

    public void SlowGame()
    {
        Time.timeScale = 0.1f;
    }

    public void ShowDrawnArrow()
    {
        basicArrow.SetActive(true);
    }

    public void HideDrawnArrow()
    {
        basicArrow.SetActive(false);
    }
}