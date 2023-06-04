using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public List<GameObject> toActivate;

    private List<Player> players = new List<Player>();
    private short humanPlayer = 0;

    private bool isInitialized = false;
    void Awake()
    {
        foreach(GameObject go in toActivate)
        {
            if (!go.activeSelf)
                go.SetActive(true);
        }
        
        for(int i=0; i<Enum.GetValues(typeof(NationsEnum)).Length; i++)
        {
            if ((NationsEnum)i == NationsEnum.NONE)
                continue;
            players.Add(new Player((NationsEnum)i));
        }
        isInitialized = true;
    }
    public Player GetHumanPlayer()
    {
        return players[humanPlayer];
    }

    public bool IsInitialized() {
        return isInitialized;
    }
}
