using System.Collections;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //현재 장착된 Hand형 타입 무기.
    [SerializeField] private Hand currentHand;

    //공격중
    private bool isAttack = false; //공격중이라면 true
    private bool isSwing = false; //팔을 휘두르고 있는 중이라면 true

    private RaycastHit hitInfo;

    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!isAttack)
            {
                //코루틴 실행
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    IEnumerator AttackCoroutine() //공격 관련 시간 조절 코루틴
    {
        isAttack = true; //공격 시작
        currentHand.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        StartCoroutine(HitCoroutine());

        //공격 활성화 시점

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false; //공격 종료
    }

    IEnumerator HitCoroutine() //무언가가 Ray에 맞았는지 맞지 않았는지 판단하는 코루틴
    {
        while (isSwing)
        {
            if (CheckObject()) //충돌됨
            {
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    private bool CheckObject() //Ray를 이용해서 전방의 적 확인
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
        {
            return true; //어떤 대상이 충돌했다면
        }
        return false; //어떤 대상도 충돌하지 않았다면
    }
}
