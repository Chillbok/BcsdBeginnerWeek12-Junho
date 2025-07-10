using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [Header("돌 스테이터스")]
    [SerializeField] private int hp; //바위의 체력
    [SerializeField] private float destroyTime; //파편 제거 시간
    [SerializeField] private SphereCollider col; //구체 콜라이더

    [Header("게임 오브젝트")]
    //필요한 게임 오브젝트
    [SerializeField] private GameObject go_rock; //일반 바위
    [SerializeField] private GameObject go_debris; //깨진 바위
    [SerializeField] private GameObject go_effectPrefabs; //채굴 이펙트

    [Header("사운드 이펙트")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip effect_sound; //곡괭이 내려칠 때 사운드 이펙트
    [SerializeField] AudioClip effect_sound2; //돌 부서질 때 사운드 이펙트

    public void Mining()
    {
        audioSource.clip = effect_sound;
        audioSource.Play();
        var clone = Instantiate(go_effectPrefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);
        hp--; //돌 hp 감소
        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        audioSource.clip = effect_sound2; //돌 부서지면 나는 소리
        audioSource.Play();
        col.enabled = false; //기존 바위 비활성화
        Destroy(go_rock); //일반 바위 삭제
        go_rock.SetActive(false);

        go_debris.SetActive(true); //깨진 바위 활성화
        Destroy(go_debris, destroyTime); //destroyTime만큼 시간 지나면 깨진 바위도 삭제
    }
}
