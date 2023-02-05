using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public InputActionReference movement;
    public float speed;

    public float maxRotation=50f;
    public float acceleration = 10f;
    public float steering = 5f;
    public float maxSpeed = 20f;
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    Rigidbody rb;
   public bool forceOwner=false;

   void Start(){
    if(forceOwner){
        rb=GetComponent<Rigidbody>();
    }
   }

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //movement.action.performed += Action_performed;
        rb = GetComponent<Rigidbody>();
        GameManager.OnRacingStart += () =>
        {
            if (!IsOwner) return;
            print("We should be able to move!");
            enabled = true;
        };

    }

    private void Action_performed(InputAction.CallbackContext ctx)
    {
        /* Vector3 input = new(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
        transform.position += input * speed; */
    }

    /*private void OnMove(InputValue input)
    {
        Vector3 move = new(input.Get<Vector2>().x, 0.5f, input.Get<Vector2>().y);
        transform.position += move * speed * Time.deltaTime;
    }*/


    // Update is called once per frame
    void Update()
    {
        if (!IsOwner&&!forceOwner) return;
        Vector3 input = new(movement.action.ReadValue<Vector2>().x, 0, movement.action.ReadValue<Vector2>().y);

        float horizontalInput = movement.action.ReadValue<Vector2>().x;//Input.GetAxis("Horizontal");
        float verticalInput = movement.action.ReadValue<Vector2>().y;//Input.GetAxis("Vertical");
        //Debug.LogFormat("h:{0} v:{1}",horizontalInput,verticalInput);
        float steeringAngle = horizontalInput * steering;
        float accelerationForce = verticalInput * acceleration;

        wheelColliders[0].steerAngle = steeringAngle;
        wheelColliders[1].steerAngle = steeringAngle;

        wheelColliders[2].motorTorque = accelerationForce;
        wheelColliders[3].motorTorque = accelerationForce;

        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed), Mathf.Clamp(rb.velocity.z, -maxSpeed, maxSpeed));
        
        //CheckUpsideDown();

/* 
        rb.AddForce(input * speed);
        if(input != Vector3.zero)
            rb.MoveRotation(Quaternion.LookRotation(input)); */
    }

    private void CheckUpsideDown()
    {
       /*  if (transform.up.y < 0.5f&&rb.angularVelocity)
        {
            rb.MoveRotation(Quaternion.LookRotation(transform.forward, Vector3.up));
            
        } */
        
        Vector3 rotation = rb.rotation.eulerAngles;
        rotation.z = Mathf.Clamp(rotation.z%360, -maxRotation, maxRotation);
        rotation.x=Mathf.Clamp(rotation.x%360,-maxRotation,maxRotation);
        rb.rotation = Quaternion.Euler(rotation);
    }
}
