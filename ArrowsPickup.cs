using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsPickup : ItemPickup
{
    [SerializeField] int arrowQuantity;

    protected override void PickUp()
    {
        for (int i = 0; i < arrowQuantity; i++)
        {
            Inventory.instance.AddItem(item);
        }

        Destroy(gameObject);
    }
}
