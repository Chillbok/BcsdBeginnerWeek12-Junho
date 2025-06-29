using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private Gun currentGun;

    private float currentFireRate; //현재 연사 속도. 1초에 1씩 감소하고 0이 되면 발사


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
        if (Input.GetButton("Fire1") && currentFireRate <= 0)
        {
            Fire();
        }
    }

    private void Fire() //발사 과정
    {
        currentFireRate = currentGun.fireRate;
        Shoot(); //총알 발사
    }

    private void Shoot() //진짜 총알 쏨
    {
        currentGun.muzzleFlash.Play(); //총구 화염 효과 재생
        PlaySE(currentGun.fire_Sound); //발사 소리 출력
        Debug.Log("총알 발사");
    }

    private void PlaySE(AudioClip _clip) //특정 오디오클립 재생시키는 메소드
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
