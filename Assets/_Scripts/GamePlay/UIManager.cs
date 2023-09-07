using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    public TMP_Text currentTurnText;
    public TMP_Text turnTableText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdateInitiate()
    {
        turnTableText.text = "";
        for (int i = 0; i < GameManager.Instance.gamePlayers.Count; i++)
        {
            turnTableText.text += "Turn" + (i + 1).ToString() + ": " + TurnManager.Instance.GetTurnBall(i).name + "\n";
        }
    }

    public void UpdateTurn(int turn)
    {
        currentTurnText.text = "Turn " + turn.ToString();
    }
}
