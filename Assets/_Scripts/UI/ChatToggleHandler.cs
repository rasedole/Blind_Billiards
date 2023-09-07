using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatToggleHandler : MonoBehaviour
{
    Toggle chatToggle;
    [SerializeField] GameObject chatWrapper;

    private void Start()
    {
        chatToggle = gameObject.GetComponent<Toggle>();
    }

    private void Update()
    {
        if (chatToggle.isOn)
        {
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Chat Off";
            chatWrapper.SetActive(true);
        }
        else
        {
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Chat On";
            chatWrapper.SetActive(false);
        }
    }
}
