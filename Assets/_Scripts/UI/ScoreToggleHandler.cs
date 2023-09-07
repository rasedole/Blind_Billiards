using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreToggleHandler : MonoBehaviour
{
    Toggle chatToggle;
    [SerializeField] GameObject scoreWrapper;
    [SerializeField] Sprite rigthArrow;
    [SerializeField] Sprite leftArrow;

    private void Start()
    {
        chatToggle = gameObject.GetComponent<Toggle>();
    }

    private void Update()
    {
        if (chatToggle.isOn)
        {
            gameObject.GetComponent<Image>().sprite = leftArrow;
            scoreWrapper.SetActive(true);
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = rigthArrow;
            scoreWrapper.SetActive(false);
        }
    }
}
