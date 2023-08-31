using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Command Settings", menuName = "menu/Command Settings", order = 1)]
public class CommandCore : ScriptableObject
{
    [SerializeField]
    char delimiter_;
    public char delimiter { get { return delimiter_; } }

    [SerializeField]
    string defaultStr;
    [SerializeField]
    List<string> commands;
    public int commandCount { get { return commands.Count; } }
    public string GetCommand(int index)
    {
        string str = delimiter_.ToString();
        if (index == -1)
        {
            str += defaultStr + delimiter_.ToString();
            return str;
        }
        else if (index < -1)
        {
            str += "error";
            return str;
        }

        str += commands[index] + delimiter_.ToString();
        return str;
    }

    // Decode raw data.
    // Separate data and commands.
    public static List<CommandData> Decode(CommandCore core, string rawData)
    {
        // Find command position.
        List<CommandData> data = new List<CommandData>();
        Dictionary<int, List<int>> commandPosRaw = new Dictionary<int, List<int>>();
        List<int> list;
        string read;
        for (int i = 0; i < core.commandCount; i++)
        {
            read = rawData;
            list = new List<int>();
            while (read.IndexOf(core.GetCommand(i)) > -1)
            {
                list.Add(read.IndexOf(core.GetCommand(i)) + (list.Count * core.GetCommand(i).Length));
                read = read.Substring(0, read.IndexOf(core.GetCommand(i))) + read.Substring(read.IndexOf(core.GetCommand(i)) + core.GetCommand(i).Length);
            }

            if (list.Count > 0)
                commandPosRaw.Add(i, list);
        }

        // Check command not exist.
        if (commandPosRaw.Count < 1)
        {
            data.Add(new CommandData(-1, RemoveDelimiter(rawData, core.delimiter)));
            return data;
        }

        // Sorting command position, also check data starts with commands.
        Dictionary<int, int> commandPos = new Dictionary<int, int>();
        bool defaultString = true;
        foreach (int cmdIndex in commandPosRaw.Keys)
            foreach (int index in commandPosRaw[cmdIndex])
            {
                commandPos.Add(index, cmdIndex);
                if (index == 0)
                    defaultString = false;
            }

        // Slice raw data(string) with command position.
        List<string> slices = new List<string>();
        slices.Add(rawData);
        int pointer = 0;

        foreach (int index in commandPos.Keys)
        {
            int leng = index;
            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i].Length > leng)
                {
                    pointer = i;
                    break;
                }
                else
                    leng -= slices[i].Length;
            }

            string slice = slices[pointer];
            slices.RemoveAt(pointer);

            slices.Insert(pointer, slice.Substring(0, leng));
            pointer++;
            slices.Insert(pointer, slice.Substring(leng, core.GetCommand(commandPos[index]).Length));
            pointer++;
            slices.Insert(pointer, slice.Substring(leng + core.GetCommand(commandPos[index]).Length));

            // Make command data.
            data.Add(new CommandData(commandPos[index], ""));
        }

        // Remove empty string(only need first).
        if (slices[0].Length == 0)
            slices.RemoveAt(0);

        // Make default data.
        CommandData defaultData = new CommandData(-1, "");
        if (defaultString)
        {
            defaultData = new CommandData(-1, slices[0]);
            slices.RemoveAt(0);
        }

        // Complete command data.
        for (int i = 0; i < slices.Count; i += 2)
        {
            pointer = (i / 2);
            if (pointer < data.Count)
                data[pointer].text = slices[i + 1];
        }

        // Complete default data.
        if (defaultString)
            data.Insert(0, defaultData);

        // Remove double delimiter.
        foreach (CommandData d in data)
            d.text = RemoveDelimiter(d.text, core.delimiter);

        return data;
    }

    static string RemoveDelimiter(string input, char target)
    {
        string val = target.ToString() + target.ToString();
        while (input.IndexOf(val) > -1)
            input = input.Substring(0, input.IndexOf(val)) + input.Substring(input.IndexOf(val) + 1);

        return input;
    }

    public static string Encode(CommandCore core, List<CommandData> data)
    {
        string rawData = "";
        for (int i = 0; i < data.Count; i++)
            rawData += core.GetCommand(data[i].command) + data[i].text;

        return rawData;
    }
}
