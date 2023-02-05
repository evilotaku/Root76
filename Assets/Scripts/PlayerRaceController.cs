using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerRaceController : NetworkBehaviour
{

    public NetworkVariable<int> score = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public TMP_Text score_text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        score.OnValueChanged += (int oldValue, int newValue) =>
        {
            score_text.text = $"{newValue}";
        };
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        if (collision.transform.TryGetComponent(out ScoreableObject obj))
        {
            score.Value += obj.Score;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //score_text.transform.LookAt(Camera.main.transform);
    }
}
