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
    public int ObstacleAmount;
    public GameObject ObstaclePrefab;
    public NetworkVariable<NetworkObjectReference> ObstacleTarget = new();
    List<GameObject> ObstacleList = new();
    
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if(Instance == null) Instance = this;
        //GetComponent<AudioSource>().Play();
        TimerPanel = GameObject.Find("Canvas").transform.Find("Timer Panel").gameObject;
        
        GameManager.OnRacingStart += async () =>
        {
            print("Starting the Race");
            TimerPanel?.SetActive(true);
            if (NetworkManager.Singleton.IsServer) SpawnObstacles();
            await GameManager.instance.Timer(RaceTime, new Progress<float>(percent =>
            {
                print($"Timer: {percent} ");
                RaceTimer.value = percent;
            }), GameManager.instance.cancelSource.Token);            
            GameManager.instance.NextState();
        };
    }   

    void SpawnObstacles()
    {
        print("Spawning Obstacles...");
        for (int i = 0; i < ObstacleAmount; i++)
        {
            Vector3 pos = new Vector3(Random.insideUnitCircle.x * transform.localScale.x, 0, 
                                        Random.insideUnitCircle.y * transform.localScale.x);
            var prefab = Instantiate(ObstaclePrefab, pos , Quaternion.identity);
            prefab.GetComponent<NetworkObject>().Spawn();
            ObstacleList.Add(prefab);
        }
        ObstacleTarget.Value = ObstacleList[Random.Range(0, ObstacleAmount)];
        print($"Obstacle Target is NetObjID: {ObstacleTarget.Value.NetworkObjectId}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
