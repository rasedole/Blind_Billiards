using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommandViewer : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CommandCore core;

    // Start is called before the first frame update
    void Start()
    {
        string str = "";
        str += "Delimiter = " + core.delimiter + "\nDefault String = " + core.GetCommand(-1).Substring(1, core.GetCommand(-1).Length - 2) + "\n\n";
        for(int i = 0; i < core.commandCount; i++)
            str += "Command " + (i + 1) + " = " + core.GetCommand(i).Substring(1, core.GetCommand(i).Length - 2) + "\n";

        text.text = str;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
