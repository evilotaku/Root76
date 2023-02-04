using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehaviour : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 0.5f;

    void Awake(){
      rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = new Vector2(-speed,0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
