using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : CloseWeaponController
{
    //활성화 여부
    public static bool isActivate = false;

    void Start()
    {
    }

    void Update()
    {
        if (isActivate)
            TryAttack();
    }

    protected override IEnumerator HitCoroutine()
    {
        Debug.Log("돌에 곡괭이 맞음");
        while (isSwing)
        {
            if (CheckObject()) //충돌됨
            {
                if (hitInfo.transform.tag == "Rock") //충돌체의 태그가 Rock라면
                {
                    hitInfo.transform.GetComponent<Rock>().Mining();
                }
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}
