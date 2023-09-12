using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//목적1 : 조이스틱을 당길 때 날아가는 방향을 표시하는 선을 그린다.
//속성1 : 입력값을 받아올 JoyStick, 선을 그릴 LineRenderer, 벽을 찾을 Ray와 RayCastHit, 조이스틱이 눌러졌는지 확인할 isClicked, 공의 위치
//순서1-1. 조이스틱을 당긴다.
//순서1-2. 해당 방향으로 Ray를 날린다.
//순서1-3. Ray가 벽과 부딫히면 그 거리를 LineRenderer에게 준다.

public class BallLineRender : MonoBehaviour
{
    //속성1 : 입력값을 받아올 JoyStick, 선을 그릴 LineRenderer, 벽을 찾을 Ray와 RayCastHit, 조이스틱이 눌러졌는지 확인할 isClicked, 공의 위치
    public bool isClicked = false;

    FixedJoystick joystick;
    LineRenderer lineRenderer;
    Ray lineRay;
    RaycastHit hitinfo = new();
    Vector3 ballPosition;

    // Start is called before the first frame update
    void Start()
    {
        joystick = GetComponent<FixedJoystick>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isNobodyMove)
        {
            if (isClicked)
            {
                if (GameManager.Instance.CheckMyBall() || TCP_BallCore.networkMode == NetworkMode.None)
                {
                    Vector3 offset = new Vector3(- joystick.Horizontal, 0, - joystick.Vertical);
                    lineRay = new Ray(ballPosition, offset.normalized);
                    int _layerMask = 1 << LayerMask.NameToLayer("Wall");
                    if (Physics.Raycast(lineRay, out hitinfo, 10, _layerMask))
                    {
                        //순서2-2. 해당 방향을 LineRenderer에 넣어준다.
                        lineRenderer.SetPosition(0, ballPosition);
                        lineRenderer.SetPosition(1, ballPosition + offset * hitinfo.distance);
                    }
                    else
                    {
                        //순서2-2. 해당 방향을 LineRenderer에 넣어준다.
                        lineRenderer.SetPosition(0, ballPosition);
                        lineRenderer.SetPosition(1, ballPosition + offset * 2);
                    }
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(0, ballPosition);
            lineRenderer.SetPosition(1, ballPosition);
        }
    }

    public void ResetBallStatus()
    {
        lineRenderer = TurnManager.Instance.GetTurnBall().GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.endColor = TurnManager.Instance.GetTurnBall().GetComponent<BallDoll>().showcaseColor;
        ballPosition = TurnManager.Instance.GetTurnBall().transform.position;
    }
}
