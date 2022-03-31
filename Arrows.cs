using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arrow", menuName = "Inventory/Weapon/Arrow")]
public class Arrows : Equipment
{
    void Reset()
    {
        itemCategory = ItemCategory.Weapons;
        equipSlotPrimary = EquipmentSlot.Arrows;
        equipSlotSecondary = EquipmentSlot.Arrows;
        isStackable = true;
    }
    public DamageType damageType;
    public GameObject quiver;
    public GameObject arrowPrefab;
}
