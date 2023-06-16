using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationEN
{
    public static Dictionary<string, string> loc = new Dictionary<string,string>() {
        {NationsEnum.UVATHA.ToString(), "Ûvatha the Horseman" },
        {NationsEnum.KHAMUL.ToString(), "Khamûl the Easterling" },
        {NationsEnum.ADUNAPHEL.ToString(), "Adûnaphel" },
        {NationsEnum.BEORN_THE_GOBLIN_BASHER.ToString(), "Beorn \"Goblin-Basher\""},
        {NationsEnum.BARD_BLACK_ARROW.ToString() , "Bard \"Black-Arrow\"" },
        {NationsEnum.THRANDUIL_OF_MIRKWOOD.ToString(), "Thranduil of Mirkwood" },
        {NationsEnum.HUZ.ToString(), "Huz III" },
        {NationsEnum.THE_MOUTH.ToString(), "The Mouth of Sauron" },
        //{ NationsEnum.SARUMAN, AlignmentsEnum.FREE_PEOPLE },
        {NationsEnum.SARUMAN_OF_MANY_COLOURS.ToString(), "Saruman of Many Colours" },
        {NationsEnum.THEODEN_KING.ToString(), "Theoden King" },
        {NationsEnum.THRAIN.ToString(), "Thráin" },
        {NationsEnum.THE_WITCH_KING.ToString(), "The Witch-King" },
        //{ NationsEnum.RADAGAST, AlignmentsEnum.FREE_PEOPLE },
        {NationsEnum.RADAGAST_SHADOWED_WANDERER.ToString(), "Radagast \"Shadow-Wanderer\"" },
        {NationsEnum.LADY_GALADRIEL.ToString(), "Lady Galadriel"},
        {NationsEnum.THE_BALROG.ToString(), "The Balrog" },
        {NationsEnum.SMAUG_THE_GOLDEN.ToString(), "Smaug The Golden" },

        {NationRegionsEnum.ANDUIN_VALES.ToString(), "Anduin Vales" },
        {NationRegionsEnum.ANGMAR.ToString(), "Angmar" },
        {NationRegionsEnum.CHELKAR.ToString(), "Chelkar" },
        {NationRegionsEnum.GAP_OF_ISEN.ToString(), "Gap of Isen" },
        {NationRegionsEnum.GORGOROTH.ToString(), "Gorgoroth" },
        {NationRegionsEnum.HORSE_PLAINS.ToString(), "The Horse Plains" },
        {NationRegionsEnum.IRON_HILLS.ToString(), "The Iron Hills" },
        {NationRegionsEnum.NORTHERN_RHOVANION.ToString(), "Northern Rhovanion" },
        {NationRegionsEnum.REDHORN_GATE.ToString(), "The Redhorn Gate" },
        {NationRegionsEnum.SEA_OF_RHUN.ToString(), "Sea of Rhûn" },
        {NationRegionsEnum.SOUTHERN_MIRKWOOD.ToString(), "Southern Mirkwood" },
        {NationRegionsEnum.WESTERN_MIRKWOOD.ToString(), "Western Mirkwood" },
        {NationRegionsEnum.WITHERED_HEATH.ToString(), "Withered Heath" },
        {NationRegionsEnum.WOLD_AND_FOOTHILLS.ToString(), "Wold & Foothills" },
        {NationRegionsEnum.WOODLAND_REALM.ToString(), "Woodland Realm" },
    };

    public static string Localize(string key)
    {
        return loc.ContainsKey(key) ? loc[key] : "*" + key + "*";
    }
}
