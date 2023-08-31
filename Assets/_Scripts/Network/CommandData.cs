using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CommandData
{
    public string text;
    public int command;

    public CommandData(int command_, string text_)
    {
        text = text_;
        command = command_;
    }
}
