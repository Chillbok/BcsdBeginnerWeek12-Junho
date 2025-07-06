using System;
using UnityEngine;

public class CloseWeapon : MonoBehaviour
{
    public string closeWeaponName; //근접 무기 이름

    //웨폰 유형
    public bool isHand; //맨손인가?
    public bool isAxe; //도끼인가?
    public bool isPickaxe; //곡괭이인가?

    public float range; //공격 범위
    public int damage; //공격력
    public float workSpeed; //작업 속도
    public float attackDelay; //공격 딜레이
    public float attackDelayA; //공격 활성화 시점
    public float attackDelayB; //공격 비활성화 시점

    public Animator anim; //애니메이터 컨트롤러
}
