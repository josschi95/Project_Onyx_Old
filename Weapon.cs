using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon/Weapon")]
public class Weapon : Equipment
{
    void Reset()
    {
        equipSlotPrimary = EquipmentSlot.Right_Hand;
        equipSlotSecondary = EquipmentSlot.Left_Hand;
        itemCategory = ItemCategory.Weapons;
    }

    [Header("Weapon Properties")]
    public GameObject activePrefab;
    public Vector3 secondaryPosition;
    public Vector3 secondaryRotation;
    [Space]
    public WeaponType weaponType;
    public DamageType damageType;
    [Space]
    [Range(5, 100)]
    public int damage;
    [Range(0.5f, 2)]
    public float weaponSpeed = 1;
    [Range(1, 10)]
    public float critChance = 1f;
    [Range(50, 500)]
    public float knockbackForce;
    public bool bothHands = false; //True for Heavy, Poles, and Bows
    public bool isTorch = false;

    public override void Use()
    {
        CheckIfEquipped();
        base.Use();
    }

    public override void UseSecondary()
    {
        CheckIfEquipped();
        base.UseSecondary();
    }

    //If the item is already equipped, unequips it
    void CheckIfEquipped()
    {
        if (this.isEquipped)
        {
            if (EquipmentManager.instance.currentEquipment[0] != null && EquipmentManager.instance.currentEquipment[0].name == this.name)
            {
                EquipmentManager.instance.Unequip(0);
            }
            else if (EquipmentManager.instance.currentEquipment[1] != null && EquipmentManager.instance.currentEquipment[1].name == this.name)
            {
                EquipmentManager.instance.Unequip(1);
            }
        }
    }
}

public enum DamageType { Bludgeoning, Piercing, Slashing, Fire, Holy, Ice, Lightning, Poison, Null }

public enum WeaponType { Shield, One_Handed, Heavy, Pole, Bow, Throwing }
//maybe later add Unarmed, Staff