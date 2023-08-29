using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    protected new Rigidbody rigidbody;
    protected LineRenderer lineRenderer;
    public VariableJoystick joystick;
    public Vector3 direction;
    public float temph = 0;
    public float tempv = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = GetComponent<MeshRenderer>().materials[0].GetColor("_Color");
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 offset = new Vector3(joystick.transform.position.x - joystick.Horizontal * 12, 0, joystick.transform.position.y - joystick.Vertical * 12);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + offset);

        float h = joystick.Horizontal;
        float v = joystick.Vertical;
        if (h != 0 || v != 0)
        {
            temph = h;
            tempv = v;
        }

        direction = Vector3.left * temph + Vector3.back * tempv;
    }

    public void Shoot()
    {
        rigidbody.AddForce(direction * 380, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(transform.position + "에서" + collision.gameObject.name + "과" + rigidbody.velocity.magnitude + "의 속도로 부딫혔다.");
        if(collision.gameObject.layer != 3)
        {
            Debug.Log("파란 공이 빨간 공을 " + collision.contacts[0].normal + "방향으로 쳤음");
        }
    }
}
