using System.Collections;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour
//미완성 클래스 = 추상 클래스
{
    //활성화 여부
    public static bool isActivate = false;

    //현재 장착된 Hand형 타입 무기.
    [SerializeField] protected CloseWeapon currentHand;

    //공격중
    protected bool isAttack = false; //공격중이라면 true
    protected bool isSwing = false; //팔을 휘두르고 있는 중이라면 true

    protected RaycastHit hitInfo;

    protected void Update()
    {
        if (isActivate)
        {
            TryAttack();
        }
    }

    protected void TryAttack()
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

    protected IEnumerator AttackCoroutine() //공격 관련 시간 조절 코루틴
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

    //미완성. 자식클래스가 완성해야 함. = 추상 코루틴
    protected abstract IEnumerator HitCoroutine(); //무언가가 Ray에 맞았는지 맞지 않았는지 판단하는 코루틴

    protected bool CheckObject() //Ray를 이용해서 전방의 적 확인
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
        {
            return true; //어떤 대상이 충돌했다면
        }
        return false; //어떤 대상도 충돌하지 않았다면
    }

    public void HandChange(CloseWeapon _hand)
    {
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false);


        currentHand = _hand;
        WeaponManager.currentWeapon = currentHand.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentHand.anim;


        currentHand.transform.localPosition = Vector3.zero; //조준한 상태에서 총 바꾸면 좌표 바뀔 수 있음. 방지용.
        currentHand.gameObject.SetActive(true);
        isActivate = true;
    }
}