using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private int hp; //바위의 체력
    [SerializeField] private float destroyTime; //파편 제거 시간
    [SerializeField] private SphereCollider col; //구체 콜라이더

    //필요한 게임 오브젝트
    [SerializeField] private GameObject go_rock; //일반 바위
    [SerializeField] private GameObject go_debris; //깨진 바위

    public void Mining()
    {
        Debug.Log("mining 작동");
        hp--;
        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        col.enabled = false; //기존 바위 비활성화
        Destroy(go_rock); //일반 바위 삭제
        Debug.Log("돌 삭제");
        go_rock.SetActive(false);

        go_debris.SetActive(true); //깨진 바위 활성화
        Debug.Log("부서진 돌 활성화");
        Destroy(go_debris, destroyTime); //destroyTime만큼 시간 지나면 깨진 바위도 삭제
    }
}
