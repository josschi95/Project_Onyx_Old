using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Keep track of equipment. Has functions for adding and removing items */

public class EquipmentManager : MonoBehaviour
{
    #region - Singleton -

    public static EquipmentManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EquipmentManager found");
            return;
        }
        instance = this;
    }

    #endregion

    EquipmentHelper EH;
    Inventory inventory;
    PlayerCombat playerCombat;

    public Equipment[] currentEquipment; //Items we currently have equipped
    //[HideInInspector] 
    public GameObject[] currentWeapons; //The GOs we are spawning into the scene
    [HideInInspector] public EquipmentSlot lastEquippedWeapon;
    [HideInInspector] public List<GameObject> currentlyActiveSpells = new List<GameObject>();

    public float apparelWeight = 1;

    //Callback for when an item is equipped/unequipped
    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    private bool longSleeves = false; //Used to overwrite forearm meshes on hands when equipping long-sleeved armor
    //private bool adjustHands = false; //Used to reset gloves when unequipping armor that overwrites forearm mesh

    #region - Base Meshes -
    [Header("Base Meshes")]
    public SkinnedMeshRenderer baseHead;
    public SkinnedMeshRenderer baseHair;
    public SkinnedMeshRenderer baseEyebrows;
    public SkinnedMeshRenderer baseFacialhair;
    public SkinnedMeshRenderer baseChest;
    public SkinnedMeshRenderer[] baseUpperArms;
    public SkinnedMeshRenderer[] baseLowerArms;
    public SkinnedMeshRenderer[] baseHands;
    public SkinnedMeshRenderer baseLegs;
    public SkinnedMeshRenderer[] baseFeet;
    Mesh baseLegMesh;

    #endregion

    #region - Equipped Meshes -
    private List<SkinnedMeshRenderer> equippedHeadMeshes = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> equippedChestMeshes = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> equippedHandMeshes = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> equippedFeetMeshes = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> equippedShoulderMeshes = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> equippedKneeMeshes = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> equippedElbowMeshes = new List<SkinnedMeshRenderer>();
    private SkinnedMeshRenderer equippedCapeMesh = null;
    #endregion

    private void Start()
    {
        EH = EquipmentHelper.instance; //EH.
        inventory = Inventory.instance;
        playerCombat = PlayerCombat.instance;

        //Initialize currentEquipment based on number of equipment slots
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
        currentWeapons = new GameObject[3]; //[numSlots];

        baseLegMesh = baseLegs.sharedMesh;
    }

    //Equip a new item
    public void Equip(Equipment newItem)
    {
        //Find out what slot the item fits in
        int slotIndex = (int)newItem.equipSlot; //This part is working correctly
        Equipment oldItem = Unequip(slotIndex);

        //Update Menu display
        newItem.isEquipped = true;
        UiManager.instance.UpdateMenu();

        //An item has been equipped so we trigger the callback
        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        switch (newItem)
        {
            //newItem is a weapon
            case Weapon weapon:
                {
                    switch (weapon.weaponType)
                    {
                        case WeaponType.Shield:
                            {
                                if (currentEquipment[0] != null)
                                {
                                    if (currentEquipment[0] is Weapon twoHander && twoHander.bothHands)
                                    {
                                        Unequip(0);
                                    }
                                    else if (currentEquipment[0] is Spell doubleSpell && doubleSpell.bothHands)
                                    {
                                        Unequip(0);
                                    }
                                }
                                break;
                            }
                        case WeaponType.One_Handed:
                            {
                                //Check if there is a two-handed weapon in the left hand, currently this is only bows
                                if (weapon.equipSlot == EquipmentSlot.Right_Hand)
                                {
                                    if (currentEquipment[1] != null && currentEquipment[1] is Weapon twoHands)
                                    {
                                        if (twoHands.bothHands)
                                        {
                                            Unequip(1);
                                        }
                                    }
                                }

                                //If equipping a left-handed weapon
                                if (weapon.equipSlot == EquipmentSlot.Left_Hand && currentEquipment[0] != null)
                                {
                                    if (currentEquipment[0] is Weapon twoHand && twoHand.bothHands)
                                    {
                                        Unequip(0);
                                    }
                                    else if (currentEquipment[0] is Spell doublSpell && doublSpell.bothHands)
                                    {
                                        Unequip(0);
                                    }
                                }
                                break;
                            }
                        case WeaponType.Heavy:
                            {
                                //Only need to check left hand because this will replace anything in the right hand
                                if (currentEquipment[1] != null)
                                {
                                    Unequip(1);
                                }
                                break;
                            }
                        case WeaponType.Pole:
                            {
                                if (currentEquipment[1] != null)
                                {
                                    Unequip(1);
                                }
                                break;
                            }
                        case WeaponType.Bow:
                            {
                                if (currentEquipment[0] != null)
                                {
                                    Unequip(0);
                                }
                                break;
                            }
                    }
                    //Instantiate new go into gameworld
                    GameObject newObj = null;
                    newObj = Instantiate<GameObject>(weapon.activePrefab);
                    //Parents new go to player
                    newObj.transform.parent = PlayerManager.instance.player.transform;
                    //insert into go equipObj aray
                    currentWeapons[slotIndex] = newObj;
                    lastEquippedWeapon = newItem.equipSlot;
                    break;
                }
            case Arrows arrow:
                {
                    GameObject newObj = Instantiate<GameObject>(arrow.quiver);
                    newObj.transform.position = EH.cape_rootBone.position;
                    newObj.transform.rotation = EH.cape_rootBone.rotation;
                    newObj.transform.parent = EH.cape_rootBone.transform;
                    currentWeapons[2] = newObj;

                    if (currentEquipment[1] != null && currentEquipment[1] is Weapon weapon)
                    {
                        if (weapon.weaponType == WeaponType.Bow)
                        {
                            ActiveWeapon bow = playerCombat.GetComponentInChildren<ActiveWeapon>();
                            if (bow != null && bow.weapon.weaponType == WeaponType.Bow)
                            {
                                bow.arrow = arrow;
                            }
                        }
                    }

                    HUDManager.instance.OnAmmunitionChange(arrow);

                    break;
                }
            case Apparel apparel:
                {
                    switch (apparel)
                    {
                        case Apparel_Head head:
                            {
                                switch (head.coverType)
                                {
                                    case CoverType.Base_Hair:
                                        {
                                            SkinnedMeshRenderer baseHairMesh = Instantiate<SkinnedMeshRenderer>(head.baseHairMesh);

                                            if (head.boneStructure == 0)
                                            {
                                                baseHairMesh.transform.parent = EH.head_Base_RootBone_v0.transform;
                                                baseHairMesh.bones = EH.head_Base_Bones_v0;
                                                baseHairMesh.rootBone = EH.head_Base_RootBone_v0;
                                            }
                                            else if (head.boneStructure == 1)
                                            {
                                                baseHairMesh.transform.parent = EH.head_Base_RootBone_v1.transform;
                                                baseHairMesh.bones = EH.head_Base_Bones_v1;
                                                baseHairMesh.rootBone = EH.head_Base_RootBone_v1;
                                            }
                                            else if (head.boneStructure == 2)
                                            {
                                                baseHairMesh.transform.parent = EH.head_Base_RootBone_v2.transform;
                                                baseHairMesh.bones = EH.head_Base_Bones_v2;
                                                baseHairMesh.rootBone = EH.head_Base_RootBone_v2;
                                            }
                                            else if (head.boneStructure == 3)
                                            {
                                                baseHairMesh.transform.parent = EH.head_Base_RootBone_v3.transform;
                                                baseHairMesh.bones = EH.head_Base_Bones_v3;
                                                baseHairMesh.rootBone = EH.head_Base_RootBone_v3;
                                            }
                                            else if (head.boneStructure == 4)
                                            {
                                                baseHairMesh.transform.parent = EH.head_Base_RootBone_v4.transform;
                                                baseHairMesh.bones = EH.head_Base_Bones_v4;
                                                baseHairMesh.rootBone = EH.head_Base_RootBone_v4;
                                            }
                                            equippedHeadMeshes.Add(baseHairMesh);
                                            break;
                                        }
                                    case CoverType.No_Hair:
                                        {
                                            SkinnedMeshRenderer noHairMesh = Instantiate<SkinnedMeshRenderer>(head.noHairMesh);

                                            if (head.boneStructure == 0)
                                            {
                                                noHairMesh.transform.parent = EH.head_noHair_RootBone_v0.transform;
                                                noHairMesh.bones = EH.head_noHair_Bones_v0;
                                                noHairMesh.rootBone = EH.head_noHair_RootBone_v0;
                                            }
                                            else if (head.boneStructure == 1)
                                            {
                                                noHairMesh.transform.parent = EH.head_noHair_RootBone_v1.transform;
                                                noHairMesh.bones = EH.head_noHair_Bones_v1;
                                                noHairMesh.rootBone = EH.head_noHair_RootBone_v1;
                                            }
                                            else if (head.boneStructure == 2)
                                            {
                                                noHairMesh.transform.parent = EH.head_noHair_RootBone_v2.transform;
                                                noHairMesh.bones = EH.head_noHair_Bones_v2;
                                                noHairMesh.rootBone = EH.head_noHair_RootBone_v2;
                                            }
                                            equippedHeadMeshes.Add(noHairMesh);
                                            if (baseHair != null)
                                            {
                                                baseHair.enabled = false;
                                            }
                                            break;
                                        }
                                    case CoverType.No_Face_Hair:
                                        {
                                            SkinnedMeshRenderer noFaceHairMesh = Instantiate<SkinnedMeshRenderer>(head.noFacialHairMesh);

                                            if (head.boneStructure == 0)
                                            {
                                                noFaceHairMesh.transform.parent = EH.head_noFace_RootBone_v0.transform;
                                                noFaceHairMesh.bones = EH.head_noFace_Bones_v0;
                                                noFaceHairMesh.rootBone = EH.head_noFace_RootBone_v0;
                                            }
                                            else if (head.boneStructure == 1)
                                            {
                                                noFaceHairMesh.transform.parent = EH.head_noFace_RootBone_v1.transform;
                                                noFaceHairMesh.bones = EH.head_noFace_Bones_v1;
                                                noFaceHairMesh.rootBone = EH.head_noFace_RootBone_v1;
                                            }
                                            else if (head.boneStructure == 2)
                                            {
                                                noFaceHairMesh.transform.parent = EH.head_noFace_RootBone_v2.transform;
                                                noFaceHairMesh.bones = EH.head_noFace_Bones_v2;
                                                noFaceHairMesh.rootBone = EH.head_noFace_RootBone_v2;
                                            }
                                            equippedHeadMeshes.Add(noFaceHairMesh);
                                            if (baseFacialhair != null)
                                            {
                                                baseFacialhair.enabled = false;
                                            }
                                            break;
                                        }
                                    case CoverType.Full:
                                        {
                                            SkinnedMeshRenderer noElementMesh = Instantiate<SkinnedMeshRenderer>(head.fullMesh);
                                            noElementMesh.transform.parent = EH.head_Full_RootBone.transform;

                                            noElementMesh.bones = EH.head_Full_Bones;
                                            noElementMesh.rootBone = EH.head_Full_RootBone;
                                            equippedHeadMeshes.Add(noElementMesh);

                                            baseHead.enabled = false;
                                            baseEyebrows.enabled = false;

                                            if (baseHair != null)
                                            {
                                                baseHair.enabled = false;
                                            }
                                            if (baseFacialhair != null)
                                            {
                                                baseFacialhair.enabled = false;
                                            }
                                            break;
                                        }
                                }
                                if (head.hasAttachment == true)
                                {
                                    SkinnedMeshRenderer headAttachMesh = Instantiate<SkinnedMeshRenderer>(head.headAttachmentMesh);
                                    headAttachMesh.transform.parent = EH.head_Attach_RootBone.transform;

                                    headAttachMesh.bones = EH.head_Attach_Bones;
                                    headAttachMesh.rootBone = EH.head_Attach_RootBone;
                                    equippedHeadMeshes.Add(headAttachMesh);
                                }
                                break;
                            }
                        case Apparel_Chest chest:
                            {
                                SkinnedMeshRenderer chestMesh = Instantiate<SkinnedMeshRenderer>(chest.bodyMesh);
                                chestMesh.transform.parent = EH.chestRootBone.transform;

                                chestMesh.bones = EH.chestBones;
                                chestMesh.rootBone = EH.chestRootBone;
                                equippedChestMeshes.Add(chestMesh);
                                baseChest.enabled = false;

                                //Top and Bottom

                                SkinnedMeshRenderer newLegMesh = Instantiate<SkinnedMeshRenderer>(chest.legMesh);
                                newLegMesh.transform.parent = EH.legsRootBone.transform;

                                if (chest.legsVariant == false)
                                {
                                    newLegMesh.bones = EH.legBones_v0;
                                }
                                else
                                {
                                    newLegMesh.bones = EH.legBones_v1;
                                }

                                newLegMesh.rootBone = EH.legsRootBone;
                                equippedChestMeshes.Add(newLegMesh);

                                //Hides the default mesh
                                baseLegs.sharedMesh = null;

                                if (chest.hasArms)
                                {
                                    SkinnedMeshRenderer L_upperArmMesh = Instantiate<SkinnedMeshRenderer>(chest.upperArmMesh_L);
                                    L_upperArmMesh.transform.parent = EH.UA_L_rootBone.transform;

                                    L_upperArmMesh.bones = EH.UA_L_Bones;
                                    L_upperArmMesh.rootBone = EH.UA_L_rootBone;
                                    equippedChestMeshes.Add(L_upperArmMesh);

                                    //Left and Right

                                    SkinnedMeshRenderer R_upperArmMesh = Instantiate<SkinnedMeshRenderer>(chest.upperArmMesh_R);
                                    R_upperArmMesh.transform.parent = EH.UA_R_rootBone.transform;

                                    R_upperArmMesh.bones = EH.UA_R_Bones;
                                    R_upperArmMesh.rootBone = EH.UA_R_rootBone;
                                    equippedChestMeshes.Add(R_upperArmMesh);

                                    foreach (SkinnedMeshRenderer UAMesh in baseUpperArms)
                                    {
                                        UAMesh.enabled = false;
                                    }
                                }
                                if (chest.longSleeves)
                                {
                                    SkinnedMeshRenderer L_lowerArmMesh = Instantiate<SkinnedMeshRenderer>(chest.lowerArmMesh_L);
                                    L_lowerArmMesh.transform.parent = EH.LA_L_rootBone.transform;

                                    L_lowerArmMesh.bones = EH.LA_L_Bones;
                                    L_lowerArmMesh.rootBone = EH.LA_L_rootBone;
                                    equippedChestMeshes.Add(L_lowerArmMesh);

                                    //Left and Right

                                    SkinnedMeshRenderer R_lowerArmMesh= Instantiate<SkinnedMeshRenderer>(chest.lowerArmMesh_R);
                                    R_lowerArmMesh.transform.parent = EH.LA_R_rootBone.transform;

                                    R_lowerArmMesh.bones = EH.LA_R_Bones;
                                    R_lowerArmMesh.rootBone = EH.LA_R_rootBone;
                                    equippedChestMeshes.Add(R_lowerArmMesh);

                                    foreach (SkinnedMeshRenderer LAMesh in baseLowerArms)
                                    {
                                        LAMesh.enabled = false;
                                    }

                                    longSleeves = true;
                                    Apparel_Hands currentHands = currentEquipment[4] as Apparel_Hands;
                                    if (currentHands != null && currentHands.hasForeArms == true)
                                    {
                                        //adjustHands = true;
                                        Equip(currentHands);
                                    }
                                }
                                break;
                            }
                        case Apparel_Hands hands:
                            {
                                if (hands.hasHands == true)
                                {
                                    SkinnedMeshRenderer L_handMesh = Instantiate<SkinnedMeshRenderer>(hands.handMesh_L);
                                    L_handMesh.transform.parent = EH.hand_L_rootBone.transform;

                                    L_handMesh.bones = EH.hand_L_Bones;
                                    L_handMesh.rootBone = EH.hand_L_rootBone;
                                    equippedHandMeshes.Add(L_handMesh);

                                    //Left and Right

                                    SkinnedMeshRenderer R_handMesh = Instantiate<SkinnedMeshRenderer>(hands.handMesh_R);
                                    R_handMesh.transform.parent = EH.hand_R_rootBone.transform;

                                    R_handMesh.bones = EH.hand_R_Bones;
                                    R_handMesh.rootBone = EH.hand_R_rootBone;
                                    equippedHandMeshes.Add(R_handMesh);

                                    foreach (SkinnedMeshRenderer handMesh in baseHands)
                                    {
                                        handMesh.enabled = false;
                                    }
                                }

                                if (hands.hasForeArms == true && longSleeves == false)
                                {
                                    SkinnedMeshRenderer L_forearmMesh = Instantiate<SkinnedMeshRenderer>(hands.forearmMesh_L);
                                    L_forearmMesh.transform.parent = EH.LA_L_rootBone.transform;

                                    L_forearmMesh.bones = EH.LA_L_Bones;
                                    L_forearmMesh.rootBone = EH.LA_L_rootBone;
                                    equippedHandMeshes.Add(L_forearmMesh);

                                    //Left and Right

                                    SkinnedMeshRenderer R_forearmMesh = Instantiate<SkinnedMeshRenderer>(hands.forearmMesh_R);
                                    R_forearmMesh.transform.parent = EH.LA_R_rootBone.transform;

                                    R_forearmMesh.bones = EH.LA_R_Bones;
                                    R_forearmMesh.rootBone = EH.LA_R_rootBone;
                                    equippedHandMeshes.Add(R_forearmMesh);

                                    foreach (SkinnedMeshRenderer LAMesh in baseLowerArms)
                                    {
                                        LAMesh.enabled = false;
                                    }
                                }
                                break;
                            }
                        case Apparel_Feet feet:
                            {
                                SkinnedMeshRenderer L_footMesh = Instantiate<SkinnedMeshRenderer>(feet.footMesh_L);
                                L_footMesh.transform.parent = EH.LowerLeg_L_rootBone.transform;

                                L_footMesh.bones = EH.LowerLeg_L_Bones;
                                L_footMesh.rootBone = EH.LowerLeg_L_rootBone.transform;
                                equippedFeetMeshes.Add(L_footMesh);

                                //Left and Right

                                SkinnedMeshRenderer R_footMesh = Instantiate<SkinnedMeshRenderer>(feet.footMesh_R);
                                R_footMesh.transform.parent = EH.LowerLeg_R_rootBone.transform;

                                R_footMesh.bones = EH.LowerLeg_R_Bones;
                                R_footMesh.rootBone = EH.LowerLeg_R_rootBone.transform;
                                equippedFeetMeshes.Add(R_footMesh);

                                foreach (SkinnedMeshRenderer feetMesh in baseFeet)
                                {
                                    feetMesh.enabled = false;
                                }
                                break;
                            }
                        case Apparel_Accessories accessory:
                            {
                                switch (accessory.accessoryType)
                                {
                                    case AccessoryType.Pauldrons:
                                        {
                                            SkinnedMeshRenderer L_shoulderMesh = Instantiate<SkinnedMeshRenderer>(accessory.pauldronMesh_L);
                                            L_shoulderMesh.transform.parent = EH.Shoulder_L_rootBone.transform;

                                            L_shoulderMesh.bones = EH.Shoulder_L_Bones;
                                            L_shoulderMesh.rootBone = EH.Shoulder_L_rootBone;
                                            equippedShoulderMeshes.Add(L_shoulderMesh);

                                            //Left and Right

                                            SkinnedMeshRenderer R_shoulderMesh = Instantiate<SkinnedMeshRenderer>(accessory.pauldronMesh_R);
                                            R_shoulderMesh.transform.parent = EH.Shoulder_R_rootBone.transform;

                                            R_shoulderMesh.bones = EH.Shoulder_R_Bones;
                                            R_shoulderMesh.rootBone = EH.Shoulder_R_rootBone;
                                            equippedShoulderMeshes.Add(R_shoulderMesh);
                                            break;
                                        }
                                    case AccessoryType.Cape:
                                        {
                                            SkinnedMeshRenderer capeMesh = Instantiate<SkinnedMeshRenderer>(accessory.capeMesh);
                                            capeMesh.transform.parent = EH.cape_rootBone.transform;

                                            if (accessory.capeBoneStruct == 0)
                                            {
                                                capeMesh.bones = EH.cape_Bones_v0;
                                            }
                                            else if (accessory.capeBoneStruct == 1)
                                            {
                                                capeMesh.bones = EH.cape_Bones_v1;
                                            }
                                            else
                                            {
                                                Debug.Log("ERROR");
                                            }

                                            capeMesh.rootBone = EH.cape_rootBone;
                                            equippedCapeMesh = capeMesh;
                                            break;
                                        }
                                    case AccessoryType.Elbowpads:
                                        {
                                            SkinnedMeshRenderer L_elbowMesh = Instantiate<SkinnedMeshRenderer>(accessory.elbowMesh_L);
                                            L_elbowMesh.transform.parent = EH.Elbow_L_rootBone.transform;

                                            L_elbowMesh.bones = EH.Elbow_L_Bones;
                                            L_elbowMesh.rootBone = EH.Elbow_L_rootBone;
                                            equippedElbowMeshes.Add(L_elbowMesh);

                                            //Left and Right

                                            SkinnedMeshRenderer R_elbowMesh = Instantiate<SkinnedMeshRenderer>(accessory.elbowMesh_R);
                                            R_elbowMesh.transform.parent = EH.Elbow_R_rootBone.transform;

                                            R_elbowMesh.bones = EH.Elbow_R_Bones;
                                            R_elbowMesh.rootBone = EH.Elbow_R_rootBone;
                                            equippedElbowMeshes.Add(R_elbowMesh);
                                            break;
                                        }
                                    case AccessoryType.Kneepads:
                                        {
                                            SkinnedMeshRenderer L_kneeMesh = Instantiate<SkinnedMeshRenderer>(accessory.kneeMesh_L);
                                            L_kneeMesh.transform.parent = EH.Knee_L_rootBone.transform;

                                            L_kneeMesh.bones = EH.Knee_L_Bones;
                                            L_kneeMesh.rootBone = EH.Knee_L_rootBone;
                                            equippedKneeMeshes.Add(L_kneeMesh);

                                            //Left and Right

                                            SkinnedMeshRenderer R_kneeMesh = Instantiate<SkinnedMeshRenderer>(accessory.kneeMesh_R);
                                            R_kneeMesh.transform.parent = EH.Knee_R_rootBone.transform;

                                            R_kneeMesh.bones = EH.Knee_R_Bones;
                                            R_kneeMesh.rootBone = EH.Knee_R_rootBone;
                                            equippedKneeMeshes.Add(R_kneeMesh);
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                    //Add weight of armor to wornEquipmentWeight
                    apparelWeight += apparel.weight;
                    apparelWeight = Mathf.Clamp(apparelWeight, 1, 60);
                    break;
                }
            case Spell spell:
                {
                    if (spell.bothHands)
                    {
                        if (currentEquipment[1] != null)
                        {
                            Unequip(1);
                        }
                    }

                    //Instantiate new go into gameworld
                    GameObject handFX = null;
                    //Equipping Right hand
                    if (spell.equipSlot == EquipmentSlot.Right_Hand)
                    {
                        handFX = Instantiate<GameObject>(spell.handFX_Primary);

                        if (currentEquipment[1] != null)
                        {
                            if (currentEquipment[1] is Weapon twoHands && twoHands.bothHands)
                            {
                                Unequip(1);
                            }
                        }
                    }
                    //Equipping Left hand
                    else
                    {
                        handFX = Instantiate<GameObject>(spell.handFX_Secondary);

                        if (currentEquipment[0] != null)
                        {
                            if (currentEquipment[0] is Weapon twoHands && twoHands.bothHands)
                            {
                                Unequip(0);
                            }
                            else if (currentEquipment[0] is Spell doubleSpell && doubleSpell.bothHands)
                            {
                                Unequip(0);
                            }
                        }
                    }
                    //Parents new go to player
                    handFX.transform.parent = PlayerManager.instance.player.transform;
                    //insert into go equipObj aray
                    currentWeapons[slotIndex] = handFX;
                    lastEquippedWeapon = newItem.equipSlot;

                    //Disable spellMarker just as a precaution
                    playerCombat.showSpellMarker = false;
                    break;
                }
        }

        //Insert the item into the corresponding equipment slot
        currentEquipment[slotIndex] = newItem;
    }

    //Unequip an item with a particular index
    public Equipment Unequip(int slotIndex)
    {
        //Only do this if an item is there
        if (currentEquipment[slotIndex] != null)
        {
            //The item that is currently equipped in that slot
            Equipment oldItem = currentEquipment[slotIndex];

            //Determine what type of item is being unequipped
            switch (oldItem)
            {
                case Weapon oldWeapon:
                    {
                        //Remove the go from the gameworld
                        if (currentWeapons[slotIndex] != null)
                        {
                            Destroy(currentWeapons[slotIndex].gameObject);
                        }
                        if (oldWeapon.isTorch)
                        {
                            playerCombat.animator.SetLayerWeight(2, 0);
                        }


                        //if unequiping a shield
                        if (oldWeapon.weaponType == WeaponType.Shield)
                        {
                            playerCombat.leftHand = HandState.Empty; //Not sure if these are redundant yet
                        }
                        //if unequiping a bow
                        else if (oldWeapon.weaponType == WeaponType.Bow)
                        {
                            playerCombat.hasBow = false;

                            playerCombat.rightHand = HandState.Empty;
                            playerCombat.leftHand = HandState.Empty;

                            playerCombat.animator.SetInteger("weaponType_1", 0);
                            playerCombat.animator.SetInteger("weaponType_2", 0);
                            playerCombat.animator.SetFloat("weaponSpeed_1", 1);
                            playerCombat.animator.SetFloat("weaponSpeed_2", 1);
                        }
                        else
                        {
                            if (oldWeapon.equipSlot == EquipmentSlot.Right_Hand)
                            {
                                playerCombat.rightHand = HandState.Empty;
                                playerCombat.animator.SetInteger("weaponType_1", 0);
                                playerCombat.animator.SetFloat("weaponSpeed_1", 1);
                            }
                            else
                            {
                                playerCombat.leftHand = HandState.Empty;
                                playerCombat.animator.SetInteger("weaponType_2", 0);
                                playerCombat.animator.SetFloat("weaponSpeed_2", 1);
                            }

                            //Unequipping a two-handed weapon, left hand no longer occupied
                            if (oldWeapon.bothHands)
                            {
                                playerCombat.leftHand = HandState.Empty;
                            }
                        }

                        playerCombat.animator.Play("Empty", 1);
                        break;
                    }
                case Arrows oldArrow:
                    {
                        //Remove the go from the gameworld
                        if (currentWeapons[2] != null)
                        {
                            Destroy(currentWeapons[2].gameObject);
                        }

                        if (currentEquipment[1] != null && currentEquipment[1] is Weapon weapon)
                        {
                            if (weapon.weaponType == WeaponType.Bow)
                            {
                                ActiveWeapon bow = playerCombat.GetComponentInChildren<ActiveWeapon>();
                                if (bow != null && bow.weapon.weaponType == WeaponType.Bow)
                                {
                                    bow.arrow = null;
                                }
                                else
                                {
                                    Debug.Log("IDK something wrong");
                                }
                            }
                        }

                        HUDManager.instance.OnAmmunitionChange(null);
                        break;
                    }
                case Apparel oldApparel:
                    {
                        switch (oldApparel)
                        {
                            case Apparel_Head:
                                {
                                    foreach (SkinnedMeshRenderer oldHeadMesh in equippedHeadMeshes)
                                    {
                                        Destroy(oldHeadMesh.gameObject);
                                    }

                                    equippedHeadMeshes.Clear();

                                    baseHead.enabled = true;
                                    baseEyebrows.enabled = true;

                                    if (baseHair != null)
                                    {
                                        baseHair.enabled = true;
                                    }
                                    if (baseFacialhair != null)
                                    {
                                        baseFacialhair.enabled = true;
                                    }
                                    break;
                                }
                            case Apparel_Chest oldChest:
                                {
                                    foreach (SkinnedMeshRenderer oldChestMesh in equippedChestMeshes)
                                    {
                                        Destroy(oldChestMesh.gameObject);
                                    }

                                    equippedChestMeshes.Clear();

                                    baseChest.enabled = true;

                                    if (baseLegMesh != null)
                                    {
                                        baseLegs.sharedMesh = baseLegMesh;
                                    }

                                    foreach (SkinnedMeshRenderer UAMesh in baseUpperArms)
                                    {
                                        UAMesh.enabled = true;
                                    }
                                    foreach (SkinnedMeshRenderer LAMesh in baseLowerArms)
                                    {
                                        LAMesh.enabled = true;
                                    }

                                    longSleeves = false;
                                    if (oldChest.longSleeves == true)
                                    {
                                        Apparel_Hands currentHands = currentEquipment[4] as Apparel_Hands;
                                        if (currentHands != null && currentHands.hasForeArms == true)
                                        {
                                            //adjustHands = true;
                                            Equip(currentHands);
                                        }
                                    }

                                    break;
                                }
                            case Apparel_Hands:
                                {
                                    foreach (SkinnedMeshRenderer oldHandsMesh in equippedHandMeshes)
                                    {
                                        Destroy(oldHandsMesh.gameObject);
                                    }

                                    equippedHandMeshes.Clear();

                                    foreach (SkinnedMeshRenderer handMesh in baseHands)
                                    {
                                        handMesh.enabled = true;
                                    }
                                    foreach (SkinnedMeshRenderer LAMesh in baseLowerArms)
                                    {
                                        LAMesh.enabled = true;
                                    }

                                    break;
                                }
                            case Apparel_Feet:
                                {
                                    foreach (SkinnedMeshRenderer oldFeetMesh in equippedFeetMeshes)
                                    {
                                        Destroy(oldFeetMesh.gameObject);
                                    }

                                    equippedFeetMeshes.Clear();

                                    foreach(SkinnedMeshRenderer feetMesh in baseFeet)
                                    {
                                        feetMesh.enabled = true;
                                    }

                                    break;
                                }
                            case Apparel_Accessories oldAccessory:
                                {
                                    switch (oldAccessory.accessoryType)
                                    {
                                        case AccessoryType.Pauldrons:
                                            {
                                                foreach (SkinnedMeshRenderer oldShoulderMesh in equippedShoulderMeshes)
                                                {
                                                    Destroy(oldShoulderMesh.gameObject);
                                                }

                                                equippedShoulderMeshes.Clear();

                                                break;
                                            }
                                        case AccessoryType.Cape:
                                            {
                                                Destroy(equippedCapeMesh);
                                                equippedCapeMesh = null;

                                                break;
                                            }
                                        case AccessoryType.Elbowpads:
                                            {
                                                foreach (SkinnedMeshRenderer oldElbowMesh in equippedElbowMeshes)
                                                {
                                                    Destroy(oldElbowMesh.gameObject);
                                                }

                                                equippedElbowMeshes.Clear();

                                                break;
                                            }
                                        case AccessoryType.Kneepads:
                                            {
                                                foreach (SkinnedMeshRenderer oldKneeMesh in equippedKneeMeshes)
                                                {
                                                    Destroy(oldKneeMesh.gameObject);
                                                }

                                                equippedKneeMeshes.Clear();

                                                break;
                                            }
                                    }

                                    break;
                                }
                        }
                        apparelWeight -= oldApparel.weight;
                        apparelWeight = Mathf.Clamp(apparelWeight, 1, 60);
                        break;
                    }
                case Spell oldSpell:
                    {
                        //Remove the go from the gameworld
                        if (currentWeapons[slotIndex] != null)
                        {
                            Destroy(currentWeapons[slotIndex].gameObject);
                        }
                        playerCombat.animator.Play("Empty", 1);

                        //Unequipping the right hand
                        if (slotIndex == 0)
                        {
                            playerCombat.rightHand = HandState.Empty;
                            playerCombat.animator.SetInteger("weaponType_1", 0);
                            playerCombat.animator.SetFloat("weaponSpeed_1", 1);
                        }
                        //Unequipping the left hand
                        else if (slotIndex == 1)
                        {
                            playerCombat.leftHand = HandState.Empty;
                            playerCombat.animator.SetInteger("weaponType_2", 0);
                            playerCombat.animator.SetFloat("weaponSpeed_2", 1);
                        }

                        if (oldSpell.bothHands)
                        {
                            playerCombat.leftHand = HandState.Empty;
                            if (currentWeapons[1] != null)
                            {
                                Destroy(currentWeapons[1].gameObject);
                            }
                        }

                        //Disable player spell marker
                        playerCombat.showSpellMarker = false;
                        break;
                    }
            }

            //Remove the item from the equipment array
            currentEquipment[slotIndex] = null;

            //Update menu display
            oldItem.isEquipped = false;
            UiManager.instance.UpdateMenu();

            //Equipment has been removed so we trigger the callback
            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }

            return oldItem;
        }
        return null;
    }
    //Unequip all items
    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }
}

//Need to later add if/else for if male/female

//at end of character creation, add all active skinnedMeshRenderers to appropriate skinnedmeshrenderer arrays
//e.g. baseHead, baseChest, baseLegs