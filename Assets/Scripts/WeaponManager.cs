using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //무기 중복 교체 실행 방지
    public static bool isChangeWeapon = false; //공유 자원. 다른 스크립트에서 해당 변수를 수정하면 동일하게 적용됨. 굳이 클래스 선언을 하지 않아도 다른 스크립트에서 불러와서 사용 가능.


    //현재 무기의 애니메이션
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim; //애니메이터 감독


    //현재 무기의 타입
    [SerializeField] private string currentWeaponType;


    //무기 교체 딜레이
    [SerializeField] private float changeWeaponDelayTime;
    [SerializeField] private float changeWeaponEndDelayTime;


    //무기 종류들 전부 관리
    [SerializeField] private Gun[] guns;
    [SerializeField] private CloseWeapon[] hands;


    //관리 차원에서 쉽게 무기 접근이 가능하도록 만듦.
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();


    [SerializeField] GunController theGunController;
    [SerializeField] HandController theHandController;


    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
        for (int i = 0; i < hands.Length; i++)
        {
            handDictionary.Add(hands[i].closeWeaponName, hands[i]);
        }
    }


    void Update()
    {
        if (!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) //숫자키 1 눌러서
            {
                //무기 교체 실행(맨손)
                Debug.Log("숫자키 1 누름");
                StartCoroutine(ChangeWeaponCoroutine("HAND", "맨손"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) //숫자키 2 눌러서
            {
                //무기 교체 실행(서브머신건)
                Debug.Log("숫자키 2 누름");
                StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMacnineGun1"));
            }
        }
    }

    //무기 바꾸는 코루틴
    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true; //무기 바꾸는 중
        currentWeaponAnim.SetTrigger("Weapon_Out"); //무기 꺼내는 애니메이션 실행


        yield return new WaitForSeconds(changeWeaponDelayTime); //무기 꺼내는 동안 기다리기


        CancelPreWeaponAction(); //무기 관련 행동 멈추기
        WeaponChange(_type, _name); //무기 바꾸기
        currentWeaponType = _type; //바꾼 무기에 맞게 무기 타입 바꿔주기


        yield return new WaitForSeconds(changeWeaponDelayTime); //무기 종료 대기 시간


        //무기 교체 종료
        isChangeWeapon = false; //무기 바꿈 종료
    }

    private void CancelPreWeaponAction() //무기 관련 행동 멈추기
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSight(); //정조준 상태 해제
                theGunController.CancelReload(); //재장전 멈추기
                GunController.isActivate = false;
                break;
            case "HAND":
                HandController.isActivate = false;
                break;
        }
    }

    //무기 교체 함수
    private void WeaponChange(string _type, string _name)
    {
        if (_type == "GUN")
        {
            theGunController.GunChange(gunDictionary[_name]);
        }
        else if (_type == "HAND")
        {
            theHandController.CloseWeaponChange(handDictionary[_name]);
        }
    }
}
