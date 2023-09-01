using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RawToCommand : MonoBehaviour
{
    public CommandCore command;
    public TMP_InputField inputField;
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Make()
    {
        text.text = "";

        List<CommandData> datas = CommandCore.Decode(command, inputField.text);
        foreach(CommandData data in datas)
            text.text += "Command " + command.GetCommand(data.command) + "(" + data.command + ") = " + data.text + "\n";
    }

    private void OnEnable()
    {
        inputField.text = "";
        text.text = "Null";
    }
}
