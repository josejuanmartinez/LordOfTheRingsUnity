using UnityEngine;
using UnityEngine.UI;

public class Attacker : MonoBehaviour
{
    public void Initialize(Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite;
    }
}
