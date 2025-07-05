using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //스피드 조정 변수
    [SerializeField] private float walkSpeed; //걷는 속도
    [SerializeField] private float runSpeed; //뛰는 속도
    [SerializeField] private float crouchSpeed; //앉았을 때 속도
    private float applySpeed; //실제로 속도로 적용되는 변수

    [SerializeField] private float jumpForce; //점프 힘수

    //상태 변수
    private bool isWalk = false; //걷는 중인가?
    private bool isRun = false; //뛰는 중인가?
    private bool isCrouch = false; //앉아있는가?
    private bool isGround = true; //땅에 있는가?

    //앉았을 때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY; //카메라 위치
    private float applyCrouchPosY;

    //땅 착지 여부 확인을 위한 컴포넌트
    private CapsuleCollider capsuleCollider;

    //민감도
    [SerializeField] private float lookSensitivity; //마우스 시야 민감도

    //카메라 한계
    [SerializeField] private float cameraRotationLimit; //화면 위로 올렸을 때 최대 시야각
    private float currentCameraRotationX = 0f; //카메라 x축 회전

    //필요한 컴포넌트
    [SerializeField] private Camera theCamera;
    private Rigidbody myRigid;
    [SerializeField] GunController theGunController;
    [SerializeField] private Crosshair theCrosshair;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();

        //초기화
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y; //상위 오브젝트 기준 위치
        applyCrouchPosY = originPosY; //적용할 카메라 높이 초기화
    }

    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        MoveCheck();
        CameraRotation();
        CharacterRotation();
    }

    //앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
}

    //앉기 동작
    private void Crouch()
    {
        //메서드가 실행될 때마다 반전시키기
        //true면 false로, false면 true로
        isCrouch = !isCrouch;
        theCrosshair.CrouchingAnimation(isCrouch);

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    //앉기 시 자연스러운 카메라 이동을 구현하기 위한 코루틴
    //부드러운 앉기 동작 실행
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            //15번 실행하면 그냥 반복문 끝내기
            if (count > 15)
                break;
            yield return null;
        }
        //카메라 높이 조절하기
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0);
    }

    //지면 체크
    private void IsGround()
    {
        //아래 방향으로 콜라이더 크기보다 0.1f만큼 더 길게 Ray 발사, collider와 닿으면 true 반환
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        if (isGround) //땅에 서 있다면
            theCrosshair.RunningAnimation(false);
    }

    //점프 시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    //점프
    private void Jump()
    {
        //앉은 상태에서 점프하면 서 있는 상태로 변경
        if (isCrouch)
            Crouch();
        myRigid.linearVelocity = transform.up * jumpForce;
    }

    //달리기 시도
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    //달리기 실행
    private void Running()
    {
        //앉은 상태에서 달리기 시작하면 서 있는 상태로 변경
        if (isCrouch)
            Crouch();

        theGunController.CancelFineSight(); //뛰기 시작하면 조준 해제

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = runSpeed;
    }

    //달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }

    //움직임 실행
    private void Move()
    {
        //인풋값 변수 생성
        float _moveDirX = Input.GetAxisRaw("Horizontal"); //왼쪽 오른쪽 움직임
        float _moveDirZ = Input.GetAxisRaw("Vertical"); //앞뒤 움직임

        //방향 벡터 생성
        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        //두개 합치고, 이동 속도 곱하기
        //normailze로 정규화
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        //캐릭터 위치 변화
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    //상하 카메라 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y"); //마우스 위아래 움직임
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX; //카메라 움직이기
        //카메라를 -cameraRotationLimit, cameraRotationLimit 사이 각도에서만 움직일 수 있도록 설정
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        //카메라에 적용
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    //
    private void MoveCheck()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); //왼쪽 오른쪽 움직임
        float _moveDirZ = Input.GetAxisRaw("Vertical"); //앞뒤 움직임

        if (isGround == false) //달리기 중이거나, 땅에 닿지 않은 상태라면
            theCrosshair.RunningAnimation(true);

        if (!isRun && !isCrouch && isGround)
        {
            if (_moveDirX == 0 && _moveDirZ == 0) //전 프레임 마지막 위치와 현재 위치가 0.01f 이상이라면
            {
                isWalk = false;
            }
            else
            {
                isWalk = true;
            }
            theCrosshair.WalkingAnimation(isWalk);
        }
    }

    //좌우 캐릭터 회전
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        //Debug.Log(myRigid.rotation);
        //Debug.Log(myRigid.rotation.eulerAngles);
    }
}
