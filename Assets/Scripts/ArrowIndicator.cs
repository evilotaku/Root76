using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ArrowIndicator : NetworkBehaviour
{
    public float height;
    Renderer Renderer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(RaceManager.Instance.ObstacleTarget.transform);
        Renderer.material.color = Color.Lerp(Color.red, Color.green, Vector3.Distance(transform.position, RaceManager.Instance.ObstacleTarget.transform.position));
    }
    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
