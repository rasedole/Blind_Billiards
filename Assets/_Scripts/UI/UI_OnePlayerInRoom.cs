using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OnePlayerInRoom : MonoBehaviour
{
    public string score
    {
        get { return text.text; }
        set { text.text = value; }
    }

    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(Color color, string _text, Transform pareant)
    {
        color.a = 1;
        image.color = color;
        text.text = _text;
        transform.SetParent(pareant);
        transform.localScale = Vector3.one;
        Vector3 localPos = transform.localPosition;
        localPos.z = 0;
        transform.localPosition = localPos;
        transform.localRotation = Quaternion.identity;  
    }
}
