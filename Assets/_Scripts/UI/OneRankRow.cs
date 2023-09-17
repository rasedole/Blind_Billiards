using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OneRankRow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI num;
    [SerializeField]
    private TextMeshProUGUI id;
    [SerializeField]
    private TextMeshProUGUI date;
    [SerializeField]
    private TextMeshProUGUI score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(string _num, string _id, string _date, string _score)
    {
        gameObject.SetActive(true);
        num.text = _num;
        id.text = _id;
        date.text = _date;
        score.text = _score;
    }
}
