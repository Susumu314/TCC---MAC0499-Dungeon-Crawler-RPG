using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleResults : MonoBehaviour
{
    public int EXP = 0;
    public List<Item> LootList;
    public Unit[] Party;
    public void OpenResultScreen(Unit[] p){
        this.gameObject.GetComponent<RawImage>().enabled = true;
        foreach(Transform child in transform) {
            child.gameObject.SetActive(true);
        }
        int i = 0;
        Transform childNode;
        foreach (Unit u in p){
            if(u){
                childNode = this.transform.GetChild(i);
                childNode.Find("Name").GetComponent<TextMeshPro>().text = u.unitName;
                childNode.Find("Level").GetComponent<TextMeshPro>().text = ""+u.unitLevel;
                childNode.Find("Next").GetComponent<TextMeshPro>().text = ""+(u.expForNextLevel - u.totalExp);
                i++;
            }
        }
    }
}
