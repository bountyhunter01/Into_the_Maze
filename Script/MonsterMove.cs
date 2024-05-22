using UnityEngine;
using UnityEngine.AI;

public class MonsterMove : MonoBehaviour
{
    public float speed = 2.0f; // 몬스터의 속도
    public float wanderRadius = 10f; // 몬스터가 이동 가능한 반경
    public float playerDetectionRadius = 15f; // 플레이어 감지 반경
    public float changeDirectionInterval = 3f; // 방향 변경 간격
    public float wallDetectionDistance = 2f; // 벽 감지 거리
    public float flashlightDetectionRadius = 10f; // 손전등 빛 감지 반경
    public float stopDistance; //멈추는반경

    public GameObject wallObj;
    private NavMeshAgent agent;
    public Transform player;
    public Light flashlight; // 손전등의 빛
    private bool isPlayerDetected = false;
    private bool isFlashlightDetected = false;
    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        flashlight = GameObject.FindGameObjectWithTag("Flashlight")?.GetComponent<Light>();
        if (flashlight == null)
        {
            Debug.LogError("Flashlight not found or Light component is missing");
        }
        agent.updateRotation = true; // NavMeshAgent의 회전을 활성화
        agent.updatePosition = true;
        InvokeRepeating("SetRandomDestination", 0f, changeDirectionInterval); // 방향 변경 간격
                                                                              //rb = GetComponent<Rigidbody>();
                                                                              // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (agent == null || player == null) return;

        // 손전등 빛 감지
        float distanceToFlashlight = Vector3.Distance(transform.position, flashlight.transform.position);
        if (distanceToFlashlight <= flashlightDetectionRadius && flashlight.enabled)
        {
            isFlashlightDetected = true;

        }
        else//플레이어가 추적거리 밖에 있으면 멈추게 구현하자
        {
            isFlashlightDetected = false;
        }

        // 플레이어 감지
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= playerDetectionRadius)
        {
            isPlayerDetected = true;

        }
        else
        {
            isPlayerDetected = false;
        }

        if (isFlashlightDetected || isPlayerDetected)
        {
            Vector3 targetPosition = isFlashlightDetected ? flashlight.transform.position : player.position;
            agent.SetDestination(targetPosition); // 손전등 빛 또는 플레이어에게 이동
            RotateTowards(targetPosition); // 목표를 향해 회전
        }
        else
        {
            // 벽 감지
            if (Physics.Raycast(transform.position, agent.velocity.normalized, out RaycastHit hit, wallDetectionDistance))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    SetRandomDestination(); // 벽 감지 시 새로운 목적지 설정
                }
            }
        }
       
    }

    void SetRandomDestination()
    {
        if (agent == null || isPlayerDetected || isFlashlightDetected) return;

        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            //agent.pathStatus= ;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            SetRandomDestination(); // 벽과 충돌 시 새로운 목적지 설정
        }
        if (collision.gameObject.tag =="Player")
        {
            agent.isStopped = true;
        }else
        {
            agent.isStopped = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // footstepSource.Stop();
        }
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
    }
}
