using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private Gun currentGun;

    private float currentFireRate; //현재 연사 속도. 1초에 1씩 감소하고 0이 되면 발사

    private bool isReload = false; //재장전 중인가?

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GunFireRateCalc();
        TryFire();
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
                StartCoroutine(ReloadCoroutine()); //재장전 코루틴 실행
        }
    }

    private void Shoot() //진짜 총알 쏨
    {
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; //연사 속도 재계산
        currentGun.muzzleFlash.Play(); //총구 화염 효과 재생
        PlaySE(currentGun.fire_Sound); //발사 소리 출력
        Debug.Log("총알 발사");
    }

    IEnumerator ReloadCoroutine() //재장전 코루틴
    {
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true; //재장전 할 때 총 못 쏘게 하기 위함
            currentGun.anim.SetTrigger("Reload"); //재장전 애니메이션 활성화

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
    }

    private void PlaySE(AudioClip _clip) //특정 오디오클립 재생시키는 메소드
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
