using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandToRaw : MonoBehaviour
{
    public CommandCore command;
    public TMP_InputField inputField;
    public TextMeshProUGUI text, list;
    public TMP_Dropdown dropdown;
    List<CommandData> commands;

    // Start is called before the first frame update
    void Start()
    {
        dropdown.ClearOptions();
        List<string> s = new List<string>();
        s.Add(command.GetCommand(-1).Substring(1, command.GetCommand(-1).Length - 2));

        for (int i = 0; i < command.commandCount; i++)
            s.Add(command.GetCommand(i).Substring(1, command.GetCommand(i).Length - 2));

        dropdown.AddOptions(s);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        commands = new List<CommandData>();
        inputField.text = "";
        text.text = "Null";
        list.text = "Null";
    }

    public void add()
    {
        commands.Add(new CommandData(dropdown.value - 1, inputField.text));
        string s = "";
        foreach(CommandData data in commands)
            s += data.command + ", " + data.text + "\n";
        list.text = s;
    }

    public void foo()
    {
        text.text = CommandCore.Encode(command, commands);
    }
}
