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
        if (collision.transform.CompareTag("Wall")) // if the ball hits a wall
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero; // reset it's velocity to 0 so it doesn't move anymore
            transform.position = initialPos; // reset it's position 
        }
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
            } else if (collision.gameObject.CompareTag("BotOutArea"))
            {
                SendShuttlecockTo(bot);
            }
        } else {
            isFirstCollision = false;
        }

    }

    void SendShuttlecockTo(Transform target)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;

        // target이 player인 경우
        if (target == player)
        {
            transform.position = initialPos; // player의 경우 초기 위치로 설정
        }
        // target이 bot인 경우
        else if (target == bot)
        {
            transform.position = new Vector3(0.753f, 1.604f, 3.76f); // bot의 경우 지정된 위치로 설정
        }
        
        isFirstCollision = true;
    }


}