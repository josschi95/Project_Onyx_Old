using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [HideInInspector] public Animator animator;

    [HideInInspector]
    public bool isBlocking = false; //Use this to activate shield collider
    [HideInInspector] public bool castingSpell_Primary = false;
    [HideInInspector] public bool castingSpell_Secondary = false;

    [Header("Bows")]
    public bool hasBow = false;
    public float fireDelay = 1f;
    [HideInInspector] public bool bowDrawn = false;
    [HideInInspector] public List<CharacterStats> targetsToIgnore = new List<CharacterStats>();

    [Header("Testing")]
    public HandState rightHand = HandState.Empty;
    public HandState leftHand = HandState.Empty;

    public virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void ClearTargetList()
    {
        targetsToIgnore.Clear();
    }
}

public enum HandState { Empty, Weapon, Shield, Magic, Occupied }