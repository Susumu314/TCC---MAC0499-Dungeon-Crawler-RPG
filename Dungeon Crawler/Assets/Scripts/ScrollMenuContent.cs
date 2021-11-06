using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMenuContent : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ButtonRef;
    Battle_System BS;
    OW_MenuSystem MS;
    Demonologist demonologist;
    public ActionMenu AM;
    bool isInit = false;
    public ButtonSelectionController scrollControler;
    public class MenuButton//criar um construtor pra essa merda
    {
        public GameObject Button;
        public int ID;
        public Demonomicon.SavedDemon demon;
        public MenuButton (GameObject Button, int ID){
            this.Button = Button;
            this.ID = ID;
        }
        public MenuButton (GameObject Button, Demonomicon.SavedDemon demon){
            this.Button = Button;
            this.demon = demon;
        }
    }
    public List<MenuButton> Buttons = new List<MenuButton>();

    public void InitBagMenu(Bag bag, Battle_System b){
        if(isInit){
            return;
        }
        isInit = true;
        BS = b;
        for (int i = 0; i < bag.items.Count; i++)
        {   
            int itemID = bag.items[i].ID;
            int amount = bag.items[i].amount;
            AddButton(amount, itemID);
        }
        SetCancelButton();
    }

    public void InitBagMenu(Bag bag, OW_MenuSystem m){
        if(isInit){
            return;
        }
        isInit = true;
        MS = m;
        for (int i = 0; i < bag.items.Count; i++)
        {   
            int itemID = bag.items[i].ID;
            int amount = bag.items[i].amount;
            AddButton(amount, itemID);
        }
        SetCancelButton();
    }

    public void InitDemonomiconMenu(Demonomicon demonomicon, Demonologist d){
        if(isInit){
            return;
        }
        isInit = true;
        demonologist = d;
        for (int i = 0; i < demonomicon.demonomicon.Count; i++)
        {   
            AddButton(demonomicon.demonomicon[i]);
        }
        SetCancelButton();
    }

    /**
    *   Reseta o menu da mochila, utilizado para quando se obtem um item novo
    */
    public void RefreshBagMenu(Bag bag){
        foreach (Transform child in this.transform) {
            if(child.GetSiblingIndex() != this.transform.childCount - 1){
                GameObject.Destroy(child.gameObject);
            }
        }
        Buttons = new List<MenuButton>();
        for (int i = 0; i < bag.items.Count; i++)
        {   
            int itemID = bag.items[i].ID;
            int amount = bag.items[i].amount;
            AddButton(amount, itemID);
        }
        transform.GetChild(0).SetAsLastSibling();
        SetCancelButton();
        scrollControler.UpdateButtonList();
        scrollControler.BackToTop();
    }

    public void AddButton(int amount, int ID){
        Item.ItemData item = Item.ItemList[ID];
        // if(MS != null && !item.OverworldUse){//se a mochila for aberta no overworld, mostrar apenas itens usaveis no overworld
        //     return;
        // }
        GameObject newButton = Instantiate(ButtonRef, this.transform);
        Buttons.Add(new MenuButton(newButton, ID));
        if(BS != null){
            newButton.transform.GetComponent<Button>().onClick.AddListener(() => BS.OnItemButton(ID));
        }
        else if(MS != null && item.OverworldUse){
            newButton.transform.GetComponent<Button>().onClick.AddListener(() => MS.OnItemButton(ID));
        }
        newButton.transform.GetChild(0).GetComponent<Text>().text = amount + "x" + item.Name;
        newButton.transform.GetComponent<SelectableElement>().Battle_System = BS;
        newButton.transform.GetComponent<SelectableElement>().text = item.Description;
        if(MS != null && !item.OverworldUse){
            newButton.transform.GetComponent<SelectableElement>().text = "Can't use this item outside of battle!";
        }
    } 

    public void AddButton(Demonomicon.SavedDemon demon){
        GameObject newButton = Instantiate(ButtonRef, this.transform);
        Buttons.Add(new MenuButton(newButton, demon));
        newButton.transform.GetComponent<Button>().onClick.AddListener(() => demonologist.OnSummonDemonButton(demon));
        BaseStats.GROWTH_RATE g = BaseStats.SearchDex(demon.SPECIES).growthRate;
        float exponent = 3.0f;
        float baseExp;
        switch (g)
        {
            case BaseStats.GROWTH_RATE.SLOW:
                baseExp = 5f/4f;
            break;
            case BaseStats.GROWTH_RATE.MEDIUM_SLOW:
                baseExp = 6f/5f;
            break;
            case BaseStats.GROWTH_RATE.MEDIUM:
                baseExp = 1;
            break;
            case BaseStats.GROWTH_RATE.FAST:
                baseExp = 4f/5f;
            break;
            default:
                baseExp = 1;
            break;
        }
        int demonLevel = Mathf.FloorToInt(Mathf.Pow(demon.totalExp/baseExp, (1.0f/exponent)));
        newButton.transform.GetChild(0).GetComponent<Text>().text = demon.SPECIES + "." +  demonLevel + "/" + demon.Nickname;
        //adicionar a parte para o selectableElement para quando tiver tudo funcionando menos o display dos status do demonio
        //newButton.transform.GetComponent<SelectableElement>().Battle_System = BS;
        //newButton.transform.GetComponent<SelectableElement>().text = item.Description;
    } 

    /**
    *   Deleta uma entrada do Demonomicon quando se invoca um dos demonios
    */
    public void DeleteDemonomiconEntry(Demonomicon.SavedDemon demon){
        MenuButton button;
        foreach (MenuButton mb in Buttons)
        {
            if(mb.demon == demon){
                button = mb;
                Destroy(button.Button);
                Buttons.Remove(button);
                scrollControler.UpdateButtonList();
                return;
            }
        }
    }

    /**
    *   Adiciona uma entrada do Demonomicon quando se salva um dos demonios
    */
    public void AddDemonomiconEntry(Demonomicon.SavedDemon demon){
        int cancelButtonIndex = transform.childCount - 1;
        AddButton(demon);
        transform.GetChild(cancelButtonIndex).SetAsLastSibling();
        AM.defaultButton = transform.GetChild(0).GetComponent<Button>();
        scrollControler.UpdateButtonList();
    }

    public void UpdateButtons(Bag bag){
        if(MS == null){
            for (int i = 0; i < bag.items.Count; i++)
            {
                while(bag.items[i].ID != Buttons[i].ID){
                    if (Buttons[i].Button == AM.defaultButton){
                        AM.defaultButton = null;
                    }
                    Destroy(Buttons[i].Button);
                    Buttons.RemoveAt(i);
                }
                Buttons[i].Button.transform.GetChild(0).GetComponent<Text>().text = bag.items[i].amount + "x" + Item.ItemList[bag.items[i].ID].Name;
            }
        }
        else{
            int j = 0;
            for (int i = 0; i < bag.items.Count; i++)
            {
                // if (!Item.ItemList[bag.items[i].ID].OverworldUse){
                //     j++;
                //     continue;
                // }
                while(bag.items[i].ID != Buttons[i-j].ID){
                    if (Buttons[i-j].Button == AM.defaultButton){
                        AM.defaultButton = null;
                    }
                    Destroy(Buttons[i-j].Button);
                    Buttons.RemoveAt(i-j);
                }
                Buttons[i-j].Button.transform.GetChild(0).GetComponent<Text>().text = bag.items[i].amount + "x" + Item.ItemList[bag.items[i].ID].Name;
            }
        }
    }  

    public void SetCancelButton(){
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => AM.OnMainMenuButton());
        transform.GetChild(0).SetAsLastSibling();
        AM.defaultButton = transform.GetChild(0).GetComponent<Button>();
        Debug.Log("Passa pelo set cancel button");
    }
}
