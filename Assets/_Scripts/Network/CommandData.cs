using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CommandData
{
    public string text;
    public int command;

    public CommandData(int _command, string _text)
    {
        text = _text;
        command = _command;
    }
}
