using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : CharacterCombat
{
    GameMaster gameMaster;
    EnemyController enemyController;

    [HideInInspector] public bool hasToken;
    [HideInInspector] public float weaponReach = 1.6f; //Will determine stopping distance for navmeshagent

    public GameObject sideArm;
    public GameObject arrows;

    float nextAttackTime;
    [Range(0.2f, 3f)] public float attackRate = 1f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        gameMaster = GameMaster.instance;
        enemyController = GetComponent<EnemyController>();

        if (hasBow)
        {
            weaponReach = 10f;
        }
        //else if(hasPole){weaponReach = 2f;}
        else
        {
            weaponReach = 1.6f;
        }
    }

    public void Attack_Primary()
    {
        if (Time.time >= nextAttackTime)
        {
            if (hasBow)
            {
                if (bowDrawn)
                    return;
                StartCoroutine(FireBow());
                nextAttackTime = Time.time + 1f / attackRate;
            }
            else
            {
                animator.SetTrigger("Attack1");
                //Add a little forward movement
                enemyController.rb.AddForce(transform.forward * 20f);
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    public void SwitchToMelee()
    {
        if (!hasBow)
            return;

        Container loot = GetComponent<Container>();
        if (loot != null)
        {
            ItemPickup bow = GetComponentInChildren<ItemPickup>();
            if (bow != null)
            {
                loot.contents.Add(bow.item);
                Destroy(bow.gameObject);
            }
        }
        hasBow = false;
        attackRate = 2f;
        sideArm.SetActive(true);
        weaponReach = 1.6f;
        nextAttackTime = 0;
    }

    IEnumerator FireBow()
    {
        bowDrawn = true;
        animator.SetBool("bowDrawn", bowDrawn);
        yield return new WaitForSeconds(fireDelay);

        bowDrawn = false;
        animator.SetBool("bowDrawn", bowDrawn);
    }
}