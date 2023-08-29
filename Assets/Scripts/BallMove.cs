using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//목적1 : 조이스틱을 당겼다가 놓으면 당긴 반대 방향으로 공이 날아가도록 한다.
//속성1 : 조이스틱, 날아갈 방향, 공을 날리기 위한 공의 RigidBody, 날리는 힘
//속성추가 : 조이스틱을 놓는 순간 이벤트가 발생하는데 그러면 조이스틱의 vertical, horizontal 값이 0이 되기에 조이스틱을 놓기 직전 값을 기준으로 날아가기 위해서 tempH, tempV 사용;
//순서1-1. 조이스틱을 당긴 값을 저장한다.
//순서1-2. 조이스틱을 놓는다.
//순서1-3. 조이스틱을 놓기 직전 값을 기준으로 방향을 정한다.
//순서1-4. 해당 방향을 향해 날리는 힘을 곱해서 힘을 가한다.


//목적2 : 조이스틱을 당길 때 날아가는 방향을 표시하는 선을 그린다.
//속성2 : 선을 그릴 LineRenderer
//순서2-1. 조이스틱을 당긴 값으로 공이 날아갈 방향을 구한다.
//순서2-2. 해당 방향을 LineRenderer에 넣어준다.

//목적3 : 게임매니저에 현재 턴을 지정하고 자신의 턴이 아닐 때는 작동하지 않도록 한다.
//속성3 : 내가 조작할 수 있는 턴
//순서3-1. 내 턴이 아니면 return한다.

public class BallMove : MonoBehaviour
{
    //속성1 : 조이스틱, 날아갈 방향, 공을 날리기 위한 공의 RigidBody, 날리는 힘
    public VariableJoystick joystick;
    public Vector3 direction;
    protected new Rigidbody rigidbody;
    public float power = 380;
    //속성추가 : 조이스틱을 놓는 순간 이벤트가 발생하는데 그러면 조이스틱의 vertical, horizontal 값이 0이 되기에 조이스틱을 놓기 직전 값을 기준으로 날아가기 위해서 tempH, tempV 사용;
    public float temph = 0;
    public float tempv = 0;

    //속성2 : 선을 그릴 LineRenderer
    protected LineRenderer lineRenderer;

    //속성3 : 내가 조작할 수 있는 턴
    public int myTurn;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = GetComponent<MeshRenderer>().materials[0].GetColor("_Color");
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.2f;
        //Debug.Log(gameObject.name + "은 " + myTurn + "에 움직인다.");
    }

    // Update is called once per frame
    void Update()
    {
        //순서3-1. 내 턴이 아니면 return한다.
        if (GameManager.Instance.turn != myTurn || !GameManager.Instance.isNobodyMove)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            return;
        }


        //순서1-1. 조이스틱을 당긴 값을 저장한다.
        float h = joystick.Horizontal;
        float v = joystick.Vertical;
        //조이스틱을 놓는 순간 h와 v값이 0이 되기에 두 값이 0이 아닐 경우에만 temph, tempv를 갱신한다
        if (h != 0 || v != 0)
        {
            temph = h;
            tempv = v;
        }

        //순서1-2. 조이스틱을 놓는다.     
        //순서1-3. 조이스틱을 놓기 직전 값을 기준으로 방향을 정한다.
        //VariableJoyStick에 구현되어 있는 OnPointerUp이 조이스틱을 떼는 순간에 작동하기에 그 함수가 실행될 때 하단에 있는 Shoot()함수를 실행시킴으로서 공을 발사한다.
        direction = Vector3.left * temph + Vector3.back * tempv;

        //순서2-1. 조이스틱을 당긴 값으로 공이 날아갈 방향을 구한다.
        Vector3 offset = new Vector3(joystick.transform.position.x - joystick.Horizontal * 12, 0, joystick.transform.position.y - joystick.Vertical * 12);

        //순서2-2. 해당 방향을 LineRenderer에 넣어준다.
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + offset);
    }

    public void Shoot()
    {
        //순서1-4. 해당 방향을 향해 날리는 힘을 곱해서 힘을 가한다.
        rigidbody.AddForce(direction * power, ForceMode.Impulse);
    }

    //2023/08/29 기준 OnColiisionEnter는 오브젝트가 부딫힐 때의 여러 값을 Console에 표시하기 위해서 사용하므로 주석처리함
    //private void OnCollisionEnter(Collision collision)
    //{
        
    //    Debug.Log(transform.position + "에서" + collision.gameObject.name + "과" + rigidbody.velocity.magnitude + "의 속도로 부딫혔다.");
    //    if (collision.gameObject.layer != 3)
    //    {
    //        Debug.Log(gameObject.name + "이/가 " + collision.gameObject.name + "을/를 " + collision.contacts[0].normal + "방향으로 쳤음");
    //    }
    //}
}
