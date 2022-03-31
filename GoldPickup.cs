using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : Interactable
{
    public int goldQuantity;

    public override void Interact()
    {
        base.Interact();

        AddGold();
    }

    void AddGold()
    {
        Debug.Log("Claimed " + goldQuantity + " gold");
        Inventory.instance.gold += goldQuantity;
        Destroy(gameObject);
    }
}
