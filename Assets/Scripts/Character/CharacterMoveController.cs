using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMoveController : MonoBehaviour
{
    public bool isMoved = true;                  // 이동 가능 여부
    public CinemachineVirtualCamera virtualCamera;
    private CharacterAnimationController animationController;

    private float moveSpeed;                    // 이동 속도
    private float addRunSpeed;                  // 달릴 때 추가 이동 속도
    private Vector3 dir;                        // 현재 방향
    private Vector3 normalizedCameraForward;    // 카메라 forward 정규화 캐싱
    private Vector3 normalizedCameraRight;      // 카메라 right 정규화 캐싱
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Vector2 movement = Vector2.zero;    // 이동 인풋 값 (상하좌우)

    private bool isRunning = false;             // 달리는 중인지 확인용
    private Vector2 runVec = Vector2.zero;      // 달리기 애니메이션 출력용
    private float sensitvity = 8.5f;            // 달리기 애니메이션 전환 속도

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        moveSpeed = GetComponent<ChampBase>().data.moveSpeed;
        animationController = GetComponent<CharacterAnimationController>();

        if (virtualCamera == null)
            return;

        // 카메라 forward, right 정규화 캐싱
        // 카메라의 forward와 right를 사용하여 방향 조정
        normalizedCameraForward = virtualCamera.transform.forward;
        normalizedCameraRight = virtualCamera.transform.right;

        normalizedCameraForward.y = 0;
        normalizedCameraRight.y = 0;
        normalizedCameraForward.Normalize();
        normalizedCameraRight.Normalize();
    }

    public NavMeshAgent GetAgent() { return agent; }
    public Rigidbody GetRigidbody() { return rb; }

    // 이동
    private void Move()
    {
        if (!isMoved)   // 이동 비활성화 시 return
            return;

        movement.x = Input.GetAxis("Horizontal");   // 인풋 x값
        movement.y = Input.GetAxis("Vertical");     // 인풋 y값
        Run();                                      // 달리기 체크, 달리기 실행

        Vector3 moveDirection = (movement.y * normalizedCameraForward + movement.x * normalizedCameraRight).normalized; // 이동 방향
        dir = moveDirection * (moveSpeed + addRunSpeed);    // 속도 값을 적용하여 이동 방향 계산
        if (moveDirection != Vector3.zero)                  // 이동 방향이 0,0,0이 아닐 시
        {
            rb.transform.rotation = Quaternion.LookRotation(moveDirection); // 해당 방향 바라보기
        }

        transform.position += dir * Time.deltaTime; // 이동
    }

    public void FixedUpdate()
    {
        Move();
    }

    public void Update()
    {
        animationController.UpdateMoveAnimation(movement + runVec);
    }

    private void Run()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift);            // 이동 키 입력 감지
        if (isRunning)                                          // 키 입력이 있을 시
        {
            addRunSpeed = moveSpeed * 0.5f;                     // 이동 속도의 절반 값을 달리기 속도로 할당
            runVec += movement * (sensitvity * Time.deltaTime); // 인풋 값, 애니메이션 전환 속도를 사용하여 이동 벡터 값 계산
            runVec.x = Mathf.Clamp(runVec.x, -1, 1);            // 이동 벡터의 x가 -1이나 1을 넘지 않게 조정
            runVec.y = Mathf.Clamp(runVec.y, -1, 1);            // 이동 벡터의 y가 -1이나 1을 넘지 않게 조정
        }
        else                                                    // 키 입력이 없을 시
        {
            addRunSpeed = 0;                                    // 달리기 속도 초기화
            runVec = Vector2.zero;                              // 이동 벡터 초기화
        }
    }

    // 이동 중지
    public void StopMove()
    {
        isMoved = false;                // 이동 불가능
        movement = Vector2.zero;        // 인풋값 초기화
        runVec = Vector2.zero;          // 달리기 벡터 초기화
        dir = Vector3.zero;             // 방향 초기화
        rb.velocity = Vector3.zero;     // RigidBody 속도 초기화
        agent.isStopped = true;         // agent 멈추기
    }
}
