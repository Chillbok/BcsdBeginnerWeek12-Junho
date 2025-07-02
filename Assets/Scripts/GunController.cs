using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GunController : MonoBehaviour
{
    [SerializeField] private Gun currentGun;

    private float currentFireRate; //현재 연사 속도. 1초에 1씩 감소하고 0이 되면 발사
    private bool isReload = false; //재장전 중인가?
    private bool isFineSightMode = false; //정조준 여부

    [SerializeField] private Vector3 originPos; //본래 포지션 값

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GunFireRateCalc(); //발사 속도 계산
        TryFire(); //탄창에 총알 있으면 사격, 총알 없으면 재장전
        TryReload(); //직접 재장전 시도
        TryFineSight(); //정조준 여부 확랴ㅜ
    }

    private void GunFireRateCalc() //발사 속도 계산용
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 대략 60분의 1 = 1
    }

    private void TryFire() //발사 시도
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }

    private void Fire() //발사 과정
    {
        if (!isReload) //재장전 중이 아니라면
        {
            if (currentGun.currentBulletCount > 0) //탄창 총알 개수가 0보다 많으면
                Shoot(); //총알 발사
            else //탄창에 총알이 없으면
            {
                CancelFineSight(); //조준 취소
                StartCoroutine(ReloadCoroutine()); //재장전 코루틴 실행
            }
        }
    }

    private void Shoot() //진짜 총알 쏨
    {
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; //연사 속도 재계산
        currentGun.muzzleFlash.Play(); //총구 화염 효과 재생
        PlaySE(currentGun.fire_Sound); //발사 소리 출력

        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
        Debug.Log("총알 발사");
    }

    private void TryReload() //탄창이 안 비어도 직접 재장전
    {
        if (Input.GetKeyDown(KeyCode.R) /*R 눌렀을 때*/ && !isReload /*재장전 중이 아닐 때*/ && currentGun.currentBulletCount < currentGun.reloadBulletCount /*현재 총알 개수가 탄창 크기보다 작을 때*/)
        {
            CancelFineSight(); //조준 상태 풀기
            StartCoroutine(ReloadCoroutine()); //재장전 시작
        }
    }

    IEnumerator ReloadCoroutine() //재장전 코루틴
    {
        if (currentGun.carryBulletCount > 0) //소유한 총알 있음
        {
            isReload = true; //재장전 할 때 총 못 쏘게 하기 위함
            currentGun.anim.SetTrigger("Reload"); //재장전 애니메이션 활성화

            currentGun.carryBulletCount += currentGun.currentBulletCount; //현재 소유한 총알 개수와 탄창 총알 개수 더하기
            currentGun.currentBulletCount = 0; //탄창 총알 비우기

            yield return new WaitForSeconds(currentGun.reloadTime); //재장전 시간동안 기다리기

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount) //탄창 크기보다 보유 총알 개수가 많으면
            {
                //탄창의 잔탄은 그냥 버려짐
                currentGun.currentBulletCount = currentGun.reloadBulletCount; //탄창에 총알 채우기
                currentGun.carryBulletCount -= currentGun.reloadBulletCount; //보유 총알 개수만큼 빠지기
            }
            else //탄창 크기보다 보유 총알 개수가 적으면
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount; //남은 총알 전부 탄창에 넣기
                currentGun.carryBulletCount = 0; //남은 총알 개수는 0
            }

            isReload = false; //재장전 끝남
        }
        else //총알이 없음
        {
            Debug.Log("소유한 총알이 없습니다.");
        }
    }

    private void TryFineSight() //정조준 시도
    {
        if (Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight(); //정조준 실행
        }
    }

    public void CancelFineSight() //조준 취소
    {
        if (isFineSightMode) //조준 중이면
            FineSight();
    }

    private void FineSight()
    {
        isFineSightMode = !isFineSightMode; //정조준 활성화
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        if (isFineSightMode) //정조준 중일 때
        {
            StopAllCoroutines(); //기존 실행중인 모든 코루틴 멈춤
            StartCoroutine(FineSightActivateCoroutine()); //정조준 시작
        }
        else //정조준 아닐 때
        {
            StopAllCoroutines(); //기존 실행중인 모든 코루틴 멈춤
            StartCoroutine(FineSightDeactivateCoroutine()); //비조준 시작
        }
    }

    IEnumerator FineSightActivateCoroutine() //정조준 작동용 코루틴
    {
        while (currentGun.transform.localPosition != currentGun.fineSightOriginPos) //정조준 자세가 될 때까지 무한반복
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator FineSightDeactivateCoroutine() //비조준 작동용 코루틴
    {
        while (currentGun.transform.localPosition != originPos) //비조준 자세가 될 때까지 무한반복
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator RetroActionCoroutine() //반동 구현 코루틴
    {
        //정조준 안했을 때 최대 반동
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z); //나머지 값은 그대로, X만 수정해서 반동 구현(앞뒤로 움직임)
        //정조준 했을 때 최대 반동
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z); //마찬가지로 X만 이동, Y, Z는 그대로 유지

        if (!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos; //총 위치를 원래대로 돌림

            //반동 시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f) //계속 반복, 단, 총 현재 위치가 총의 retroActionForce과 대충 일치하면 종료.
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            //원위치
            while (currentGun.transform.localPosition != originPos) //될때까지 반복
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos; //총 위치를 원래대로 돌림

            //반동 시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f) //계속 반복, 단, 총 현재 위치가 총의 retroActionForce과 대충 일치하면 종료.
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            //원위치
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos) //될때까지 반복
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    private void PlaySE(AudioClip _clip) //특정 오디오클립 재생시키는 메소드
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
