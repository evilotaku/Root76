using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIHelper : MonoBehaviour
{
    public Image LoadingPanel;
    public float loadPanelFadeInTime=0.5f;
    public void ShowLoadingScreen(){
        StartCoroutine(_showLoadingScreen());
    }
    IEnumerator _showLoadingScreen(){
        LoadingPanel.gameObject.SetActive(true);
        float elapsedTime=0f;
        Color startColor=LoadingPanel.color;
        while(elapsedTime<=loadPanelFadeInTime){
            startColor.a=Mathf.Lerp(0,1,elapsedTime/loadPanelFadeInTime);
            LoadingPanel.color=startColor;
            yield return 0;
            elapsedTime+=Time.deltaTime;
        }
    }
    public void QuitGame(){
        Application.Quit();
    }
}
