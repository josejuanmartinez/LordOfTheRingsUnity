using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct okOption
{
    public string text;
    public UnityAction cardBoolFunc;
}

public struct cancelOption
{
    public string text;
    public UnityAction cardBoolFunc;
}

public class PopupManager : MonoBehaviour
{
    public GameObject popup;
    public GameObject okButton;
    public GameObject cancelButton;
    public GameObject grid;
    public TextMeshProUGUI text;
    public void Initialize(string title, List<okOption> options, cancelOption cancelOption)
    {
        ShowPopup();
        text.text = title;

        int childCount = grid.transform.childCount;
        for(int i = 0; i < childCount;i++)
            DestroyImmediate(grid.transform.GetChild(0).gameObject);
        
        for(int i=0; i< options.Count; i++)
        {
            GameObject go = Instantiate(okButton, grid.transform);
            TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
            text.text = options[i].text;
            Button button = go.GetComponentInChildren<Button>();
            button.onClick.AddListener(options[i].cardBoolFunc);
        }
        GameObject goCancel = Instantiate(cancelButton, grid.transform);
        TextMeshProUGUI textCancel = goCancel.GetComponentInChildren<TextMeshProUGUI>();
        textCancel.text = cancelOption.text;
        Button buttonCancel = goCancel.GetComponentInChildren<Button>();
        buttonCancel.onClick.AddListener(cancelOption.cardBoolFunc);
    }

    public void ShowPopup()
    {
        popup.gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        popup.gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return popup.gameObject.activeSelf;
    }
}
