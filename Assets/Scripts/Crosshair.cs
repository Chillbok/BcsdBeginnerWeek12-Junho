using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Animator animator;

    //크로스헤어 상태에 따른 총의 정확도
    private float gunAccuracy;

    //크로스헤어 비활성화를 위한 부모 객체.
    [SerializeField] GameObject go_CrosshairHUD;
    [SerializeField] private GunController theGunController;

    public void WalkingAnimation(bool _flag)
    {
        WeaponManager.currentWeaponAnim.SetBool("Walk", _flag); //무기
        animator.SetBool("Walking", _flag); //크로스헤어
    }

    public void RunningAnimation(bool _flag)
    {
        WeaponManager.currentWeaponAnim.SetBool("Run", _flag); //무기
        animator.SetBool("Running", _flag); //크로스헤어
    }

    public void JumpingAnimation(bool _flag)
    {
        WeaponManager.currentWeaponAnim.SetBool("Run", _flag); //무기
        animator.SetBool("Running", _flag); //크로스헤어
    }

    public void CrouchingAnimation(bool _flag)
    {
        animator.SetBool("Crouching", _flag); //크로스헤어
    }

    public void FineSightAnimation(bool _flag)
    {
        animator.SetBool("FineSight", _flag); //크로스헤어
    }

    public void FireAnimation()
    {
        if (animator.GetBool("Walking"))
            animator.SetTrigger("Walk_Fire");
        else if (animator.GetBool("Crouching"))
            animator.SetTrigger("Crouch_Fire");
        else
            animator.SetTrigger("Idle_Fire");
    }

    public float GetAccuracy()
    {
        if (animator.GetBool("Crouching")) //앉아 있을 때
            gunAccuracy = 0.015f;
        else if (animator.GetBool("Running")) //달리는 중일 때
            gunAccuracy = 1f;
        else if (theGunController.GetFineSightMode()) //조준 중일 때
            gunAccuracy = 0.001f;
        else if (animator.GetBool("Walking")) //걸을 때
            gunAccuracy = 0.06f;
        else //가만히 서 있을 때
            gunAccuracy = 0.035f;

        return gunAccuracy;
    }
}
