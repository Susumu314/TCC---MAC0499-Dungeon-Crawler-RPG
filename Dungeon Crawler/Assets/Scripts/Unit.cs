using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;

    public int maxHP;
    public int currentHP;

    public int maxMana;
    public int currentMana;

    public int speed;

    public bool isPlayerUnit;

    public bool isDead = false;

    public BattleAction Move;
    public EnemyBattleAI AI; //used only for NonPlayableUnits

    private Battle_System BS;

    public PlayerBattleHUD HUD;
    private bool OnGuard = false; // devo resetar isso alguma hora depois do BattleTurn,
                                 // provavelmente quando o personagem for selecionado para escolher a sua proxima ação

    void Start(){
        BS = GameObject.Find("Battle_System").GetComponent<Battle_System>(); //referencia para o script do Battle_System
    }
    public void TakeDamage(int damage){
        string mensage = "";
        if (!OnGuard){
            HUD.SetHP(Math.Max(0, currentHP - damage));
            mensage = unitName + ": AHHHHH I took " + damage + "\n I have " + currentHP + " remaining. \n";
        }
        else{
            HUD.SetHP(Math.Max(0, currentHP - damage/2));//como placeholder, se o personagem estiver defendendo leva metade do dano
            mensage = unitName + ": AHHHHH I took " + damage/2 + "\n I have " + currentHP + " remaining. \n";
        }
        print(mensage);
        if (currentHP == 0)
            Dead();
        //play action animation
        //play soundfx
        //display mensage
        //wait for animations and mensage and return
    }

    public void SetGuard(bool guard){ // setter para definir se o personagem esta defendendo
        OnGuard = guard;
        if(OnGuard)
            print(unitName + ": I'm On Guard\n");
    }

    public void Escape(){
        print(unitName + ": Trying to escape battle\n");
    }

    public void Dead(){
        isDead = true;
        //avisa o Battle_System que uma unidade morreu para checar o fim da batalha
        BS.TestBattleEnd();
    }
}

