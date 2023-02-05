using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.UI;
using System.Threading;
using TMPro;
using Random = UnityEngine.Random;

public class RaceManager : NetworkBehaviour
{
    public static RaceManager Instance;
    public float RaceTime;
    public Slider RaceTimer;
    GameObject TimerPanel;
    int ObstacleAmount;
    GameObject ObstaclePrefab;
    public GameObject ObstacleTarget;
    List<GameObject> ObstacleList = new();
    
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if(Instance != null) Instance = this;
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
            if (IsServer) SpawnObstacles();
            GameManager.instance.NextState();
        };
    }   

    void SpawnObstacles()
    {
        for (int i = 0; i < ObstacleAmount; i++)
        {
            var pos = new Vector3(Random.insideUnitCircle.x, 0, UnityEngine.Random.insideUnitCircle.y);
            var prefab = Instantiate(ObstaclePrefab, pos * 10, Quaternion.identity);
            prefab.GetComponent<NetworkObject>().Spawn();
            ObstacleList.Add(prefab);
        }
        ObstacleTarget = ObstacleList[Random.Range(0, ObstacleAmount)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
