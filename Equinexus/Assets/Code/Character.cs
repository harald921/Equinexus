using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public struct Input
    {
        public Quaternion targetRotation;
        public Vector3 moveDirection;

        public bool shoot;
        public bool throwWeapon;
        public bool aim;
        public bool use;
        public bool reload;

        public Input(Quaternion inTargetRotation, Vector3 inMoveDirection, bool inShoot, bool inThrowWeapon, bool inAim, bool inUse, bool inReload)
        {
            targetRotation = inTargetRotation;
            moveDirection = inMoveDirection;
            shoot = inShoot;
            throwWeapon = inThrowWeapon;
            aim = inAim;
            use = inUse;
            reload = inReload;
        }
    }
    public Input input;

    [System.Serializable]
    public struct Stats
    {
        public float health;
        public float baseMoveSpeed;
        public float baseTurnSpeed;
        public float throwStrength;
    }
    [SerializeField]
    Stats _stats;

    [SerializeField]
    GameObject _hand;

    List<GameObject> _weaponsInReach = new List<GameObject>();

    bool firstShot = true;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        HandleAiming();
        HandleMovement();
        HandleRotation();

        HandleShooting();

        HandleReloading();

        HandleWeaponThrowing();

        HandleWeaponPickUp();
    }


    void HandleMovement()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(input.moveDirection.normalized * _stats.baseMoveSpeed);
    }

    void HandleAiming()
    {
        if (_hand.transform.childCount > 0)
        {
            if (input.aim) _hand.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = true;
            else _hand.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = false;
        }
    }

    void HandleRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, input.targetRotation, _stats.baseTurnSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        if (input.shoot && _hand.transform.childCount > 0)
            firstShot = _hand.transform.GetChild(0).gameObject.GetComponent<Weapon>().TryShoot(firstShot);
        else
            firstShot = true;
    }

    void HandleReloading()
    {
        if (input.reload && _hand.transform.childCount > 0)
            _hand.transform.GetChild(0).gameObject.GetComponent<Weapon>().TryReload();
    }

    void HandleWeaponThrowing()
    {
        if (input.throwWeapon)
        {
            if (_hand.transform.childCount < 1)
                return;
            GameObject currentWeaponGO = _hand.transform.GetChild(0).gameObject;

            DropWeapon();

            currentWeaponGO.transform.SetParent(transform.parent);
            currentWeaponGO.GetComponent<Rigidbody>().AddForce(transform.forward * _stats.throwStrength);
            currentWeaponGO.GetComponent<Rigidbody>().AddTorque(transform.up * _stats.throwStrength);
        }
    }

    void HandleWeaponPickUp()
    {
        if (input.use && _weaponsInReach.Count > 0)
        {
            if (_hand.transform.childCount > 0)
            {
                // Swap weapons
                GameObject droppedWeaponGO = DropWeapon();

                droppedWeaponGO.GetComponent<Rigidbody>().AddForce((transform.forward * 100) + (transform.right * 50));
            }

            PickUpWeapon(_weaponsInReach[0]);
        }
    }


    GameObject DropWeapon()
    {
        if (_hand.transform.childCount > 0)
        {
            _hand.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = false;
            GameObject currentWeaponGO = _hand.transform.GetChild(0).gameObject;
            currentWeaponGO.GetComponent<Rigidbody>().isKinematic = false;
            currentWeaponGO.GetComponent<MeshCollider>().enabled = true;
            currentWeaponGO.transform.SetParent(transform.parent);

            return currentWeaponGO;
        }

        return null;
    }

    void PickUpWeapon(GameObject inWeaponToPickup)
    {
        inWeaponToPickup.GetComponent<Rigidbody>().isKinematic = true;
        inWeaponToPickup.GetComponent<MeshCollider>().enabled = false;
        inWeaponToPickup.transform.SetParent(_hand.transform);
        inWeaponToPickup.transform.localEulerAngles = new Vector3(0, 0, 0);
        inWeaponToPickup.transform.localPosition = new Vector3(0, 0, 0);

        _weaponsInReach.Remove(inWeaponToPickup);
    }




    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Weapon>())
            _weaponsInReach.Add(col.gameObject);
    }

    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<Weapon>())
            if (_weaponsInReach.Contains(col.gameObject))
                _weaponsInReach.Remove(col.gameObject);
    }
}
