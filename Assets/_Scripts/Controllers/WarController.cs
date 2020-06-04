using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarController : MonoBehaviour
{
    [Header("Things")]
    public float maxPlayerTowerHealth, maxEnemyTowerHealth;
    public int battlepoints, battlePointsPerSecond;

    [Range(1, 5)]
    public int level;

    [Header("UI")]
    public Slider playerHealthSlider, enemyHealthSlider;
    public TextMeshProUGUI bp, bpRate;
    public GameObject toolTipBox;
    public TextMeshProUGUI toolTipDesc, toolTipCost;
    [TextArea(1,3)]
    public string[] tooltips;
    public GameObject outcomeCanvas;
    public TypeSlowly outcomeText;
    public TypeSlowly netText;
    private bool outcomeCalled = false;
    private bool outcome;
    private int moneyNet, manPowerNet, favourNet;

    [Header("Spawning")]
    public GameObject[] playerUnitTypes;
    public UnitStats[] unitStats;
    public Transform playerSpawn, enemySpawn;
    private int nextId = 0;

    [Header("Game")]
    public bool hasWon = false;
    public bool hasLost = false;
    private WarOrder playerOrder, enemyOrder;
    private float playerTowerHealth, enemyTowerHealth;
    private float timeSinceLastBP = 0;
    private List<Unit> playerUnits;

    [Header("\'AI\'")]
    public float enemyBP;
    public AIBehaviour[] behaviourLevels;
    private AIBehaviour currentBehaviour;
    private List<Unit> enemyUnits;
    private bool hasSpawnOpportunity;
    private float timeSinceLastSpawnOpportunity;

    private GameController gameController;

    public void EnterHover(int index)
    {
        toolTipBox.SetActive(true);
        toolTipDesc.text = tooltips[index];
        toolTipCost.text = index < playerUnitTypes.Length? playerUnitTypes[index].GetComponent<Unit>().broCost.ToString() + "BP" : "TERI RANGE SE BAHAR";
    }

    public void ExitHover(int index)
    {
        toolTipBox.SetActive(false);
    }

    public void Spawn(int unit)
    {
        if (unit > playerUnitTypes.Length - 1) return;
        var c = playerUnitTypes[unit].GetComponent<Unit>().broCost;
        if (battlepoints >= c)
        {
            battlepoints -= c;
            Unit u = Instantiate(playerUnitTypes[unit], playerSpawn.position, Quaternion.identity).GetComponent<Unit>();
            u.id = nextId++;
            u.ApplyStats(unitStats[unit]);
            Debug.Log(unitStats[unit].ToString());
            playerUnits.Add(u);
        }
    }

    public void SpawnEnemy(int unit)
    {
        if (unit > currentBehaviour.spawns.Length - 1) return;
        var c = currentBehaviour.spawns[unit].prefab.GetComponent<Unit>().broCost;
        if (enemyBP >= c)
        {
            enemyBP -= c;
            Unit u = Instantiate(currentBehaviour.spawns[unit].prefab, enemySpawn.position, Quaternion.identity).GetComponent<Unit>();
            var s = u.gameObject.transform.localScale;
            s.x *= -1;
            u.gameObject.transform.localScale = s;
            u.id = nextId++;
            enemyUnits.Add(u);
            currentBehaviour.lastSpawned = unit;
            currentBehaviour.spawnsThisTurn++;
        }
    }

    public void HandleDeath(Unit u)
    {
        
    }

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        level = gameController.nextLevel;
        unitStats = gameController.unitStats;
        battlePointsPerSecond = gameController.battlePointsPerSecond;
        maxPlayerTowerHealth = gameController.maxPlayerTowerHealth;
        playerTowerHealth = maxPlayerTowerHealth;
        playerUnits = new List<Unit>();
        enemyUnits = new List<Unit>();
        currentBehaviour = behaviourLevels[level-1];
        for(int i = 0; i < currentBehaviour.spawns.Length; i++)
        {
            currentBehaviour.spawns[i].timeSinceLastSpawn = 0;
        }
        currentBehaviour.queuedSpawns = new List<int>();
        maxEnemyTowerHealth = currentBehaviour.towerMax;
        enemyTowerHealth = maxEnemyTowerHealth;
        Debug.Log(playerTowerHealth);
    }

    private void Lose()
    {
        if (outcomeCalled) return;
        outcomeCanvas.SetActive(true);
        outcomeCalled = true;
        outcomeText.SetTargetText("You Lost...");
        outcomeText.lettersPerSecond = 2;
        moneyNet = currentBehaviour.loseRewards.moneyReward;
        manPowerNet = currentBehaviour.loseRewards.mpReward;
        favourNet = currentBehaviour.loseRewards.favourReward;
        netText.SetTargetText("Through your violent pursuits, you netted:\n\t" + moneyNet.ToString() + "$\n\t" + manPowerNet.ToString() + "MP\n\t" + favourNet.ToString() + "Favour");
        outcome = false;
    }

    private void Win()
    {
        if (outcomeCalled) return;
        outcomeCanvas.SetActive(true);
        outcomeCalled = true;
        outcomeText.lettersPerSecond = 8;
        outcomeText.SetTargetText("You Won!");
        moneyNet = currentBehaviour.winRewards.moneyReward;
        manPowerNet = currentBehaviour.winRewards.mpReward;
        favourNet = currentBehaviour.winRewards.favourReward;
        netText.SetTargetText("Through your violent pursuits, you netted:\n\t" + moneyNet.ToString() + "$\n\t" + manPowerNet.ToString() + "MP\n\t" + favourNet.ToString() + "Favour");
        outcome = true;
    }

    public void WannaGoBack()
    {
        gameController.Return(moneyNet, manPowerNet, favourNet, outcome);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash) && Application.isEditor) battlepoints += 1000;
        if (Input.GetKeyDown(KeyCode.O) && Application.isEditor) hasLost = true;
        if (Input.GetKeyDown(KeyCode.P) && Application.isEditor) hasWon = true;

        timeSinceLastBP += Time.deltaTime;
        if (timeSinceLastBP >= 1)
        {
            timeSinceLastBP -= 1;
            battlepoints += battlePointsPerSecond;
            enemyBP += currentBehaviour.bpPerSecond;
        }
        UpdateUI();
        if (hasLost) Lose();
        if (hasWon) Win();
        if (hasLost || hasWon) return;
        UpdateAI();
    }

    private void UpdateUI()
    {
        playerHealthSlider.value = playerTowerHealth / maxPlayerTowerHealth;
        enemyHealthSlider.value = enemyTowerHealth / maxEnemyTowerHealth;
        bp.text = "BP: " + battlepoints.ToString();
        bpRate.text = "+" + battlePointsPerSecond.ToString() + "BP/s";
    }

    private void UpdateAI()
    {
        currentBehaviour.spawnsThisTurn = 0;
        var queuedThisTurn = new List<int>();
        var t = Time.deltaTime;
        for(int i = 0; i < currentBehaviour.maxSpawnsPerTurn; i++)
        {
            var index = Random.Range(0, currentBehaviour.spawns.Length);
            var v2 = Random.value;
            if (v2 > currentBehaviour.aggressiveness && currentBehaviour.spawnsThisTurn != 0) break; 
            if(currentBehaviour.spawns[index].timeSinceLastSpawn >= currentBehaviour.spawns[index].timePerSpawn)
            {
                currentBehaviour.queuedSpawns.Add(index);
                queuedThisTurn.Add(index);
                continue;
            }
        }

        for(int i = 0; i < currentBehaviour.spawns.Length; i++)
        {
            if (!queuedThisTurn.Contains(i))
                currentBehaviour.spawns[i].timeSinceLastSpawn += t;
            else
                currentBehaviour.spawns[i].timeSinceLastSpawn = 0;
        }

        if(hasSpawnOpportunity)
        {
            if(currentBehaviour.queuedSpawns.Count > 0)
            {
                var i = currentBehaviour.queuedSpawns[0];
                SpawnEnemy(i);
                currentBehaviour.queuedSpawns.Remove(i);
                timeSinceLastSpawnOpportunity = 0;
                hasSpawnOpportunity = false;
            }
        }
        else
        {
            timeSinceLastSpawnOpportunity += t;
            if (timeSinceLastSpawnOpportunity >= currentBehaviour.spawnSpacing)
                hasSpawnOpportunity = true;
        }
    }

    public void DamageTower(float damage)
    {
        if (hasWon || hasLost) return;
        playerTowerHealth -= damage;
        if (playerTowerHealth <= 0) hasLost = true;
    }

    public void DamageEnemyTower(float damage)
    {
        if (hasWon || hasLost) return;
        enemyTowerHealth -= damage;
        if (enemyTowerHealth <= 0) hasWon = true;
    }
}

enum WarOrder
{
    hold, advance
}

[System.Serializable]
public struct EnemySpawn
{
    public GameObject prefab;
    public float timePerSpawn;
    [System.NonSerialized]
    public float timeSinceLastSpawn;
}

[System.Serializable]
public struct AIBehaviour
{
    public float towerMax;
    public EnemySpawn[] spawns;
    public float bpPerSecond;
    [Range(0.001f,1f)]
    public float aggressiveness;
    [Range(1,10)]
    public int maxSpawnsPerTurn;
    public float spawnSpacing;
    public Rewards winRewards;
    public Rewards loseRewards;
    [System.NonSerialized]
    public int spawnsThisTurn;
    [System.NonSerialized]
    public int lastSpawned;
    [System.NonSerialized]
    public List<int> queuedSpawns;
}

[System.Serializable]
public struct Rewards
{
    public int moneyReward, favourReward, mpReward;
}