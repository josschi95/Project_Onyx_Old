using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    EnemyController enemyController;
    Container container;
    public bool isStunned = false;
    float stunTime = 2f;
    public float fleeChance;
    [SerializeField] int expValue;
    

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        container = GetComponentInChildren<Container>();
        container.enabled = false;
    }

    public override void TakeDamage(int damage, DamageType damageType, bool critResult, Vector3 impactPosition)
    {
        base.TakeDamage(damage, damageType, critResult, impactPosition);

        enemyController.StopAllCoroutines();
        enemyController.DamageTaken();

        if (currentHealth <= maxHealth / 4)
        {
            var result = Random.Range(1, 100);
            if (result < fleeChance)
            {
                enemyController.RunAway();
            }
        }
    }

    public override void Die()
    {
        base.Die();

        Container loot = GetComponent<Container>();
        if (loot != null)
        {
            ItemPickup weapon = GetComponentInChildren<ItemPickup>();
            if (weapon != null)
            {
                loot.contents.Add(weapon.item);
                Destroy(weapon.gameObject);
            }
        }

        container.enabled = true;
        PlayerManager.instance.GainEXP(expValue);
    }

    IEnumerator Stunned()
    {
        isStunned = true;

        yield return new WaitForSeconds(stunTime);

        isStunned = false;
    }
}