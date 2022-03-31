using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Weapon weapon;
    [HideInInspector] public Rigidbody parentRb;
    [HideInInspector] public CharacterCombat characterCombat;
    [HideInInspector] public CharacterStats characterStats;
    [HideInInspector] public LayerMask targetLayers;

    public ParticleSystem fireFX;
    public ParticleSystem impactFX;
    Rigidbody rb;

    [Range(10f, 100f)]
    public float speed;
    public LayerMask obstacleLayers;
    public GameObject soundPing;
    public DamageType damageType;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        if (fireFX != null) fireFX.Play(true);
        if (parentRb == null) parentRb = rb;
        weapon.damageType = damageType;
        StartCoroutine(DestroySelf());
    }

    void OnTriggerEnter(Collider other)
    {
        ActiveWeapon weapon = other.GetComponent<ActiveWeapon>();
        if (weapon != null) return; //this projectile struck a weapon, not a creature

        //this weapon strikes a character
        CharacterStats target = other.GetComponentInParent<CharacterStats>();
        if (target != null && ((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            DealDamage(other, target);
            Destroy(gameObject);
        }
        else if ((1 << other.gameObject.layer & obstacleLayers) != 0)
        {
            //Play arrow hitting stone/ground/wood sound FX
            Instantiate(soundPing, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    protected virtual void DealDamage(Collider other, CharacterStats target)
    {
        if (impactFX != null) impactFX.Play(true);

        int totalDamage = characterStats.power.GetValue() + weapon.damage;

        #region - Critical Hit -
        bool critResult = CriticalHit();
        EnemyController enemy = other.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            if (enemy.playerDetected == false)
            {
                critResult = true;
            }
        }
        if (critResult)
        {
            totalDamage = totalDamage * characterStats.critMultiplier.GetValue();
        }
        #endregion

        CinemachineShake.instance.ShakeCamera(3f, 0.1f); //Add screen shake
        Vector3 impactPosition = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        if (target.GetComponent<CharacterCombat>().isBlocking)
        {
            other.GetComponentInParent<CharacterCombat>().animator.SetTrigger("shieldStruck");
            //play projectile hitting shield sound effect
        }

        target.TakeDamage(totalDamage, damageType, critResult, impactPosition);

        if (weapon.knockbackForce > 0) target.Knockback(parentRb.transform.position, weapon.knockbackForce);
    }

    //Determine if the strike was a critical hit
    protected virtual bool CriticalHit()
    {
        float critRoll = Random.Range(1f, 100f);
        if (critRoll <= (weapon.critChance + characterStats.critChance.GetValue()))
        {
            Debug.Log("Critical Hit scored");
            return true;
        }
        else return false;
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(20f);
        Destroy(gameObject);
    }
}