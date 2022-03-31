using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Base Item Properties")]
    new public string name = "New Item";
    public ItemCategory itemCategory;
    [Space]
    public Sprite icon = null;
    public float weight;
    public int value;
    public int itemQuantity = 1;
    public string flavorText = "";
    public bool isStackable = false;

    public virtual void Use()
    {
        Debug.Log("Using " + name);
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.RemoveItem(this);
    }

    public void DropItem()
    {
        RemoveFromInventory();
        //Would need base items for non-equipment items... not a big deal, just something to consider
    }
}

public enum ItemCategory { Apparel, Weapons, Magic, Potions, Misc }