using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventInterface : MonoBehaviour
{
    public TextMeshProUGUI moneyText, favourText, manPowerText;
    public TextMeshProUGUI[] policyCostTexts;
    public GameObject[] crossouts;
    public Policy[] policies;

    public ValuedSprite[] defenseValues;
    public ValuedSprite[] attackValues;

    GameController controller;
    AudioSource kaching;
    
    void Start()
    {
        kaching = GetComponent<AudioSource>();
        controller = GameController.instance;
        for(int i = 0; i < defenseValues.Length; i++)
        {
            var defLevel = Mathf.FloorToInt((controller.unitStats[i].maxHealth - controller.unitStatsOriginal[i].maxHealth)/controller.defensePerMan) + 1;
            var attackLevel = Mathf.FloorToInt((controller.unitStats[i].attackDamage - controller.unitStatsOriginal[i].attackDamage)/controller.attackPerMan) + 1;
            defenseValues[i].value = defLevel;
            attackValues[i].value = attackLevel;
        }
    }

    public void GoWar()
    {
        controller.GoWar();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && Application.isEditor) controller.money += 10000;
        UpdateUI();
    }

    public void TryBuyDefense(int index)
    {
        if (controller.manPower < 1 || defenseValues[index].value == 5) return;
        defenseValues[index].value++;
        controller.unitStats[index].maxHealth += controller.defensePerMan;
        controller.manPower -= 1;
        kaching.PlayOneShot(kaching.clip);
        Debug.Log(controller.unitStats[index].ToString());
    }

    public void TryBuyAttack(int index)
    {
        if (controller.manPower < 1 || attackValues[index].value == 5) return;
        attackValues[index].value++;
        controller.unitStats[index].attackDamage += controller.attackPerMan;
        controller.manPower -= 1;
        kaching.PlayOneShot(kaching.clip);
        Debug.Log(controller.unitStats[index].ToString());
    }

    public void TryBuy(int index)
    {
        var p = policies[index];
        switch(index)
        {
            case 0:
            case 1:
                TryBuyMan(p.change, p.cost);
                break;

            case 2:
                TryBuyTHealth(p.change, p.cost);
                break;

            case 3:
                TryBuyFavour(p.change, p.cost);
                break;

            case 4:
                TryBuyBattlePointRate(p.change, p.cost);
                break;

            case 5:
                TryBuyMoney(p.change, p.cost);
                break;
        }
    }

    public void TryBuyMan(int men, int cost)
    {
        if (controller.money >= cost)
        {
            controller.manPower += men;
            controller.money -= cost;
            kaching.PlayOneShot(kaching.clip);
        }
    }

    public void TryBuyFavour(int favour, int cost)
    {
        if (controller.money >= cost)
        {
            controller.favour += favour;
            controller.money -= cost;
            kaching.PlayOneShot(kaching.clip);
        }
    }

    public void TryBuyTHealth(int tHealth, int cost)
    {
        if (controller.money >= cost)
        {
            controller.maxPlayerTowerHealth += tHealth;
            controller.money -= cost;
            kaching.PlayOneShot(kaching.clip);
        }
    }

    public void TryBuyBattlePointRate(int dbps, int cost)
    {
        if (controller.money >= cost)
        {
            controller.battlePointsPerSecond += dbps;
            controller.money -= cost;
            kaching.PlayOneShot(kaching.clip);
        }
    }

    public void TryBuyMoney(int money, int cost)
    {
        if(controller.money >= cost)
        {
            controller.money += money;
            controller.money -= cost;
            kaching.PlayOneShot(kaching.clip);
        }
    }


    void UpdateUI()
    {
        moneyText.text = controller.money.ToString();
        favourText.text = controller.favour.ToString();
        manPowerText.text = controller.manPower.ToString();
        for(int i = 0; i < policyCostTexts.Length; i++)
        {
            policyCostTexts[i].text = policies[i].cost.ToString() + " Money";
        }

        for(int i = 0; i < controller.nextLevel-1; i++)
        {
            crossouts[i].SetActive(true);
        }
    }
}

[System.Serializable]
public struct Policy
{
    public int cost, change;
}