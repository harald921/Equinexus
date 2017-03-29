using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        public float    fireRate;
        public float    projectileSpread;
        public float    velocityInconsistensy;

        public int      damageMin;
        public int      damageMax;

        public int      projectilesPerShot;

        public float    projectileSpeed;

        public float    projectileLifeTime;

        public int      weaponAmmoMax;
        public float    reloadSpeed;
    }

    [SerializeField] Stats      _stats;
    [SerializeField] GameObject  _projectileGO;

    Transform _muzzleTransform;


    bool _isFiring       = false;
    float _fireProgress   = 1.0f;
    bool  _isReloading    = false;
    float _reloadProgress = 1.0f;

    int _weaponAmmoCurrent;

    void Start()
    {
        _muzzleTransform = transform.GetChild(0);

        _weaponAmmoCurrent = _stats.weaponAmmoMax;
    }

    void Update()
    {
        // Add time to the fire progress and check if it is finished
        if (_isFiring)
            if ((_fireProgress += _stats.fireRate * Time.deltaTime) > 1)
            {
                _isFiring = false;
                _fireProgress = 1;
            }

        // Add time to the reload progress and check if it is finished
        if (_isReloading)
            if ((_reloadProgress += _stats.reloadSpeed * Time.deltaTime) > 1)
            {
                // Refill ammo in weapon
                _weaponAmmoCurrent = _stats.weaponAmmoMax;

                _isReloading = false;
                _reloadProgress = 1;
            }
    }

    /* External Methods */
    public void TryShoot()
    {
        // Return if weapon is reloading or firing
        if (_isFiring || _isReloading)
            return;

        // Reload if the weapon has no ammo
        if (_weaponAmmoCurrent == 0)
        {
            TryReload();
            return;
        }

        // Spawn new projectile, set position and lifetime
        for (int i = 0; i < _stats.projectilesPerShot; i++)
        {
            GameObject newProjectileGO = MonoBehaviour.Instantiate(_projectileGO);
            Rigidbody newProjectileRB = newProjectileGO.GetComponent<Rigidbody>();
            newProjectileGO.transform.position = _muzzleTransform.position;
            newProjectileGO.GetComponent<Projectile>().lifeTime = _stats.projectileLifeTime;

            // Calculate new projectiles vector
            Vector3 newProjectileSpread = new Vector3(Random.Range(-_stats.projectileSpread, _stats.projectileSpread), 0, Random.Range(-_stats.projectileSpread, _stats.projectileSpread));
            Vector3 newProjectileDirection = _muzzleTransform.forward + newProjectileSpread;
            float newProjectileVelocityModifier = _stats.projectileSpeed + Random.Range(-_stats.velocityInconsistensy, _stats.velocityInconsistensy);

            newProjectileRB.AddForce(newProjectileDirection * newProjectileVelocityModifier);

            _weaponAmmoCurrent--;
        }

        _isFiring = true;
        _fireProgress = 0;
    }

    public void TryReload()
    {
        // Return if the ammo is already reloaded
        if (_weaponAmmoCurrent == _stats.weaponAmmoMax)
            return;

        // Return if the weapon is already being reloaded
        if (_isReloading)
            return;

        _isReloading = true;
        _reloadProgress = 0;
    }
}