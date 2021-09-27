using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractableType {Door, Shortcut};
    public InteractableType type;
    public int ID;
    public bool used; //o valor desse used deve estar salvo no game manager

    void Start(){
        if(used){
            this.transform.GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    public void Act(PlayerController p){
        if (type == Interactable.InteractableType.Shortcut){
            Vector3 direction = (this.transform.position - p.transform.position).normalized;
            print(direction);
            if((direction == Vector3.forward) || used){
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
    }
}
