using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ArrowIndicator : NetworkBehaviour
{
    public float height;
    Renderer Renderer;
    public NetworkObject target;
    float maxDist;


    // Start is called before the first frame update
    void Start()
    {
        RaceManager.Instance.ObstacleTarget.OnValueChanged += TargetChanged;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Renderer = GetComponent<Renderer>();
        if (!IsOwner) Renderer.enabled = false;
        
    }

    private void TargetChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
    {        
        newValue.TryGet(out target);
        maxDist = Vector3.Distance(transform.position, target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        RaceManager.Instance.ObstacleTarget.Value.TryGet(out target);
        if (target == null || !IsOwner)
        {
            Renderer.enabled = false;
            return;
        }
        Renderer.enabled = true;
        transform.parent.LookAt(target.transform);
        var dist = Vector3.Distance(transform.position, target.transform.position);
        //print($"we are {dist} away from target...");
        Renderer.material.color = Color.Lerp(Color.red, Color.green, 2f/dist);
    }
    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
