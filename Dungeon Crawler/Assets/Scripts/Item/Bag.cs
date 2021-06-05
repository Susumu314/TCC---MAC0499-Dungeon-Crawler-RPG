using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Classe utilizada para representar o inventário onde serão armazenados os itens do jogador
*/
public class Bag : MonoBehaviour
{   
    [System.Serializable]
    public class StoredItem
    {
        public int amount;
        public int ID;
    }
    public List<StoredItem> items = new List<StoredItem>(); 

    public bool UseItem(int itemID){
        foreach(StoredItem item in items){
            if(item.ID == itemID){
                item.amount--;
                if(item.amount <= 0){
                    items.Remove(item);
                }
                return true;
            }
        }
        return false;
    }

    public void AddItem(int position){

    }
}
