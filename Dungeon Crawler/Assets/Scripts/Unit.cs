using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;

    public int maxHP;
    public int currentHP;

    public int maxMana;
    public int currentMana;

    private Unit Target;

    private BattleAction Move;

    public void TakeDamage(int damage){
        currentHP = Math.Min(0, currentHP - damage);
        //DisplayDamage(damage);
        //Is_Dead()?
    }
}

