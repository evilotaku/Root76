using UnityEngine;
using Unity.Netcode;

public class ScoreableObject : NetworkBehaviour
{
    public int Score;
    public Rigidbody rb;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        rb = GetComponent<Rigidbody>();

    }

    [ServerRpc(RequireOwnership = false)]
    public void BlowUpServerRPC(float force)
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        //if (!IsOwner) return;
        if (collision.transform.TryGetComponent(out PlayerRaceController obj))
        {
            print("Boom!");
            rb.AddExplosionForce(100f, collision.transform.position, 10f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
