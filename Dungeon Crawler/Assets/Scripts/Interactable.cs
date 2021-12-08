using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    public enum InteractableType {LoadZone, Shortcut};
    public InteractableType type;
    public int ID;
    public bool used; //o valor desse used deve estar salvo no game manager
    public string LoadingZone = "";

    void Start(){
        StartCoroutine(CheckUsage());
    }
    public void Act(PlayerController p){
        if (type == InteractableType.Shortcut){
            Vector3 direction = (this.transform.position - p.transform.position).normalized;
            print(direction);
            if((direction == transform.forward) || used){//mexi aqui para tentar consertar o shortcut era Vector3.foward antes
                p.movePoint.position += p.transform.rotation * Vector3.forward * 2 *p.step;
                if(!used){
                    used = true;
                    GameManager.Instance.AddInteractable(ID);
                    this.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            else{
                if(!this.transform.GetChild(0).gameObject.activeInHierarchy){
                    this.transform.GetChild(0).gameObject.SetActive(true);
                }
                p.ShowDialog("Looks like you can open a passage from the other side.");
            }
        }  
        if (type == InteractableType.LoadZone){
            FindObjectOfType<GameHandler_Overmap>().SaveInitialPosition();
            SceneManager.LoadScene(LoadingZone);
        }
    }
    //espera 1 frame para checar se o used foi setado como true no start de outro gameobject
    private IEnumerator CheckUsage(){
        yield return null;
        if(used){
            this.transform.GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
