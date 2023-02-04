using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.UI;
using System.Threading;

public class RaceManager : NetworkBehaviour
{
    public float RaceTime;
    public Slider RaceTimer;
    
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //GetComponent<AudioSource>().Play();
        GameManager.instance.OnRacingStart += async () =>
        {
            await GameManager.instance.Timer(RaceTime, new Progress<float>(percent =>
            {
                print($"Timer: {percent} ");
                RaceTimer.value = percent;
            }), GameManager.instance.cancelSource.Token);
        };
    }   


    // Update is called once per frame
    void Update()
    {
        
    }
}
