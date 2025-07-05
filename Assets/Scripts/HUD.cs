using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HUD : MonoBehaviour
{
    //필요한 컴포넌트
    [SerializeField] private GunController theGunController;
    private Gun currentGun;

    //필요하면 HUD 호출, 필요없으면 HUD 비활성화
    [SerializeField] private GameObject go_BulletHUD;

    //총알 개수 텍스트에 반영
    [SerializeField] private TextMeshProUGUI[] text_bullet;

    void Update()
    {
        CheckBullet();
    }

    private void CheckBullet()
    {
        currentGun = theGunController.GetGun();
        text_bullet[0].text = currentGun.carryBulletCount.ToString();
        text_bullet[1].text = currentGun.reloadBulletCount.ToString();
        text_bullet[2].text = currentGun.currentBulletCount.ToString();
    }
}