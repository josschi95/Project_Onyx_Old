using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;

    public override void Interact()
    {
        base.Interact();

        PickUp();
    }

    protected virtual void PickUp()
    {
        Inventory.instance.AddItem(item);
        Destroy(gameObject);
    }
}
