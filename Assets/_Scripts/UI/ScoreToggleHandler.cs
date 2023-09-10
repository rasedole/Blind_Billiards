using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreToggleHandler : MonoBehaviour
{
    private bool chatToggle;


    [SerializeField] 
    private GameObject scoreWrapper;
    [SerializeField] 
    private Sprite spriteWhenClose;
    [SerializeField] 
    private Sprite spriteWhenOpen;

    private void Awake()
    {
        chatToggle = true;
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void Toggle()
    {
        chatToggle = !chatToggle;

        if (chatToggle)
        {
            gameObject.GetComponent<Image>().sprite = spriteWhenOpen;
            scoreWrapper.SetActive(true);
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = spriteWhenClose;
            scoreWrapper.SetActive(false);
        }
    }
}
