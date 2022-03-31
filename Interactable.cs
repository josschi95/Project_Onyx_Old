using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool interacting = false;
    public InteractionMethod interactionMethod;

    public virtual void Interact()
    {
        interacting = true;
    }
}

public enum InteractionMethod { Take, Open, Unlock, Loot, Talk, Read, Interact}
