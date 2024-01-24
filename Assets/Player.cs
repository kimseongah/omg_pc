using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public Transform aimTarget;
    float speed = 3f;
    float hitForce = 9f;
    float upForce = 9f;
    bool isHitting;
    public Transform shuttlecock;
    Animator animator;
    Vector3 aimTargetInitialPosition;
    public GameObject playerInArea;
    public float minDistanceToHit = 0f;

    Vector3 origin = new Vector3(1.28f, 0.3825f, 0.67f);
    const float r = 0.8f;
    private float lowPassFilterFactor = 0.1f;
    private Vector3 lowPassValue = Vector3.zero;

    // 예상 위치에 도달할 시간 (초)
    public float predictionTime = 2f;

    bool isNearBy = false;
    string objectName;
    public Transform racquet;
    public float smoothMoveSpeed = 5f; 
    public Collider targetArea;
    private void Start()
    {   
        animator = GetComponent<Animator>();
        aimTargetInitialPosition = aimTarget.position;
        objectName = gameObject.name;
        if (racquet == null)
        {
            racquet = transform.Find("racquet");
        }
        if (objectName == "player")
        {
            transform.position = new Vector3(0.176f, 1.62f, -10.22f);
        }
        // target이 bot인 경우
        else if (objectName == "player2")
        {
            transform.position = new Vector3(0.904f, 1.62f, 7.45f);
        }
    }

    void Update()
    {
        Rigidbody shuttlecockRb = shuttlecock.GetComponent<Rigidbody>();
        Ball shuttlecockScript = shuttlecock.GetComponent<Ball>();
        // 서브일 때 포물선으로 올려주기
        // 나한테 다가올 때
            //y==racquet.y일 때 t구하고 위치 예측해서

        if (objectName == "player"){
            if (Input.GetKeyDown(KeyCode.Return) && isNearBy) // 엔터를 눌렀을 때
            {
                HitShuttlecock();
                PlayerPrefs.SetInt("whoIsHitting", 1);
                PlayerPrefs.Save();
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Space) && isNearBy) // 스페이스바를 눌렀을 때
            {
                HitShuttlecock();
                PlayerPrefs.SetInt("whoIsHitting", 2);
                PlayerPrefs.Save();
            }
        }
        int whoIsHitting = PlayerPrefs.GetInt("whoIsHitting", 0);
        
        if(whoIsHitting == 1 && objectName == "player2")
        {
            Vector3 predictedPosition = CalculateLandingPosition(shuttlecockRb.position, shuttlecockRb.velocity, racquet.position.y);
            predictedPosition.x += 0.8f;
            predictedPosition.z -= 1;
            StartCoroutine(MoveToPosition(predictedPosition));
            PlayerPrefs.SetInt("whoIsHitting", 0);
            PlayerPrefs.Save();
        } 
        else if(whoIsHitting == 2 && objectName == "player")
        {
            Vector3 predictedPosition = CalculateLandingPosition(shuttlecockRb.position, shuttlecockRb.velocity, racquet.position.y);
            predictedPosition.x -= 0.8f;
            predictedPosition.z += 1;
            StartCoroutine(MoveToPosition(predictedPosition));
            PlayerPrefs.SetInt("whoIsHitting", 0);
            PlayerPrefs.Save();
        }
        
    }

    // 셔틀콕의 초기 위치, 초기 속도, 목표 y 위치(라켓의 y 위치)를 받아서 x, z 위치를 반환
    public Vector3 CalculateLandingPosition(Vector3 initialPosition, Vector3 initialVelocity, float targetYPosition)
    {
        float gravity = Physics.gravity.y;

        Vector3 adjustedVelocity = initialVelocity;

        Debug.Log("v: " + adjustedVelocity);

        // 조정된 속도를 사용하여 시간 계산
        float time;
        if (Mathf.Approximately(adjustedVelocity.y, 0))
        {
            time = Mathf.Sqrt(2 * (targetYPosition - initialPosition.y) / -gravity);
        }
        else
        {
            float discriminant = adjustedVelocity.y * adjustedVelocity.y + 2 * gravity * (initialPosition.y - targetYPosition);
            if (discriminant < 0)
            {
                Debug.LogError("The object does not reach the target Y position.");
                return new Vector3(0, 1.62f, 0); 
            }
            time = (adjustedVelocity.y + Mathf.Sqrt(discriminant)) / (-gravity);
        }

        // 시간 t에서의 x, z 위치 계산
        float x = initialPosition.x + adjustedVelocity.x * time;
        float z = initialPosition.z + adjustedVelocity.z * time;
        return new Vector3(x, targetYPosition, z);
    }



    // 해당 위치로 이동
    IEnumerator MoveToPosition(Vector3 position)
    {
        if (!Mathf.Approximately(position.y, 1.62f))
        {
            Debug.Log(position);
            position.y = 1.62f;
            while (Vector3.Distance(transform.position, position) > 0.01f) // 목표 위치에 가까워질 때까지 루프
            {
                transform.position = Vector3.Lerp(transform.position, position, smoothMoveSpeed * Time.deltaTime);
                yield return null; // 다음 프레임까지 대기
            }
        }
    }

    bool IsShuttlecockApproaching(Rigidbody shuttlecockRb)
    {
        Vector3 directionToPlayer = transform.position - shuttlecock.position;
        float angle = Vector3.Angle(shuttlecockRb.velocity, directionToPlayer);

        // 셔틀콕이 플레이어 방향으로 움직이고 있는지 확인 (예: 90도 이내)
        return angle < 90.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) // if we collide with the shuttlecock 
        {
            isNearBy = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball")) // Ball 태그를 가진 오브젝트와 떨어졌을 때
        {
            isNearBy = false;
        }
    }

    private void HitShuttlecock()
    {
        // 라켓의 회전을 기반으로 힘의 방향 계산
        Vector3 forwardDirection = transform.forward;
        Vector3 hitDirection = forwardDirection.normalized * hitForce + Vector3.up * upForce;

        // 셔틀콕에 힘 적용
        shuttlecock.GetComponent<Rigidbody>().velocity = hitDirection;

        // 애니메이션 재생
        Vector3 shuttlecockDir = shuttlecock.position - transform.position;
        if (shuttlecockDir.x >= 0)
        {
            animator.Play("forehand");
        }
        else
        {
            animator.Play("backhand");
        }
        
    }

}