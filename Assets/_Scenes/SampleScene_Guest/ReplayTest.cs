using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayTest : MonoBehaviour
{
    public List<MoveData> moves;
    public bool sampleSceneIsReplayer;

    public void Foos()
    {
        if(!sampleSceneIsReplayer)
        {
            moves = GameManager.Instance.ballMoveData;
        }

        for (int i = 0; i < moves.Count; i++)
        {
            MoveData temp = moves[i];
            temp.index = i;
            moves[i] = temp;
        }
        GuestReplayer.ReplayTurn(moves);
    }
}
