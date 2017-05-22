using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    Color _originalColor;

    [SerializeField]
    int _team;
    public int Team
    {
        get { return _team; }
    }

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

    private void Start()
    {
        _originalColor = GetComponent<MeshRenderer>().material.color;
    }

    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        HandleAiming();
        HandleRotation();

        HandleShooting();

        HandleReloading();

        HandleWeaponThrowing();

        HandleWeaponPickUp();
    }

    /* External Methods */
    public void ModifyHealth(float healthModifier)
    {
        _stats.health += healthModifier;

        StartCoroutine("DamageFlash");

        if (_stats.health <= 0)
            Destroy(gameObject);

        GetComponent<NavMeshAgent>().velocity = Vector3.zero;

    }

    IEnumerator DamageFlash()
    {
        float flashTimer = 1.0f;
        float flashSpeed = 6.0f;
        Color originalColor = _originalColor;
        
        while (flashTimer > 0.0f)
        {
            GetComponent<MeshRenderer>().material.color = Color.Lerp(originalColor, Color.red, flashTimer);
            flashTimer -= Time.deltaTime * flashSpeed;

            yield return null;
        }
    }

    /* Internal Methods */
    void HandleMovement()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(input.moveDirection.normalized * _stats.baseMoveSpeed * Time.deltaTime);
    }

    void HandleAiming()
    {
        if (_hand)
            if (_hand.transform.childCount > 0)
                _hand.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = input.aim;
    }

    void HandleRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, input.targetRotation, _stats.baseTurnSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        if (input.shoot)
        {
            if (_hand.transform.childCount > 0)
                firstShot = _hand.transform.GetChild(0).gameObject.GetComponent<Weapon>().TryShoot(firstShot);
        }

        else
            firstShot = true;
    }

    void HandleReloading()
    {
        if (input.reload)
            if (_hand.transform.childCount > 0)
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
