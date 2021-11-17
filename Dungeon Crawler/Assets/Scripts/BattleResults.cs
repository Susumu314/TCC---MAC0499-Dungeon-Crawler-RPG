using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 

public class BattleResults : MonoBehaviour
{
    private int EXP = 0;
    public List<Item> LootList;
    public List<Transform> Units = new List<Transform>();
    [SerializeField]
    private TextMeshProUGUI expText;
    private int expPerUnit;
    private float[] targetProgress;
    private float[] initialProgress;
    private List<Image> xpProgressBars = new List<Image>(); 
    private bool canExit = false;
    private float fillSpeed = 1f;
    private bool evolution = false;
    private bool learnSkill = false;
    private bool open = false;
    public void OpenResultScreen(Unit[] p){
        this.gameObject.GetComponent<RawImage>().enabled = true;
        foreach(Transform child in transform) {
            if(child.GetSiblingIndex() > 5){
                child.gameObject.SetActive(true);
            }
        }
        int i = 0;
        int s = 0;
        Transform childNode;
        foreach (Unit u in p){
            if(u && u.species != ""){
                childNode = this.transform.GetChild(i);
                childNode.gameObject.SetActive(true);
                print(childNode.Find("Name"));
                childNode.Find("Name").GetComponent<TextMeshProUGUI>().text = u.unitName;
                //Level e Next devem ser atualizados junto com a barra de progresso de xp
                childNode.Find("Level").GetComponent<TextMeshProUGUI>().text = ""+u.unitLevel;
                i++;
                if(!u.isDead){
                    s++;
                }
                Units.Add(childNode);
                xpProgressBars.Add(childNode.Find("XP_bg").GetChild(0).GetComponent<Image>());
            }
        }
        targetProgress = new float[i];
        initialProgress = new float[i];
        expPerUnit = Mathf.RoundToInt((float)EXP/(float)s);
        print(expPerUnit);
        i = 0;
        float[] progress = new float[2];
        //Distribui a experiencia para as unidades do jogador, e seta a barra de xp para a animação de ganhar experiencia
        foreach (Unit u in p){
            if(u && u.species != ""){
                if(!u.isDead){
                    progress = u.GainExp(expPerUnit);
                    initialProgress[i] = progress[0];
                    targetProgress[i] = progress[1];
                    xpProgressBars[i].fillAmount = initialProgress[i];
                    Units[i].Find("Next").GetComponent<TextMeshProUGUI>().text = ""+(u.expForNextLevel - u.totalExp);
                    if(u.canEvolve){
                        evolution = true;
                    }
                    if(u.canLearnSkill){
                        learnSkill = true;
                    }
                }
                i++;
            }
        }
        print("Chegou ao final do Open Results");
        open = true;
    }
    public void TotalEXP(int xp){
        EXP = xp;
        expText.text = "" + xp;
    }

    void Update(){
        if(open){
            FillXpBars();
            if(Input.GetButtonDown("Submit") && canExit){
                if(evolution){
                    GameManager.Instance.state = GameManager.State.Evolution;
                    SceneManager.LoadScene("EvolutionScene");
                }
                else if(learnSkill){
                    GameManager.Instance.state = GameManager.State.LearnSkill;
                    SceneManager.LoadScene("LearnSkill");
                }
                else{
                    SceneManager.LoadScene(GameManager.Instance.CurrentOverworldScene);
                }
            }
        }
    }

    private void FillXpBars(){
        canExit = true;
        for (int i = 0; i < xpProgressBars.Count; i++)
        {
            if(xpProgressBars[i].fillAmount < targetProgress[i]){
                canExit = false;
                xpProgressBars[i].fillAmount += fillSpeed*Time.deltaTime;
                if(xpProgressBars[i].fillAmount >= 1.0f){
                    xpProgressBars[i].fillAmount -= 1.0f;
                    targetProgress[i] -= 1.0f;
                    Units[i].Find("Level").GetComponent<TextMeshProUGUI>().text = ""+(int.Parse(Units[i].Find("Level").GetComponent<TextMeshProUGUI>().text)+1);
                }
                if(xpProgressBars[i].fillAmount >= targetProgress[i]){
                    xpProgressBars[i].fillAmount = targetProgress[i];
                }
            }
        }
        
    }

}
