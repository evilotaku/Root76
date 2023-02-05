using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.UI;
using System.Threading;
using TMPro;

public class RaceManager : NetworkBehaviour
{
    public float RaceTime;
    public Slider RaceTimer;
    GameObject TimerPanel;
    
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //GetComponent<AudioSource>().Play();
        TimerPanel = GameObject.Find("Timer Panel");
        
        GameManager.instance.OnRacingStart += async () =>
        {
            TimerPanel.SetActive(true);
            await GameManager.instance.Timer(RaceTime, new Progress<float>(percent =>
            {
                print($"Timer: {percent} ");
                RaceTimer.value = percent;
            }), GameManager.instance.cancelSource.Token);            
            GameManager.instance.NextState();
        };
    }   


    // Update is called once per frame
    void Update()
    {
        
    }
}
