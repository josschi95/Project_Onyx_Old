using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : ActiveSpell
{
    [Range(10f, 100f)]
    public float speed;
    public LayerMask obstacleLayers;
    new Collider collider; //don't know if this is necessary
    Rigidbody rb;

    protected override void Start()
    {
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        if (parentRb == null)
        {
            parentRb = rb;
        }
        StartCoroutine(DestroySelf());
    }

    void OnTriggerEnter(Collider other)
    {
        //this weapon strikes a character
        CharacterStats target = other.GetComponentInParent<CharacterStats>();
        if (target != null && ((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            DealDamage(other, target);
            Destroy(gameObject);
        }
        else if ((1 << other.gameObject.layer & obstacleLayers) != 0)
        {
            //Play spell hitting stone/ground/wood sound FX
            Destroy(gameObject);
        }
    }

    protected virtual void DealDamage(Collider other, CharacterStats target)
    {
        int totalDamage = characterStats.power.GetValue() + spell.damage;

        CinemachineShake.instance.ShakeCamera(3f, 0.1f); //Add screen shake
        Vector3 impactPosition = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        if (target.GetComponent<CharacterCombat>().isBlocking)
        {
            other.GetComponentInParent<CharacterCombat>().animator.SetTrigger("shieldStruck");
            //play spell hitting shield sound effect
        }

        target.TakeDamage(totalDamage, spell.damageType, false, impactPosition);

        if (spell.knockbackForce > 0)
        {
            target.Knockback(parentRb.transform.position, spell.knockbackForce);
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(20f);
        Destroy(gameObject);
    }
}
