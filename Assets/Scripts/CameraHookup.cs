using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
public class CameraHookup : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;
    /* public Transform followTarget, lookatTarget; */
    // Start is called before the first frame update
    void Start()
    {
        
        if(GetComponentInParent<NetworkObject>().IsLocalPlayer){
            VirtualCamera.transform.SetParent(null,true);
            VirtualCamera.enabled=true;
            /* var vc=VirtualCamRef.Instance;
            vc.LookAt=lookatTarget;
            vc.Follow=followTarget; */
        }
    }

    
}
