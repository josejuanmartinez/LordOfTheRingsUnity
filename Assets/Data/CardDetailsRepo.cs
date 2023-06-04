using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardDetailsRepo : MonoBehaviour
{
    private Dictionary<string, GameObject> cardDetailsDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> cityDetailsDict = new Dictionary<string, GameObject>();

    private bool isInitialized = false;

    void Awake()
    {
        string[] deckCards = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/_GameObjects/Cards/Decks" });
        foreach (var guid in deckCards)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            cardDetailsDict[go.name] = go;
        }

        string[] avatarCards = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/_GameObjects/Cards/Avatars" });
        foreach (var guid in avatarCards)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            cardDetailsDict[go.name] = go;
        }

        string[] cityCards = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/_GameObjects/Cards/_Sites" });
        foreach (var guid in cityCards  )
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            cityDetailsDict[go.name] = go;
        }
        isInitialized = true;
    }

    public GameObject GetCardDetails(string cardId)
    {
        if(!cardDetailsDict.ContainsKey(cardId))
            Debug.LogError("Calling to `GetCardDetails` but " + cardId + " not added to the CardDetailsRepo");
        return cardDetailsDict[cardId];
    }

    public GameObject GetCityDetails(string cardId)
    {
        if (!cityDetailsDict.ContainsKey(cardId))
            Debug.LogError("Calling to `GetCityDetails` but " + cardId + " not added to the CardDetailsRepo");
        return cityDetailsDict[cardId];
    }

    public bool IsInitialized() { return isInitialized; }

}
