using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    bool hasBeenSet = false;
    float _lifeTime = 0;
    public float lifeTime
    {
        set
        {
            _lifeTime = value;
            hasBeenSet = true;
        }
    }

    float _currentLifeTime = 0;

    Weapon _parentWeapon; // The weapon the projectile was fired from
    public Weapon parentWeapon {  set { _parentWeapon = value; } }

    void Update()
    {
        if (!hasBeenSet)
            return;

        if ((_currentLifeTime += Time.deltaTime) >= _lifeTime)
            Destroy(gameObject);
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.isTrigger)
            return;

        Character hitCharacter = col.GetComponent<Character>();
        if (hitCharacter)
            hitCharacter.ModifyHealth(-Random.Range(_parentWeapon.stats.damageMin, _parentWeapon.stats.damageMax));

        Destroy(gameObject);
    }
}
