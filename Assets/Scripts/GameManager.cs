using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int turn;
    protected GameObject[] gamePlayers;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gamePlayers = GameObject.FindGameObjectsWithTag("Player");
        turn = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetTurnPlayer()
    {
        StartCoroutine(EndTurn());
        return gamePlayers[turn];
    }

    IEnumerator EndTurn()
    {
        bool isNobodyMove = false;
        while(!isNobodyMove)
        {
            for(int i = 0; i < gamePlayers.Length; i++)
            {
                isNobodyMove = true;
                if (gamePlayers[i].GetComponent<Rigidbody>().velocity.magnitude != 0)
                {
                    isNobodyMove = false;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
        }
        if(turn < gamePlayers.Length)
        {
            turn++;
        }
        else
        {
            turn = 1;
        }
    }
}
