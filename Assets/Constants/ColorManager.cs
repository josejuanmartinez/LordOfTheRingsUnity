using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public List<NationsEnum> nations = new List<NationsEnum>();
    public List<Color> colors = new List<Color>();

    public Color GetColor(NationsEnum nation)
    {
        int index = nations.IndexOf(nation);
        return colors[index];
    }
}
