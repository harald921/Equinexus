using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpawner : MonoBehaviour 
{
    /* Fields */
    [SerializeField] GameObject _prefabToSpawn;
    [SerializeField] float _spawnInterval = 10.0f;

    float _spawnTimer;

    /* Base */
    private void Start()
    {
        _spawnTimer = Random.Range(1.0f, _spawnInterval);
    }

    private void Update()
    {
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval)
        {
            Instantiate(_prefabToSpawn, transform.position, Quaternion.identity);
            _spawnTimer = 0.0f;
        }
    }

    /* External */

    /* Internal */
}