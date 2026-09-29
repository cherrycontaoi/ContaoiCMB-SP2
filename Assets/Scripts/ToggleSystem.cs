using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;

public class ToggleSystem : MonoBehaviour
{
    ToggleGroup toggleGroup;
    Toggle toggle;

    private void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();
    }

    private void Update()
    {
        toggle = toggleGroup.ActiveToggles().FirstOrDefault();
    }

    public string getToggledOn()
    {
        string selected = toggle.GetComponentInChildren<TMP_Text>().text;
        return selected;
    }

    // public void Submit()
    // {
    //     Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
    //     Debug.Log(toggle.name+" _ "+toggle.GetComponentInChildren<Text>().text);
    // }
}
