using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public struct Input
    {
        public Quaternion targetRotation;
        public Vector3 moveDirection;

        public bool isShooting;
        public bool isThrowingWeapon;
        public bool isAiming;

        public Input(Quaternion inTargetRotation, Vector3 inMoveDirection, bool inIsShooting, bool inIsThrowingWeapon, bool inIsAiming)
        {
            targetRotation      = inTargetRotation;
            moveDirection       = inMoveDirection;
            isShooting          = inIsShooting;
            isThrowingWeapon    = inIsThrowingWeapon;
            isAiming            = inIsAiming;
        }
    }
    public Input input;

    [System.Serializable]
    public struct Stats
    {
        public float health;
        public float moveSpeed;
        public float turnSpeed;
        public float throwStrength;
    }
    [SerializeField] Stats _stats;

    [SerializeField] GameObject _weaponsHolder;


    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Movement
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(input.moveDirection.normalized * _stats.moveSpeed);
        }

        // Aiming
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, input.targetRotation, _stats.turnSpeed * Time.deltaTime);
        }

        // Shooting
        {
            if (input.isShooting)
                for (int i = 0; i < _weaponsHolder.transform.childCount; i++)
                    _weaponsHolder.transform.GetChild(i).gameObject.GetComponent<Weapon>().TryShoot();
        }

        // Throwing weapons 
        {
            if (input.isThrowingWeapon)
                for (int i = 0; i < _weaponsHolder.transform.childCount; i++)
                {
                    GameObject currentWeaponGO = _weaponsHolder.transform.GetChild(i).gameObject;
                    currentWeaponGO.transform.SetParent(transform.parent);
                    currentWeaponGO.GetComponent<Rigidbody>().AddForce(transform.forward * _stats.throwStrength);
                }
        }
    }
}
