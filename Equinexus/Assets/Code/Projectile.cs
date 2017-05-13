using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float _lifeTime = 0;
    public float lifeTime { set { _lifeTime = value; } }
    float _currentLifeTime = 0;

    Weapon _parentWeapon;
    public Weapon parentWeapon { set { _parentWeapon = value; } }

    float _minimumExtent;
    float _sqrMinimumExtent;
    float _partialExtent;
    Vector3 _previousPosition;
    Rigidbody _rigidbody;
    Collider _collider;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _previousPosition = _rigidbody.position;
        _minimumExtent = Mathf.Min(Mathf.Min(_collider.bounds.extents.x, _collider.bounds.extents.y), _collider.bounds.extents.z);
        _partialExtent = _minimumExtent * (1.0f - 0.1f);
        _sqrMinimumExtent = _minimumExtent * _minimumExtent;
    }

    private void Update()
    {
        if ((_currentLifeTime += Time.deltaTime) >= _lifeTime)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(transform.position, _previousPosition, Color.red, 0.1f);

        Vector3 movementThisStep = _rigidbody.position - _previousPosition;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > _sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);

            RaycastHit hitInfo;

            if (Physics.Raycast(_previousPosition, movementThisStep, out hitInfo, movementMagnitude))
                if (hitInfo.collider)
                    if (!hitInfo.collider.isTrigger)
                        _rigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * _partialExtent;
        }

        _previousPosition = _rigidbody.position;
    }

    private void OnTriggerEnter(Collider col)
    {
        OnProjectileHit(col); 
    }

    private void OnProjectileHit(Collider hitCollider)
    {
        if (hitCollider.isTrigger)
            return;

        Debug.Log(hitCollider.gameObject.name);

        Character hitCharacter = hitCollider.GetComponent<Character>();
        if (hitCharacter)
            hitCharacter.ModifyHealth(-Random.Range(_parentWeapon.stats.damageMin, _parentWeapon.stats.damageMax));

        Destroy(gameObject);
    }
}
