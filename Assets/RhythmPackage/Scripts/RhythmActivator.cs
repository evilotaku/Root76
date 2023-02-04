using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmActivator : MonoBehaviour
{
    Image image;
    public bool active = false;
    public GameObject note;
    Color oldC;

    public GameObject RSM;

    public float PerfectOffset = 0.2f;

    


    // Start is called before the first frame update
    void Start()
    {
      RSM = GameObject.FindWithTag("RhythmManager");
      image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown("space"))
        {
          StartCoroutine(Pressed());
        }

        if(Input.GetKeyDown("space") && active)
        {
          if((note.transform.position.x > transform.position.x - PerfectOffset) && (note.transform.position.x < transform.position.x + PerfectOffset))
          {
                Debug.Log("Perfect");
                RSM.GetComponent<ScoreManager>().Hit();
          }

          if(note.transform.position.x > transform.position.x - PerfectOffset && !(note.transform.position.x < transform.position.x + PerfectOffset))
          {
            Debug.Log("Early");
            RSM.GetComponent<ScoreManager>().LateOrEarlyHit();
          }

          if(note.transform.position.x < transform.position.x - PerfectOffset && !(note.transform.position.x > transform.position.x - PerfectOffset))
          {
            Debug.Log("Late");
            RSM.GetComponent<ScoreManager>().LateOrEarlyHit();
          }



          Destroy(note);
          //RSM.GetComponent<ScoreManager>().Hit();
          active=false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
      active = true;
      if(col.gameObject.tag == "note")
      {
        note = col.gameObject;
      }
    }

    void OnTriggerExit2D(Collider2D col)
    {
      active = false;
    }

    IEnumerator Pressed()
    {
      oldC = image.color;
      image.color = new Color(0, 0, 0);
      yield return new WaitForSeconds(0.1f);
      image.color=oldC;
    }
}
