using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToggleText : MonoBehaviour
{
    public Text text;
    public Toggle toggle;
    public GameObject rtc, ctr;

    private void Start()
    {
        foo();
    }

    public void foo()
    {
        if (toggle.isOn)
            text.text = "Raw > Command";
        else
            text.text = "Command > Raw";

        rtc.SetActive(toggle.isOn);
        ctr.SetActive(!toggle.isOn);
    }
}
