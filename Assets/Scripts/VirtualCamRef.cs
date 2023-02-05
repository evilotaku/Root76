using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class VirtualCamRef : MonoBehaviour
{
    public static CinemachineVirtualCamera Instance=null;
    
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance==null){
            Instance=GetComponent<CinemachineVirtualCamera>();
        }
    }

    // Update is called once per frame
    void OnDestroy()
    {
        if(Instance==this) Instance=null;
    }
}
