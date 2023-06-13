using System.Collections.Generic;
using UnityEngine;

public class ManaPool: MonoBehaviour
{
    public TMPro.TextMeshProUGUI textFreeBastion;
    public TMPro.TextMeshProUGUI textDarkBastion;
    public TMPro.TextMeshProUGUI textFrontierBastion;
    public TMPro.TextMeshProUGUI textNeutralBastion;
    public TMPro.TextMeshProUGUI textLair;
    public TMPro.TextMeshProUGUI textWilderness;
    public TMPro.TextMeshProUGUI textSea;

    public Dictionary<CardTypesEnum, short> manaPool = new Dictionary<CardTypesEnum, short> {
        { CardTypesEnum.FREE_BASTION, 0 },
        { CardTypesEnum.FRONTIER_BASTION, 0 },
        { CardTypesEnum.DARK_BASTION, 0 },
        { CardTypesEnum.NEUTRAL_BASTION, 0 },
        { CardTypesEnum.LAIR, 0 },
        { CardTypesEnum.WILDERNESS, 0 },
        { CardTypesEnum.SEA, 0 }
    };

    public bool dirty = false;
    void Update()
    {
        if (dirty)
        {
            textFreeBastion.text = manaPool[CardTypesEnum.FREE_BASTION].ToString();
            textDarkBastion.text = manaPool[CardTypesEnum.DARK_BASTION].ToString();
            textFrontierBastion.text = manaPool[CardTypesEnum.FRONTIER_BASTION].ToString();
            textNeutralBastion.text = manaPool[CardTypesEnum.NEUTRAL_BASTION].ToString();
            textLair.text = manaPool[CardTypesEnum.LAIR].ToString();
            textWilderness.text = manaPool[CardTypesEnum.WILDERNESS].ToString();
            textSea.text = manaPool[CardTypesEnum.SEA].ToString();
            dirty = false;
        }
    }

    public void AddMana(CardTypesEnum cardType)
    {
        manaPool[cardType]++;
    }

    public void ToggleDirty()
    {
        dirty = true;
    }

    public bool HasEnoughMana(List<CardTypesEnum> cards)
    {
        foreach (CardTypesEnum card in cards) {
            int required = cards.FindAll(x => x.Equals(card)).Count;
            if(required > 0)
            {
                if (manaPool[card] >= required)
                    return true;
            }            
        }
        return false;
    }

    public float GetHazardCreatureSuccessProbability(CardDetails cardDetails)
    {
        if (cardDetails.GetHazardCreatureCardDetails() == null)
            return 0.0f;
        else
            return GetHazardCreatureSuccessProbability(cardDetails.GetHazardCreatureCardDetails().cardTypes);
    }

    public float GetHazardCreatureSuccessProbability(List<CardTypesEnum> cards)
    {
        if (!HasEnoughMana(cards))
            return 0f;

        short required = (short) cards.Count;
        short missing = 0;

        Dictionary<CardTypesEnum, short> checkPool = new Dictionary<CardTypesEnum, short>(manaPool);
        foreach (CardTypesEnum card in cards)
        {
            checkPool[card]--;
            if (checkPool[card]<0)
                missing++;
        }

        return missing * 1f / required;
    }
}
