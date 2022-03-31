using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellNova : ActiveSpell
{
    new Collider collider;
    Rigidbody rb;

    float nextTimeCall;
    List<CharacterStats> damagedEnemies = new List<CharacterStats>();

    protected override void Start()
    {
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        if (parentRb == null)
        {
            parentRb = rb;
        }
        nextTimeCall = Time.time + 1f;

        StartCoroutine(SpellDuration());
    }

    private void Update()
    {
        if (Time.time >= nextTimeCall)
        {
            ClearList();
            nextTimeCall += 1f;
        }
    }
    void OnTriggerStay(Collider other)
    {
        //this weapon strikes a character
        CharacterStats target = other.GetComponentInParent<CharacterStats>();
        if (target != null && ((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            if (!damagedEnemies.Contains(target))
            {
                DealDamage(other, target);
                damagedEnemies.Add(target);
            }
        }
    }

    protected virtual void DealDamage(Collider other, CharacterStats target)
    {
        int totalDamage = characterStats.power.GetValue() + spell.damage;

        Vector3 impactPosition = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        target.TakeDamage(totalDamage, spell.damageType, false, impactPosition);
    }

    void ClearList()
    {
        damagedEnemies.Clear();
    }

    IEnumerator SpellDuration()
    {
        yield return new WaitForSeconds(spell.spellDuration);
        Destroy(gameObject);
    }
}
