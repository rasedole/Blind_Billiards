using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitToggleHandler : MonoBehaviour
{
    public void PointerDownHandler()
    {
        gameObject.GetComponent<Image>().color = Color.gray;
    }
    public void PointerUpHandler()
    {
        gameObject.GetComponent<Image>().color = Color.white;
    }
}
