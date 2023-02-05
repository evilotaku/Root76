using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public InputActionReference movement;
    public float speed;
    Rigidbody rb;
   

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //movement.action.performed += Action_performed;
        rb = GetComponent<Rigidbody>();
        GameManager.instance.OnRacingStart += () =>
        {
            enabled = true;
        };

    }

    private void Action_performed(InputAction.CallbackContext ctx)
    {
        Vector3 input = new(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
        transform.position += input * speed;
    }

    /*private void OnMove(InputValue input)
    {
        Vector3 move = new(input.Get<Vector2>().x, 0.5f, input.Get<Vector2>().y);
        transform.position += move * speed * Time.deltaTime;
    }*/


    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        Vector3 input = new(movement.action.ReadValue<Vector2>().x, 0, movement.action.ReadValue<Vector2>().y);
        rb.AddForce(input * speed);
        rb.MoveRotation(Quaternion.LookRotation(input));
    }
}
