using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum DiceRollEnum
{
    CharacterRoll,
    HazardCreatureRoll,
    ObjectRoll
}

public class DiceManager : MonoBehaviour
{
    public static short D10 = 10;
    public short maxTime = 4;

    public GameObject dice;
    public GameObject diceCanvas;

    private GameObject instantiatedDice;
    private int diceValue;

    private PlaceDeck placeDeck;
    private bool dicing = false;

    DiceRollEnum diceRollType;

    void Awake()
    {
        placeDeck = GameObject.Find("PlaceDeck").GetComponent<PlaceDeck>();    
    }

    public IEnumerator Roll(DiceRollEnum diceRollType, SpawnCardLocation spawnCardLocation, CardDetails cardDetails)
    {
        this.diceRollType = diceRollType;
        yield return StartCoroutine(RollDice());
        yield return StartCoroutine(CheckResult(spawnCardLocation, cardDetails));
    }

    public IEnumerator RollDice()
    {
        dicing = true;
        diceCanvas.SetActive(true);
        instantiatedDice = Instantiate(dice, diceCanvas.transform);

        diceValue = 0;
        Die_d10 d10 = instantiatedDice.GetComponentInChildren<Die_d10>();
        for (int i = 0; i < maxTime; i++)
        {
            yield return new WaitForSeconds(1);
            if (d10.value != 0)
            {
                diceValue = d10.value;
                break;
            }
        }

        if (diceValue == 0)
            diceValue = Random.Range(1, 10);

        dicing = false;
        StopAllCoroutines();
    }

    public IEnumerator CheckResult(SpawnCardLocation spawnCardLocation, CardDetails cardDetails)
    {
        yield return new WaitUntil(() => diceValue != 0);
        DestroyImmediate(instantiatedDice);
        diceCanvas.SetActive(false);
        placeDeck.GatherDiceResults(diceValue, spawnCardLocation, cardDetails);
    }

    public bool IsDicing()
    {
        return dicing;
    }
}
