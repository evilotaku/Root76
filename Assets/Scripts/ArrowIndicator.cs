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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Renderer = GetComponent<Renderer>();
        RaceManager.Instance.ObstacleTarget.OnValueChanged += TargetChanged;
    }

    private void TargetChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
    {        
        newValue.TryGet(out target);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        transform.LookAt(target?.transform);
        Renderer.material.color = Color.Lerp(Color.red, Color.green, Vector3.Distance(transform.position, target.transform.position));
    }
    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
