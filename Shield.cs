using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Weapon/Shield")]
public class Shield : Weapon
{
    void Reset()
    {
        equipSlotPrimary = EquipmentSlot.Left_Hand;
        equipSlotSecondary = EquipmentSlot.Left_Hand;
        itemCategory = ItemCategory.Weapons;
        weaponType = WeaponType.Shield;
    }

    [Range(10, 50)]
    public float blockPercent; //How much damage is decreased if the character is blocking
}