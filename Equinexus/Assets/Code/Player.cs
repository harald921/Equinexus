using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 50;

    [SerializeField] GameObject _weaponsGO;

    void Update()
    {
        HandleMovement();
        HandleAim();
    }

    void HandleAim()
    {
        if (!Input.GetMouseButton(1))
            return;
        
        // Look in the direction of the mouse
        Vector3 aimPosition = Input.mousePosition;
        aimPosition.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimPosition = Camera.main.ScreenToWorldPoint(aimPosition);
        transform.LookAt(new Vector3(aimPosition.x, 1, aimPosition.z));

        HandleShooting();
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0))
            for (int i = 0; i < _weaponsGO.transform.childCount; i++)
                _weaponsGO.transform.GetChild(i).gameObject.GetComponent<Weapon>().TryShoot();
    }

    void HandleMovement()
    {
        Vector3 movementDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            movementDirection += Vector3.forward;

        else if (Input.GetKey(KeyCode.S))
            movementDirection += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            movementDirection += Vector3.left;

        else if (Input.GetKey(KeyCode.D))
            movementDirection += Vector3.right;

        Rigidbody playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.AddForce(movementDirection.normalized * _moveSpeed);
    }
}

/* CURRENT 
 *  - Weapons: Make it possible to drop and pick up weapons
 */

 /*  // TODO // 
 *  - AI: Implement enemy that shoots at the player from a distance
 *  - AI: Implement rusher enemy
 * 
 *  - UI: Show bullets left in magazine + magazine size
 *  - UI: Make circular reload progress bar
 *  - UI: Make health bar for player and enemies
 *  
 *  - Weapons: Make weapon recoil when shot
 *  - Weapons: Add bolt action, shotgun, desert eagle and minigun
 *  - Weapons: Add casings that land on the ground around the player
 *  - Weapons: Make it possible to dual wield weapons
 *  
 *  - Sound: Add footsteps for player and enemies
 *  - Sound: Add shooting sounds for weapons
 *  - Sound: Add reload sounds for weapons
 */