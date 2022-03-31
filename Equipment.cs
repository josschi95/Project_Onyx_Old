using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    [Header("Equipment Properties")]
    [HideInInspector] public bool isEquipped = false;
    [HideInInspector] public EquipmentSlot equipSlot;
    public EquipmentSlot equipSlotPrimary;
    public EquipmentSlot equipSlotSecondary;

    [Header("Item Stats")]
    public int armorModifier;
    public int powerModifier;

    public override void Use()
    {
        equipSlot = equipSlotPrimary;
        EquipmentManager.instance.Equip(this);
    }

    public virtual void UseSecondary()
    {
        equipSlot = equipSlotSecondary;
        EquipmentManager.instance.Equip(this);
    }
}

public enum EquipmentSlot { Right_Hand, Left_Hand, Head, Body, Hands, Feet,  Shoulders, Cape, Elbows, Knees, Arrows }