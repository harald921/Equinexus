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
        Character.Input input = new Character.Input(Quaternion.identity, Vector3.zero, false, false, false);

        // Get shooting/aim input
        if (Input.GetMouseButton(0)) input.isShooting   = true;
        if (Input.GetMouseButton(1)) input.isAiming     = true;

        // Get movement input
        if (Input.GetKey(KeyCode.W))      input.moveDirection += Vector3.forward;
        else if (Input.GetKey(KeyCode.S)) input.moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A))      input.moveDirection += Vector3.left;
        else if (Input.GetKey(KeyCode.D)) input.moveDirection += Vector3.right;

        // Calculate aim target
        Vector3 aimTarget = UnityEngine.Input.mousePosition;
        aimTarget.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        aimTarget = Camera.main.ScreenToWorldPoint(aimTarget);
        aimTarget = new Vector3(aimTarget.x, 1, aimTarget.z);
        input.targetRotation = Quaternion.LookRotation(aimTarget - transform.position);

        _targetCharacter.input = input;
    }
}