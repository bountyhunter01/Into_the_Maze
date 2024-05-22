using UnityEngine;
using UnityEngine.AI;

public class MonsterMove : MonoBehaviour
{
    public float speed = 2.0f; // ������ �ӵ�
    public float wanderRadius = 10f; // ���Ͱ� �̵� ������ �ݰ�
    public float playerDetectionRadius = 15f; // �÷��̾� ���� �ݰ�
    public float changeDirectionInterval = 3f; // ���� ���� ����
    public float wallDetectionDistance = 2f; // �� ���� �Ÿ�
    public float flashlightDetectionRadius = 10f; // ������ �� ���� �ݰ�
    public float stopDistance; //���ߴ¹ݰ�

    public GameObject wallObj;
    private NavMeshAgent agent;
    public Transform player;
    public Light flashlight; // �������� ��
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
        agent.updateRotation = true; // NavMeshAgent�� ȸ���� Ȱ��ȭ
        agent.updatePosition = true;
        InvokeRepeating("SetRandomDestination", 0f, changeDirectionInterval); // ���� ���� ����
                                                                              //rb = GetComponent<Rigidbody>();
                                                                              // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (agent == null || player == null) return;

        // ������ �� ����
        float distanceToFlashlight = Vector3.Distance(transform.position, flashlight.transform.position);
        if (distanceToFlashlight <= flashlightDetectionRadius && flashlight.enabled)
        {
            isFlashlightDetected = true;

        }
        else//�÷��̾ �����Ÿ� �ۿ� ������ ���߰� ��������
        {
            isFlashlightDetected = false;
        }

        // �÷��̾� ����
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
            agent.SetDestination(targetPosition); // ������ �� �Ǵ� �÷��̾�� �̵�
            RotateTowards(targetPosition); // ��ǥ�� ���� ȸ��
        }
        else
        {
            // �� ����
            if (Physics.Raycast(transform.position, agent.velocity.normalized, out RaycastHit hit, wallDetectionDistance))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    SetRandomDestination(); // �� ���� �� ���ο� ������ ����
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
            SetRandomDestination(); // ���� �浹 �� ���ο� ������ ����
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
