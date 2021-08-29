using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    private List<GameManager.Tutorial> tutorialList = GameManager.Instance.tutorials;
    public string tutorialName; 

    public int tutorialIndex;
    void Start(){
        int i;
        for (i = 0; i < tutorialList.Count; i++)
        {
            if(tutorialList[i].tutorialName == tutorialName){
                if(tutorialList[i].cleared){
                    Destroy(this.gameObject);
                }
                tutorialIndex = i;
            }
        }
        if(i == tutorialList.Count){
            Debug.Log("Nao achou o tutorial na lista");
        }
    }

    public void OpenTutorial(){
        GameManager.Instance.StartTutorial(tutorialName);
        Destroy(this.gameObject);
    }
}
