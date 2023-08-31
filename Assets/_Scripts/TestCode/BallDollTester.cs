using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDollTester : MonoBehaviour
{
    public List<BallDollTesterData> datas;

    // Start is called before the first frame update
    void Start()
    {
        foreach(BallDollTesterData data in datas)
        {
            data.ballDoll.Init(data.color, data.showMode);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public struct BallDollTesterData
{
    public BallDoll ballDoll;
    public Color color;
    public BallShowMode showMode;
}