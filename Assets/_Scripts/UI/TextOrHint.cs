using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextOrHint : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI placeHolder;
    [SerializeField]
    TMP_InputField input;

    public string text
    {
        get
        {
            string input_ = "", placeHolder_ ="";

            if(input != null)
                input_ = input.text.Trim((char)8203);

            if (placeHolder != null)
                placeHolder_ = placeHolder.text.Trim((char)8203);

            if (input_ == "")
                return placeHolder_;
            else
                return input_;
        }

        set
        { input.text = value; }
    }
}
