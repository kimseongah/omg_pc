using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    float hitForce;
    float upForce;
    public Transform shuttlecock;
    Animator animator;
    bool isNearBy = false;
    string objectName;
    public Transform racquet;
    private Quaternion lastRotation; // 이전 프레임에서의 회전
    public float rotationThreshold = 50000; // 회전 속도 임계값
    public float smoothMoveSpeed = 5f;

    private void Start()
    {
        hitForce = Random.Range(6f, 9f);
        upForce = Random.Range(6f, 9f);

        animator = GetComponent<Animator>();
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
        lastRotation = racquet.rotation;
    }

    void Update()
    {
        Rigidbody shuttlecockRb = shuttlecock.GetComponent<Rigidbody>();
        Ball shuttlecockScript = shuttlecock.GetComponent<Ball>();
        float rotationSpeed = Quaternion.Angle(racquet.rotation, lastRotation) / Time.deltaTime;

        // 서브일 때 포물선으로 올려주기
        // 나한테 다가올 때
        //y==racquet.y일 때 t구하고 위치 예측해서
        Debug.Log(rotationSpeed+ " > " + rotationThreshold);
        Debug.Log("isNearBy: "+isNearBy);
        if (objectName == "player")
        {   
            if (rotationSpeed > rotationThreshold && isNearBy) // 엔터를 눌렀을 때
            {
                HitShuttlecock();
                PlayerPrefs.SetInt("whoIsHitting", 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            if (rotationSpeed > rotationThreshold && isNearBy) // 스페이스바를 눌렀을 때
            {
                HitShuttlecock();
                PlayerPrefs.SetInt("whoIsHitting", 2);
                PlayerPrefs.Save();
            }
        }
        int whoIsHitting = PlayerPrefs.GetInt("whoIsHitting", 0);
        if (shuttlecockRb.useGravity)
        {
            if (whoIsHitting == 1 && objectName == "player2")
            {
                Vector3 predictedPosition = CalculateLandingPosition(shuttlecockRb.position, shuttlecockRb.velocity, racquet.position.y);
                predictedPosition.x += 0.8f;
                predictedPosition.z += 1;
                StartCoroutine(MoveToPosition(predictedPosition));
                PlayerPrefs.SetInt("whoIsHitting", 0);
                PlayerPrefs.Save();
            }
            else if (whoIsHitting == 2 && objectName == "player")
            {
                Vector3 predictedPosition = CalculateLandingPosition(shuttlecockRb.position, shuttlecockRb.velocity, racquet.position.y);
                predictedPosition.x -= 0.8f;
                predictedPosition.z -= 1;
                StartCoroutine(MoveToPosition(predictedPosition));
                PlayerPrefs.SetInt("whoIsHitting", 0);
                PlayerPrefs.Save();
            }
        }
        SensorData sensor = SocketManager.instance.sensor[(objectName == "player") ? 0 : 1];
        float rotationAngle = (float)sensor.gyrX;
        rotationAngle = Mathf.Clamp(rotationAngle, -180f, 180f);
        racquet.rotation *= Quaternion.Euler(0, 0, rotationAngle);
    }

    // 셔틀콕의 초기 위치, 초기 속도, 목표 y 위치(라켓의 y 위치)를 받아서 x, z 위치를 반환
    public Vector3 CalculateLandingPosition(Vector3 initialPosition, Vector3 initialVelocity, float targetYPosition)
    {
        float gravity = Physics.gravity.y;

        Vector3 adjustedVelocity = initialVelocity;

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

        // 셔틀콕이 플레이어 방향으로 움직이고 있는지 확인 (90도 이내)
        return angle < 90.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) // Ball 태그를 가진 오브젝트와 떨어졌을 때
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
        shuttlecock.GetComponent<Rigidbody>().useGravity = true;
        // 라켓의 회전을 기반으로 힘의 방향 계산
        Vector3 forwardDirection = transform.forward;
        Vector3 hitDirection = forwardDirection.normalized * hitForce + Vector3.up * upForce;

        // 셔틀콕에 힘 적용
        shuttlecock.GetComponent<Rigidbody>().velocity = hitDirection;
    }

}