using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGroupManager : MonoBehaviour
{
    public CharacterUIInitializer character;
    public void Initialize(CardInPlay card)
    {
        character.Initialize(card);
    }
}
