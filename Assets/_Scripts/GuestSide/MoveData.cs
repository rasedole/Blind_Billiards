using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// List<MoveData> moveDatas == OneTurnData
[System.Serializable]
public struct MoveData
{
    [HideInInspector]
    public int index;

    public int ballIndex;
    public Vector3 startPos;
    public float startTime;
}
