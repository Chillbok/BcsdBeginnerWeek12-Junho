using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    //기존 위치
    private Vector3 originPos;

    //현재 위치
    private Vector3 currentPos;

    //sway 한계
    [SerializeField] private Vector3 limitPos;

    //정조준 sway
    [SerializeField] private Vector3 fineSightLimitPos;

    //부드러운 움직임 정도
    [SerializeField] private Vector3 smoothSway;

    //필요한 컴포넌트
    [SerializeField] private GunController theGunController;

    void Start()
    {
        originPos = this.transform.localPosition; //스크립트 붙어있는 게임 오브젝트의 로컬 포지션
    }

    void Update()
    {
        TrySway();
    }

    private void TrySway()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0) //마우스 인풋 값이 있을 때
        {
            Swaying();
        }
        else //마우스 인풋 값이 없을 때
            BackToOriginPos();
    }

    private void Swaying()
    {
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");

        if (theGunController.isFineSightMode) //정조준 상태가 아닐 때 무기 흔들림 구현
        {
            currentPos.Set(
                Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x),
                Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -limitPos.y, limitPos.y),
                originPos.z
            );
        }
        else //정조준 상태일 때 무기 흔들림 구현
        {
            currentPos.Set(
                Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -fineSightLimitPos.x, fineSightLimitPos.x),
                Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                originPos.z
            );
        }
        transform.localPosition = currentPos; //계산된 흔들림 위치를 현재 위치에 대입

    }

    private void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos; //계산된 흔들림 위치를 현재 위치에 대입
    }
}
