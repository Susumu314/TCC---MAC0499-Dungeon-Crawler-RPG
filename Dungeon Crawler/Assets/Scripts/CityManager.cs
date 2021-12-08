using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CityManager : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private string npc_name = "Player";
    public GameObject menu;
    public bool canOpenMenu = true;

    void Start(){
        GameManager.Instance.Init();
        dialogueManager = FindObjectOfType<DialogueManager>();
        if(GameManager.Instance.state == GameManager.State.GameOver){
            HealAll();
        }
        GameManager.Instance.state = GameManager.State.City;
        AudioManager.instance.Play("City");
    }
    void Update(){
        if(dialogueManager.dialogueFinished && canOpenMenu){
			if(!menu.activeSelf){
				menu.SetActive(true);
			}
		}
    }
    public void ChangeScene(string sceneName){
        menu.SetActive(false);
        switch (sceneName)
        {
            case "Guild":
            break;
            case "DemonologistShop":
                if(!GameManager.Instance.EventList[(int)GameManager.Event.GotFirstDemon]){
                    dialogueManager.StartDialogue(new Dialogue(npc_name, 
                                      new string[2] {"This shop is for DEMON TAMERS only.",
                                      "Maybe I can get a DEMON at the GUILD."
                                      },
                                                    1f));
                    return;
                }
            break;
            case "Solana Forest":
                if(!GameManager.Instance.EventList[(int)GameManager.Event.GotFirstDemon]){
                    dialogueManager.StartDialogue(new Dialogue(npc_name, 
                                      new string[2] {"It's too dangerous to enter the forest without a DEMON.",
                                      "Maybe I can get a DEMON at the GUILD."
                                      },
                                                    1f));
                    return;
                }
            break;
            default:
            break;
        }
        SceneManager.LoadScene(sceneName);
    }
    private void HealAll(){
        foreach (Unit unit in GameManager.Instance.party.GetComponentsInChildren<Unit>())
        {
            if(unit){
                if(unit.species != ""){
                    unit.FullHeal();
                }
            }
        }
    }
}
