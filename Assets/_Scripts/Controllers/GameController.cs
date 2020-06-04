using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using JetBrains.Annotations;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    [Header("Resources")]
    public int money;
    public int manPower, favour;
    public int moneyPerSecond, manPowerPerSecond;
    public bool freezeAssets = false;
    public int defensePerMan, attackPerMan;

    [Header("WarConnections")]
    public float maxPlayerTowerHealth;
    public int battlePointsPerSecond;
    public GameObject[] playerUnits;
    public UnitStats[] unitStats;
    public UnitStats[] unitStatsOriginal;
    public int nextLevel = 1;

    private float timeCounter;
    private bool isStrategyOpen;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        isStrategyOpen = false;
        unitStats = new UnitStats[4];
        unitStatsOriginal = new UnitStats[4];
        for(int i = 0; i < 4; i++)
        {
            unitStats[i] = playerUnits[i].GetComponent<Unit>().GetStats();
            unitStatsOriginal[i] = playerUnits[i].GetComponent<Unit>().GetStats();
        }
    }

    void Update()
    {
        if(!freezeAssets)timeCounter += Time.deltaTime;
        if(timeCounter >= 1)
        {
            manPower += manPowerPerSecond;
            money += moneyPerSecond;
            timeCounter = timeCounter - 1;
        }

        if(Input.GetKeyDown(KeyCode.Delete) && Application.isEditor)
        {
            GoWar();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && Application.isEditor) nextLevel++;
        if (Input.GetKeyDown(KeyCode.DownArrow) && Application.isEditor) nextLevel--;
    }

    public void OpenStrategy()
    {
        isStrategyOpen = true;
    }

    public void GoWar()
    {
        freezeAssets = true;
        SceneManager.LoadScene(2);
    }

    public void Return(int money, int mp, int favour, bool won)
    {
        if (won) nextLevel++;
        this.money += money;
        this.manPower += mp;
        this.favour += favour;
        Debug.Log(nextLevel);
        SceneManager.LoadScene(1);
        freezeAssets = false;
    }

    public void CloseStrategy()
    {
        isStrategyOpen = false;
    }
}