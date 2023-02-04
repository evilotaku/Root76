using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int numOfNotes;
    public float eachAcc;
    public float totalAcc;

    public float LateEarlyPenaltyD =1.5f;
    // Start is called before the first frame update
    void Start()
    {
        eachAcc = 100/numOfNotes;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hit()
    {
      totalAcc += eachAcc;
    }

    public void LateOrEarlyHit()
    {
      totalAcc += eachAcc/LateEarlyPenaltyD;
    }
}
