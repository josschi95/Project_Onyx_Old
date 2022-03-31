using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    #region - Singleton -

    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found");
            return;
        }

        instance = this;
    }

    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public float carryWeight;
    public int gold;

    [Space]
    public List<Item> apparel = new List<Item>();
    public List<Item> weapons = new List<Item>();
    public List<Item> spells = new List<Item>();
    public List<Item> potions = new List<Item>();
    public List<Item> miscellaneous = new List<Item>();

    public Inventory()
    {
        ClearInventory();
    }

    public void ClearInventory()
    {
        apparel.Clear();
        weapons.Clear();
        potions.Clear();
        miscellaneous.Clear();
    }

    //Add item to appropriate list
    public void AddItem(Item item)
    {
        switch (item.itemCategory)
        {
            case ItemCategory.Apparel:
                {
                    if (item.isStackable)
                    {
                        bool itemAlreadyInApparelInventory = false;
                        foreach (Item inventoryItem in apparel)
                        {
                            if (inventoryItem.name == item.name)
                            {
                                inventoryItem.itemQuantity += item.itemQuantity;
                                itemAlreadyInApparelInventory = true;
                            }
                        }
                        if (!itemAlreadyInApparelInventory)
                        {
                            var instance = ScriptableObject.Instantiate(item);
                            apparel.Add(instance);
                        }
                    }
                    else
                    {
                        var instance = ScriptableObject.Instantiate(item);
                        apparel.Add(instance);
                    }
                    apparel = apparel.OrderBy(go => go.name).ToList();
                    break;
                }
            case ItemCategory.Weapons:
                {
                    if (item.isStackable)
                    {
                        bool itemAlreadyInWeaponsInventory = false;
                        foreach (Item inventoryItem in weapons)
                        {
                            if (inventoryItem.name == item.name)
                            {
                                inventoryItem.itemQuantity += item.itemQuantity;
                                itemAlreadyInWeaponsInventory = true;
                            }
                        }
                        if (!itemAlreadyInWeaponsInventory)
                        {
                            var instance = ScriptableObject.Instantiate(item);
                            weapons.Add(instance);
                        }
                    }
                    else
                    {
                        var instance = ScriptableObject.Instantiate(item);
                        weapons.Add(instance);
                    }
                    weapons = weapons.OrderBy(go => go.name).ToList();
                    break;
                }
            case ItemCategory.Magic:
                {
                    if (QuerySpellKnown(item))
                    {
                        Debug.Log("This spell is already known.");
                    }
                    else
                    {
                        Debug.Log("You learned the " + item.name + " spell");
                        var instance = ScriptableObject.Instantiate(item);
                        spells.Add(instance);
                    }
                    spells = spells.OrderBy(go => go.name).ToList();
                    break;
                }
            case ItemCategory.Potions:
                {
                    if (item.isStackable)
                    {
                        bool itemAlreadyInPotionsInventory = false;
                        foreach (Item inventoryItem in potions)
                        {
                            if (inventoryItem.name == item.name)
                            {
                                inventoryItem.itemQuantity += item.itemQuantity;
                                itemAlreadyInPotionsInventory = true;
                            }
                        }
                        if (!itemAlreadyInPotionsInventory)
                        {
                            var instance = ScriptableObject.Instantiate(item);
                            potions.Add(instance);
                        }
                    }
                    else
                    {
                        var instance = ScriptableObject.Instantiate(item);
                        potions.Add(instance);
                    }
                    potions = potions.OrderBy(go => go.name).ToList();
                    break;
                }
            case ItemCategory.Misc:
                {
                    if (item.isStackable)
                    {
                        bool itemAlreadyInMiscInventory = false;
                        foreach (Item inventoryItem in miscellaneous)
                        {
                            if (inventoryItem.name == item.name)
                            {
                                inventoryItem.itemQuantity += item.itemQuantity;
                                itemAlreadyInMiscInventory = true;
                            }
                        }
                        if (!itemAlreadyInMiscInventory)
                        {
                            var instance = ScriptableObject.Instantiate(item);
                            miscellaneous.Add(instance);
                            //miscellaneous.Add(item);
                        }
                    }
                    else
                    {
                        var instance = ScriptableObject.Instantiate(item);
                        miscellaneous.Add(instance);
                        //miscellaneous.Add(item);
                    }

                    miscellaneous = miscellaneous.OrderBy(go => go.name).ToList();
                    break;
                }
        }
        carryWeight += item.weight * item.itemQuantity;

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    //Remove item from appropriate list
    public void RemoveItem(Item item)
    {
        switch (item.itemCategory)
        {
            case ItemCategory.Apparel:
                {
                    if (item.isStackable)
                    {
                        if (item.itemQuantity > 1)
                        {
                            item.itemQuantity--;
                            break;
                        }
                        else
                        {
                            apparel.Remove(item);
                            break;
                        }
                    }
                    else
                    {
                    apparel.Remove(item);
                    }
                    break;
                }
            case ItemCategory.Weapons:
                {
                    if (item.isStackable)
                    {
                        if (item.itemQuantity > 1)
                        {
                            item.itemQuantity--;
                            break;
                        }
                        else
                        {
                            weapons.Remove(item);

                            if (item is Arrows arrow)
                            {
                                EquipmentManager.instance.Unequip(10);
                            }
                            break;
                        }
                    }
                    else
                    {
                        weapons.Remove(item);
                    }
                    break;
                }
            case ItemCategory.Potions:
                {
                    if (item.isStackable)
                    {
                        if (item.itemQuantity > 1)
                        {
                            item.itemQuantity--;
                            break;
                        }
                        else
                        {
                            potions.Remove(item);
                            break;
                        }
                    }
                    else
                    {
                        potions.Remove(item);
                    }
                    break;
                }
            case ItemCategory.Misc:
                {
                    if (item.isStackable)
                    {
                        if (item.itemQuantity > 1)
                        {
                            item.itemQuantity--;
                            break;
                        }
                        else
                        {
                            miscellaneous.Remove(item);
                            break;
                        }
                    }
                    else
                    {
                        miscellaneous.Remove(item);
                        break;
                    }
                }
        }
        carryWeight -= item.weight;
        //May need to make changes here in the future if removing multiple items at the same time, or find a way to just run this x number of times

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public bool HasProjectiles(Item projectile)
    {
        foreach(Item item in weapons)
        {
            if (item.name == projectile.name)
            {
                return true;
            }
        }
        return false;
    }

    public bool QuerySpellKnown(Item spell)
    {
        foreach (Item item in spells)
        {
            if (item.name == spell.name)
            {
                return true;
            }
        }
        return false;
    }
}