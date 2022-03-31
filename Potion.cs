using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Potion")]
public class Potion : Item
{
    void Reset()
    {
        itemCategory = ItemCategory.Potions;
    }

    public PotionType potionType;
    public int healAmount;
    public int manaAmount;
    [Space]
    public Statbonus affectedStat;
    public int statBonus;
    public float effectDuration;

    PlayerStats playerStats;

    private void Awake()
    {
        playerStats = PlayerStats.instance;
    }
    public override void Use()
    {
        base.Use();

        //Have effect based on what type of potion is being used
        switch (potionType)
        {
            case PotionType.Health_Potion:
                {
                    playerStats.Heal(healAmount);
                    break;
                }
            case PotionType.Mana_Potion:
                {
                    playerStats.RestoreMana(manaAmount);
                    break;
                }
            case PotionType.Rejuvenation_Potion:
                {
                    playerStats.Heal(healAmount);
                    playerStats.RestoreMana(manaAmount);
                    break;
                }
            case PotionType.Stat_Bonus_Potion:
                {
                    switch (affectedStat) //Another switch/case running through all of the player stats
                    {
                        case Statbonus.Power:
                            {
                                playerStats.BoostStat(playerStats.power, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Armor:
                            {
                                playerStats.BoostStat(playerStats.armor, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Bludgeoning_Resistance:
                            {
                                playerStats.BoostStat(playerStats.bludgeoningResistance, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Piercing_Resistance:
                            {
                                playerStats.BoostStat(playerStats.piercingResistance, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Slashing_Resistance:
                            {
                                playerStats.BoostStat(playerStats.slashingResistance, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Fire_Resistance:
                            {
                                playerStats.BoostStat(playerStats.fireResistance, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Holy_Resistance:
                            {
                                playerStats.BoostStat(playerStats.holyResistance, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Ice_Resistance:
                            {
                                playerStats.BoostStat(playerStats.iceResistance, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Lightning_Resistance:
                            {
                                playerStats.BoostStat(playerStats.lightningResistance, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Poison_Resistance:
                            {
                                playerStats.BoostStat(playerStats.poisonResistance, statBonus, effectDuration);
                                break;
                            }
                        case Statbonus.Carry_Capacity:
                            {
                                playerStats.BoostStat(playerStats.carryCapacity, statBonus, effectDuration);
                                break;
                            }
                    }
                    break;
                }
        }

        RemoveFromInventory();
    }
}

public enum PotionType { Health_Potion, Mana_Potion, Rejuvenation_Potion, Stat_Bonus_Potion }
public enum Statbonus { Power, Armor, Bludgeoning_Resistance, Piercing_Resistance, Slashing_Resistance, Fire_Resistance, Holy_Resistance, Ice_Resistance, Lightning_Resistance, Poison_Resistance, Carry_Capacity }