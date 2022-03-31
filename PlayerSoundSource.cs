using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundSource : SoundSource
{
    Rigidbody rb;
    EquipmentManager equipmentManager;

    public override void Start()
    {
        base.Start();

        rb = GetComponentInParent<Rigidbody>();
        equipmentManager = EquipmentManager.instance;
    }

    void Update()
    {
        soundCollider.radius = (rb.velocity.magnitude) * soundMultiplier * equipmentManager.apparelWeight;
    }
}

//rb.velocity.magnitude hits about 3 when crouching, 4 at walking, 5 while running, and 9-10 while sprinting
//apparelWeight will range from 1(naked) up to ~5 for plain cloth, 10 with light armor, 20-35 with medium armor, and maybe 35-50 with heavy armor?
//Later add in a sneak multiplier which is 1 by default, and gets smaller and smaller as the player's stealth score increases
