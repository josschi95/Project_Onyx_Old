using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Inventory/Spell")]
public class Spell : Equipment
{
    void Reset()
    {
        equipSlotPrimary = EquipmentSlot.Right_Hand;
        equipSlotSecondary = EquipmentSlot.Left_Hand;
        itemCategory = ItemCategory.Magic;
    }

    public SpellType spellType;
    public DamageType damageType;
    public CastingType castingType;
    public SpellShape spellShape;
    [Space]
    public GameObject handFX_Primary;
    public GameObject handFX_Secondary;
    public GameObject spellPrefab;
    [Range(10, 200)] public int damage;
    [Range(1, 100)] public int manaCost;

    [Tooltip("Multiplier for animation speed")]
    [Range(0.1f, 2)] public float castingSpeed = 1;
    [Range(0f, 300f)] public float spellDuration;
    [Range(0, 500)] public float knockbackForce;
    [Range(0, 10)] public float spellRadius;
    [Range(0, 50)] public float spellRange;
    public bool bothHands = false; //Probalby only true for higher level spells or ritual-like spells
    [HideInInspector] public float critChance = 0f;

    public override void Use()
    {
        base.Use();
        EquipmentManager.instance.Equip(this);
    }

    public override void UseSecondary()
    {
        equipSlot = equipSlotSecondary;
        EquipmentManager.instance.Equip(this);
    }
}

public enum CastingType { Charged, Continuous }
public enum SpellShape { Projectile, A_o_E, Nova }
public enum SpellType { Offensive, Defensive }