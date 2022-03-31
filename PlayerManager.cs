using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    #region - Singleton -

    public static PlayerManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject player;
    public GameObject playerCenter;
    [SerializeField] float deathDelay = 7f;

    public int totalExp { get; private set; }

    public void KillPlayer()
    {
        PlayerCombat.instance.animator.Play("Die", 0);
        StartCoroutine(ResetLevel());
    }

    IEnumerator ResetLevel()
    {
        yield return new WaitForSeconds(deathDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GainEXP(int xpValue)
    {
        totalExp += xpValue;
        Debug.Log("Player gained +" + xpValue + " exp!");
    }
}
