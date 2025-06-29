using UnityEngine;

public class HandController : MonoBehaviour
{
    //현재 장착된 Hand형 타입 무기.
    [SerializeField] private Hand currentHand;

    //공격중
    private bool isAttack = false; //공격중이라면 true
    private bool isSwing = false; //팔을 휘두르고 있는 중이라면 true

    private RaycastHit hitInfo;

    void Start()
    {

    }

    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {

        }
    }
}
