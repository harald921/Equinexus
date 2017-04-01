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
        // Aiming
        {
            if (_hand.transform.childCount > 0)
            {
                if (input.aim) _hand.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = true;
                else           _hand.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = false;
            }

            
        }

        // Movement
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(input.moveDirection.normalized * _stats.baseMoveSpeed);
        }

        // Aiming
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, input.targetRotation, _stats.baseTurnSpeed * Time.deltaTime);
        }

        // Shooting
        {
            if (input.shoot && _hand.transform.childCount > 0)
                firstShot = _hand.transform.GetChild(0).gameObject.GetComponent<Weapon>().TryShoot(firstShot);

            else
                firstShot = true;
        }

        // Reloading
        {
            if (input.reload && _hand.transform.childCount > 0)
                _hand.transform.GetChild(0).gameObject.GetComponent<Weapon>().TryReload();
        }

        // Throwing weapons 
        {
            if (input.throwWeapon)
            {
                GameObject currentWeaponGO = _hand.transform.GetChild(0).gameObject;

                currentWeaponGO.GetComponent<Rigidbody>().isKinematic = false;
                currentWeaponGO.GetComponent<MeshCollider>().enabled = true;
                _hand.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = false;

                currentWeaponGO.transform.SetParent(transform.parent);
                currentWeaponGO.GetComponent<Rigidbody>().AddForce(transform.forward * _stats.throwStrength);
            }
        }

        // Pick up weapons
        {
            if (input.use && _weaponsInReach.Count > 0)
            {
                if (_hand.transform.childCount > 0)
                {
                    // Swap weapons
                    _hand.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = false;
                    GameObject currentWeaponGO = _hand.transform.GetChild(0).gameObject;
                    currentWeaponGO.GetComponent<Rigidbody>().isKinematic = false;
                    currentWeaponGO.GetComponent<MeshCollider>().enabled = true;
                    currentWeaponGO.transform.SetParent(transform.parent);
                    currentWeaponGO.GetComponent<Rigidbody>().AddForce((transform.forward * 100) + (transform.right * 50));
                }

                // Pick up weapon
                _weaponsInReach[0].GetComponent<Rigidbody>().isKinematic = true;
                _weaponsInReach[0].GetComponent<MeshCollider>().enabled = false;
                _weaponsInReach[0].transform.SetParent(_hand.transform);
                _weaponsInReach[0].transform.localEulerAngles = new Vector3(0, 0, 0);
                _weaponsInReach[0].transform.localPosition = new Vector3(0, 0, 0);
                _weaponsInReach.RemoveAt(0);
            }
        }
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
