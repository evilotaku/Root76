using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public CharacterDB CharacterDB;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        foreach (var client in GameManager.instance.ClientData)
        {
            var pos = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
            var character = CharacterDB.GetCharacterByID(client.Value.characterId);
            var player = Instantiate(character?.GameplayPrefab, pos, Quaternion.identity);
            player.SpawnAsPlayerObject(client.Value.clientID);
        }
    }
}
