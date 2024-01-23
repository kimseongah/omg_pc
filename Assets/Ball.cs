using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Transform player;
    public Transform bot;
    Vector3 initialPos; // ball's initial position

    private bool isFirstCollision = true;

    private void Start()
    {
        initialPos = transform.position; // default it to where we first place it in the scene
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(isFirstCollision);
        if (!isFirstCollision) // 첫 충돌이 아니면 로직 수행
        {
            if (collision.gameObject.CompareTag("PlayerInArea"))
            {
                SendShuttlecockTo(bot);
                
            } else if (collision.gameObject.CompareTag("BotInArea"))
            {
                SendShuttlecockTo(player);
                
            } else if (collision.gameObject.CompareTag("PlayerOutArea"))
            {
                SendShuttlecockTo(player);
            } 
            else if (collision.gameObject.CompareTag("BotOutArea"))
            {
                SendShuttlecockTo(bot);
            }

            if (collision.gameObject.CompareTag("Player") && collision.gameObject.transform == player)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        } else {
            isFirstCollision = false;
        }

        
    }

    void SendShuttlecockTo(Transform target)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // target이 player인 경우
        if (target == player)
        {
            transform.position = new Vector3(0.61f, 1.664f, -9.87f); // player의 경우 초기 위치로 설정
        }
        // target이 bot인 경우
        else if (target == bot)
        {
            transform.position = new Vector3(0.753f, 1.664f, 7.25f); // bot의 경우 지정된 위치로 설정
        }

        isFirstCollision = true;
    }
}