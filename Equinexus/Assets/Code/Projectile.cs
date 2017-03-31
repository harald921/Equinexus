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

    void Update()
    {
        if (!hasBeenSet)
            return;

        if ((_currentLifeTime += Time.deltaTime) >= _lifeTime)
            Destroy(gameObject);
    }
}
