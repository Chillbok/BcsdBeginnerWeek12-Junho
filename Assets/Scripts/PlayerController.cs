using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed; //걷는 속도
    [SerializeField] private float lookSensitivity; //마우스 시야 민감도
    [SerializeField] private float cameraRotationLimit; //화면 위로 올렸을 때 최대 시야각
    private float currentCameraRotationX = 0f; //카메라 x축 회전

    [SerializeField] private Camera theCamera;
    private Rigidbody myRigid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //플레이어 오브젝트의 하위 자식 중 카메라는 하나밖에 없을 것이므로, 그냥 이렇게 사용하자
        theCamera = GetComponentInChildren<Camera>();
        myRigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CameraRotation();
        CharacterRotation();
    }

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
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

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

    //좌우 캐릭터 회전
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        Debug.Log(myRigid.rotation);
        Debug.Log(myRigid.rotation.eulerAngles);
    }
}
