using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCombat : CharacterCombat
{
    #region - Singleton -

    public static PlayerCombat instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerCombat found");
            return;
        }

        instance = this;
    }

    #endregion
    Inventory inventory;
    PlayerController playerController;
    AimCameraController aimCam;

    public Transform clavicle_L;
    public Transform clavicle_R;
    public Camera cam;
    public GameObject spellMarker;
    [SerializeField] Transform spellMarkerRadius;
    //[HideInInspector] 
    public bool showSpellMarker = false;

    public override void Start()
    {
        base.Start();
        inventory = Inventory.instance;
        playerController = GetComponent<PlayerController>();
        aimCam = GetComponent<AimCameraController>();
    }

    #region - Aim Up/Down -
    private void LateUpdate()
    {
        TiltShoulders();
    }

    void TiltShoulders()
    {
        if (aimCam.aimCamActive)
        {
            float shoulderL = playerController.turn.y - 45;
            clavicle_L.transform.localRotation = Quaternion.Euler(clavicle_L.transform.localEulerAngles.x, clavicle_L.transform.localEulerAngles.y, shoulderL);
        }
    }
    #endregion

    #region - Primary Hand -
    //RH attack or shield bash
    public void Press_Primary()
    {
        if (GameMaster.instance.gamePaused) return;
        //There is a weapon in the player's right hand
        if (rightHand == HandState.Weapon) animator.SetTrigger("Attack1");
        //Shield bash
        else if (leftHand == HandState.Shield && isBlocking) animator.SetTrigger("Attack1");
    }

    //RH powerAttack, begin to cast RH spell, or draw bow
    public void Hold_Primary()
    {
        if (GameMaster.instance.gamePaused) return;
        //Right hand power attack
        if (rightHand == HandState.Weapon) animator.SetTrigger("powerAttack");
        //There is a spell prepared in the player's right hand
        else if (rightHand == HandState.Magic) PrepareAttack_Primary();
        //this is currently only for bows, but may need to make a change in the future
        else if (leftHand == HandState.Weapon && rightHand == HandState.Occupied) PrepareAttack_Primary();
    }

    //Draw bow or begin casting spell
    public void PrepareAttack_Primary()
    {
        if (GameMaster.instance.gamePaused) return;

        //Player has a bow equipped
        if (rightHand == HandState.Occupied && leftHand == HandState.Weapon)
        {
            ActiveWeapon bow = GetComponentInChildren<ActiveWeapon>();
            if (bow.weapon.weaponType == WeaponType.Bow)
            {
                Arrows equippedArrow = bow.arrow;
                if (equippedArrow == null)
                {
                    foreach (Item item in Inventory.instance.weapons)
                    {
                        if (item is Arrows foundArrow)
                        {
                            foundArrow.Use();
                            break;
                        }
                    }
                    Debug.Log("No Arrows");
                    return;
                }
            }

            StartCoroutine(aimCam.AimZoomIn());
            bowDrawn = true;
            animator.SetBool("bowDrawn", bowDrawn);
        }
        else if (rightHand == HandState.Magic)
        {
            ActiveSpell[] preparedSpells = GetComponentsInChildren<ActiveSpell>();
            foreach (ActiveSpell spellBeingCast in preparedSpells)
            {
                if (spellBeingCast.activeEquipSlot == EquipmentSlot.Right_Hand)
                {

                    if (PlayerStats.instance.CheckManaCost(spellBeingCast.spell.manaCost))
                    {
                        castingSpell_Primary = true;
                        animator.SetBool("castingSpell_Primary", castingSpell_Primary);
                    }
                    else
                    {
                        if (castingSpell_Primary == true) //Lets see what happens without this
                        {
                            //castingSpell_Primary = false;
                        }
                        Debug.Log("Not enough Mana");
                        return;
                    }
                }
            }
        }
    }


    //Attack with Right hand, shield bash, or fire bow
    public void ReleaseAttack_Primary()
    {
        if (rightHand == HandState.Magic)
        {
            castingSpell_Primary = false;
            animator.SetBool("castingSpell_Primary", castingSpell_Primary);
        }
        //again, this currently only applies to bows
        else if (leftHand == HandState.Weapon && rightHand == HandState.Occupied)
        {
            bowDrawn = false;
            animator.SetBool("bowDrawn", bowDrawn);
            StartCoroutine(aimCam.AimzoomOut());
        }
    }
    #endregion

    #region - Secondary/Off Hand -
    //Off-hand, this is only going to be for shields, weapons, and spells
    public void Press_Secondary()
    {
        if (GameMaster.instance.gamePaused) return;
        //There is a weapon in the player's left hand but not a bow
        if (leftHand == HandState.Weapon && rightHand != HandState.Occupied) animator.SetTrigger("Attack2");
    }

    //LH power attack, begin to cast spell, or block
    public void Hold_Secondary()
    {
        if (GameMaster.instance.gamePaused) return;
        if (leftHand == HandState.Weapon && rightHand != HandState.Occupied) animator.SetTrigger("powerAttack"); //Right now this will only trigger a RH power attack, need to differentiate
        else if (leftHand == HandState.Magic) PrepareAttack_Secondary();
        else if (leftHand == HandState.Shield)
        {
            isBlocking = true;
            animator.SetBool("Blocking", isBlocking);
        }
    }

    //stop blocking or cast spell in secondary hand, 
    public void ReleaseAttack_Secondary()
    {
        if (leftHand == HandState.Shield)
        {
            isBlocking = false;
            animator.SetBool("Blocking", isBlocking);
        }
        else if (leftHand == HandState.Magic)
        {
            castingSpell_Secondary = false;
            animator.SetBool("castingSpell_Secondary", castingSpell_Secondary);
        }
    }

    //LH spellcasting
    public void PrepareAttack_Secondary()
    {
        if (GameMaster.instance.gamePaused) return;

        if (leftHand == HandState.Magic)
        {
            ActiveSpell[] preparedSpells = GetComponentsInChildren<ActiveSpell>();
            foreach (ActiveSpell spellBeingCast in preparedSpells)
            {
                if (spellBeingCast.activeEquipSlot == EquipmentSlot.Left_Hand)
                {
                    if (PlayerStats.instance.CheckManaCost(spellBeingCast.spell.manaCost))
                    {
                        castingSpell_Secondary = true;
                        animator.SetBool("castingSpell_Secondary", castingSpell_Secondary);
                    }
                    else
                    {
                        if (castingSpell_Secondary == true) //Lets see what happens without this
                        {
                            //castingSpell_Primary = false;
                        }
                        Debug.Log("Not enough Mana");
                        return;
                    }
                }
            }
        }
    }
    #endregion

    public IEnumerator DisplaySpellMarker()
    {
        Spell spell = GetComponentInChildren<ActiveSpell>().spell;
        if (spell != null)
        {
            spellMarker.SetActive(true);
            while (showSpellMarker)
            {
                Debug.Log("Displaying Spell marker");
                Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 20))//spell.spellRange))
                {
                    Vector3 target = hit.point;
                    Debug.DrawLine(cam.transform.position, target);
                    spellMarker.transform.position = target;
                }
                yield return null;
            }
            spellMarker.SetActive(false);
            spellMarker.transform.position = Vector3.zero;
        }
    }
}
