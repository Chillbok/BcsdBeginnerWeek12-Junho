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

    [Header("필요한 사운드")]
    //필요한 사운드 이름
    [SerializeField] private string strike_Sound;
    [SerializeField] private string destroy_Sound;

    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);

        var clone = Instantiate(go_effectPrefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);
        hp--; //돌 hp 감소
        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        SoundManager.instance.PlaySE(destroy_Sound);
        col.enabled = false; //기존 바위 비활성화
        Destroy(go_rock); //일반 바위 삭제
        go_rock.SetActive(false);

        go_debris.SetActive(true); //깨진 바위 활성화
        Destroy(go_debris, destroyTime); //destroyTime만큼 시간 지나면 깨진 바위도 삭제
    }
}
