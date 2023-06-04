using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile
{
    Vector2Int hex;
    CityInPlay city;
    List<CardInPlay> cards = new List<CardInPlay>();

    public BoardTile(Vector2Int hex, CityInPlay city, List<CardInPlay> cards)
    {
        this.hex = hex;
        this.city = city;
        this.cards = cards;
    }

    public BoardTile(Vector2Int hex, CityInPlay city)
    {
        this.hex = hex;
        this.city = city;
        this.cards = new List<CardInPlay>();
    }

    public BoardTile(Vector2Int hex, CardInPlay card)
    {
        this.hex = hex;
        this.city = null;
        this.cards = new List<CardInPlay>() { card };
    }

    public BoardTile(Vector2Int hex)
    {
        this.hex = hex;
        this.city = null;
        this.cards = new List<CardInPlay>();
    }

    public BoardTile(Vector2Int hex, List<CardInPlay> cards)
    {
        this.hex = hex;
        this.city = null;
        this.cards = cards;
    }

    public void RemoveCard(CardInPlay card)
    {
        cards.Remove(card);
    }

    public void AddCard(CardInPlay card)
    {
        cards.Add(card);
    }

    public void RemoveCity(CityInPlay city)
    {
        city = null;
    }

    public void AddCity(CityInPlay city)
    {
        this.city = city; 
    }

    public bool HasCity()
    {
        return city != null;
    }

    public bool HasCards()
    {
        return cards.Count > 0;
    }

    public CityInPlay GetCity()
    { 
        return city; 
    }

    public List<CardInPlay> GetCards()
    { 
        return cards; 
    }

}
