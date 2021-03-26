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

    public bool isPlayerUnit;

    public bool isDead = true;

    public BattleAction Move;
    public GameObject SelectionArrow; //usado apenas para unidades inimiga para mostrar qual esta sendo selecionada
    public EnemyBattleAI AI; //used only for NonPlayableUnits
    private bool OnGuard = false; // devo resetar isso alguma hora depois do BattleTurn,
                                 // provavelmente quando o personagem for selecionado para escolher a sua proxima ação

    public void TakeDamage(int damage){
        string mensage = "";
        if (!OnGuard){
            currentHP = Math.Max(0, currentHP - damage);
            mensage = unitName + ": AHHHHH I took " + damage + "\n I have " + currentHP + " remaining. \n";
        }
        else{
            currentHP = Math.Max(0, currentHP - damage/2);//como placeholder, se o personagem estiver defendendo leva metade do dano
            mensage = unitName + ": AHHHHH I took " + damage/2 + "\n I have " + currentHP + " remaining. \n";
        }
        print(mensage);
        //play action animation
        //play soundfx
        //display mensage
        //wait for animations and mensage and return
    }

    public void SetGuard(bool guard){ // setter para definir se o personagem esta defendendo
        OnGuard = guard;
        print(unitName + ": I'm On Guard\n");
    }

    public void Escape(){
        print(unitName + ": Trying to escape battle\n");
    }

    public void IsSelected(bool selected){//used only for enemy units
        SelectionArrow.SetActive(selected);
    }
}

