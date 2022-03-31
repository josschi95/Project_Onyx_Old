using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    //Singleton
    public static GameMaster instance;

    Inventory inventory;
    PlayerManager playerManager;
    public int difficultyLevel;
    public int maxCombatTokens;
    public int currentCombatTokens;
    public bool gamePaused = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GameMaster found");
            return;
        }
        instance = this;

        inventory = Inventory.instance;
        playerManager = PlayerManager.instance;

        if (difficultyLevel == 0) //Story difficulty
        {
            maxCombatTokens = 3;
        }
        else if (difficultyLevel == 1) //Normal difficulty
        {
            maxCombatTokens = 4;
        }
        else if (difficultyLevel == 2) //Hard difficulty
        {
            maxCombatTokens = 5;
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCombatTokens > maxCombatTokens)
        {
            currentCombatTokens = maxCombatTokens;
        }
    }

    public bool RetrieveToken()
    {
        if (currentCombatTokens >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ReturnToken()
    {
        currentCombatTokens++;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        gamePaused = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        gamePaused = false;
    }
}
