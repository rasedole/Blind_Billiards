using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//목적1 : 각 공의 점수를 저장하고 관리한다.
//속성1 : 플레이어 이름과 점수를 가지고 있는 Dictionary

public class ScoreManager : MonoBehaviour
{
    //플레이어 이름과 점수를 가지고 있는 Dict
    Dictionary<string, int> scoreList = new Dictionary<string, int>();

    void Start()
    {
        
    }
}
