using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Character _targetCharacter;


    void Start()
    {
        _targetCharacter = GetComponent<Character>();
    }

    void Update()
    {
        Character.Input input = new Character.Input(Quaternion.identity, Vector3.zero, false, false, false, false, false);

        // Get shooting/aim input
        if (Input.GetMouseButton(0)) input.shoot   = true;
        if (Input.GetMouseButton(1)) input.aim     = true;

        // Get movement input
        if (Input.GetKey(KeyCode.W))      input.moveDirection += Vector3.forward;
        else if (Input.GetKey(KeyCode.S)) input.moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A))      input.moveDirection += Vector3.left;
        else if (Input.GetKey(KeyCode.D)) input.moveDirection += Vector3.right;

        // Get throwing weapon input

        input.throwWeapon = Input.GetKeyDown(KeyCode.G);
        input.use         = Input.GetKeyDown(KeyCode.E);
        input.reload      = Input.GetKeyDown(KeyCode.R);

        // Calculate aim target
        Vector3 aimTarget    = UnityEngine.Input.mousePosition;
        aimTarget.z          = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimTarget            = Camera.main.ScreenToWorldPoint(aimTarget);
        aimTarget            = new Vector3(aimTarget.x, 1, aimTarget.z);
        input.targetRotation = Quaternion.LookRotation(aimTarget - transform.position);

        _targetCharacter.input = input;
    }
}

/*  // TODO // 
 *  
 *  - Structure: Child projectiles to a GameObject when they get spawned
 *  - Structure: Move all the stuff in Character.HandleInput into own methods so that DRY can be applied
 *  - Structure: Clean up the PlayerUI script. Make it get called rather than the opposite
 *  - Structure: Add a vector of flags to Character, that holds stuff like "IS_AIMING" or "HURT" or whatever. Different states
 *  - Structure: Separate how characters die into own method
 *  
 *  - Enemies: Implement enemy that shoots at the player from a distance
 *  - Enemies: Make enemies have two types, metal and biological. One weak to ballistic projectiles, the other to plasma projectiles
 * 
 *  - UI: Make world space health bar for player and enemies
 *  - UI: Make world space progress bar for "generators" that need to be charged up
 *  
 *  - Effects: Add light that flashes when weapons fire
 *  - Effects: Add particle effect to muzzle (Muzzle flash)
 *  - Effects: Add particle effect to where bullets hit
 *  
 *  - Weapons: Add plasma weapon
 *  - Weapons: Add grenade which pushes enemies away and deals damage depending on distance
 *  - Weapons: Make weapon recoil when shot
 *  - Weapons: Add bolt action, desert eagle and minigun
 *  - Weapons: Add casings that land on the ground around the player
 *  - Weapons: Change the way hit detection is made to it is explained at: "http://answers.unity3d.com/questions/661326/bullet-goes-through-object.html"
 *  
 *  - Sound: Add footsteps for player and enemies
 *  - Sound: Add shooting sounds for weapons
 *  - Sound: Add reload sounds for weapons
 *  - Sound: Add death sounds for enemies and player
 *  
 *  - Map: Add "generator" which the player has to defend for it to charge up. When charged up, its script has a bool turned to true
 *  - Map: Add door which can be set to listen for something (like a generator) before it opens
 *  - Map: Add enemy spawners
 */
