using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform aimTarget;
    float speed = 3f;
    float hitForce = 15f;
    float upForce = 10f;
    bool isHitting;
    public Transform shuttlecock;
    Animator animator;
    Vector3 aimTargetInitialPosition;
    public GameObject playerInArea;
    public float minDistanceToHit = 0f;

    // 예상 위치에 도달할 시간 (초)
    public float predictionTime = 2f;

    bool isNearBy = false;
    string objectName;

    private void Start()
    {
        animator = GetComponent<Animator>();
        aimTargetInitialPosition = aimTarget.position;
        objectName = gameObject.name;
    }

    void Update()
    {
        Rigidbody shuttlecockRb = shuttlecock.GetComponent<Rigidbody>();
        // 셔틀콕이 움직이지 않을 때, 셔틀콕의 현재 위치로 이동
        if (shuttlecockRb.velocity.magnitude <= 0.001f)
        {
            if(IsShuttlecockInPlayerArea()){
                MoveTowardsShuttlecock(shuttlecock.position);
            }
        }
        else
        {
            if (IsShuttlecockApproaching(shuttlecockRb))
            {
                Vector3 predictedPosition = PredictShuttlecockLanding();
                MoveTowardsPredictedPosition(predictedPosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            isHitting = true;
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            isHitting = false;
        }

        if (isHitting)
        {
            aimTarget.Translate(Vector3.right * speed * 2 * Time.deltaTime);
        }

        if (objectName == "player"){
            if (Input.GetKeyDown(KeyCode.Return) && isNearBy) // 스페이스바를 눌렀을 때
            {
                HitShuttlecock();
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Space) && isNearBy) // 스페이스바를 눌렀을 때
            {
                HitShuttlecock();
            }
        }
        
    }

    bool IsShuttlecockInPlayerArea()
    {
        BoxCollider playerAreaCollider = playerInArea.GetComponent<BoxCollider>();
        Bounds playerAreaBounds = playerAreaCollider.bounds;
        Vector3 shuttlecockPos = shuttlecock.position;

        // PlayerArea의 x와 z 축 범위 내에 있는지 확인
        bool isInXBounds = shuttlecockPos.x >= playerAreaBounds.min.x && shuttlecockPos.x <= playerAreaBounds.max.x;
        bool isInZBounds = shuttlecockPos.z >= playerAreaBounds.min.z && shuttlecockPos.z <= playerAreaBounds.max.z;

        return isInXBounds && isInZBounds;
    }



    bool IsShuttlecockApproaching(Rigidbody shuttlecockRb)
    {
        Vector3 directionToPlayer = transform.position - shuttlecock.position;
        float angle = Vector3.Angle(shuttlecockRb.velocity, directionToPlayer);

        // 셔틀콕이 플레이어 방향으로 움직이고 있는지 확인 (예: 90도 이내)
        return angle < 90.0f;
    }

    Vector3 PredictShuttlecockLanding()
    {
        Rigidbody shuttlecockRb = shuttlecock.GetComponent<Rigidbody>();
        Vector3 initialVelocity = shuttlecockRb.velocity;
        float time = predictionTime;
        Vector3 gravity = Physics.gravity * shuttlecockRb.mass;

        // 중력의 영향을 고려한 위치 계산
        Vector3 position = shuttlecock.position + initialVelocity * time + 0.5f * gravity * time * time;

        // 드래그를 고려하여 속도 감소
        float dragEffect = 1f / (1f + shuttlecockRb.drag * time);

        position *= dragEffect;

        position.y = 0; // Y축 값은 무시

        if (objectName == "player"){
            position.z += 3;
        } else {
            position.z -= 6;
        }
        
        return position;
    }

    void MoveTowardsPredictedPosition(Vector3 predictedPosition)
    {
        Vector3 direction = predictedPosition - transform.position;
        direction.y = 0; // Y축 이동 제외

        // 셔틀콕에 충분히 가까워질 때까지 이동
        if (direction.magnitude > minDistanceToHit)
        {
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        }
    }

    void MoveTowardsShuttlecock(Vector3 shuttlecockPosition)
    {
        Vector3 direction = shuttlecockPosition - transform.position;
        if (direction.magnitude > minDistanceToHit)
        {
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        }
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
        Vector3 dir = aimTarget.position - transform.position; // get the direction to where we want to send the shuttlecock
        Vector3 force = dir.normalized * hitForce + Vector3.up * upForce; // adding upward force for a parabolic trajectory

        shuttlecock.GetComponent<Rigidbody>().velocity = force;

        Vector3 shuttlecockDir = shuttlecock.position - transform.position;
        if (shuttlecockDir.x >= 0)
        {
            animator.Play("forehand");
        }
        else
        {
            animator.Play("backhand");
        }

        aimTarget.position = aimTargetInitialPosition; // reset the position of the aiming gameObject to its original position
    }
}