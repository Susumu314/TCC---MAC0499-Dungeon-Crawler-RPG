using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMenuContent : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ButtonRef;
    Battle_System BS;
    public ActionMenu AM;
    bool isInit = false;
    public class MenuButton//criar um construtor pra essa merda
    {
        public GameObject Button;
        public int ID;

        public MenuButton (GameObject Button, int ID){
            this.Button = Button;
            this.ID = ID;
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

    public void AddButton(int amount, int ID){
        Item.ItemData item = Item.ItemList[ID];
        GameObject newButton = Instantiate(ButtonRef, this.transform);
        Buttons.Add(new MenuButton(newButton, ID));
        newButton.transform.GetComponent<Button>().onClick.AddListener(() => BS.OnItemButton(ID));
        newButton.transform.GetChild(0).GetComponent<Text>().text = amount + "x" + item.Name;
        newButton.transform.GetComponent<SelectableElement>().Battle_System = BS;
        newButton.transform.GetComponent<SelectableElement>().text = item.Description;
    } 

    public void UpdateButtons(Bag bag){
        for (int i = 0; i < bag.items.Count; i++)
        {
            while(bag.items[i].ID != Buttons[i].ID){
                Destroy(Buttons[i].Button);
                Buttons.RemoveAt(i);
            }
            Buttons[i].Button.transform.GetChild(0).GetComponent<Text>().text = bag.items[i].amount + "x" + Item.ItemList[bag.items[i].ID].Name;
        }
    }  

    public void SetCancelButton(){
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => AM.OnMainMenuButton());
        transform.GetChild(0).SetAsLastSibling();
    }
}
