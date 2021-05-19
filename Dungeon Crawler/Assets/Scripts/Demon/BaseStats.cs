﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/**
* Classe que guarda os valores dos atributos base de todos os demônios do jogos
* Os Atributos de cada demônio do jogo são calculados a partir de seus niveis e de seus atributos base
*/
public class BaseStats {
    public enum TYPE {NORMAL, FIRE, GRASS, WATER, AIR, DARK, LIGHT, ICE, ROCK, POISON, DRAGON, BUG};
    public enum GROWTH_RATE {SLOW, MEDIUM_SLOW, MEDIUM, FAST};
    public struct DemonStats{
        private readonly string SPECIES;
        private readonly int HP;
        private readonly int MP;
        private readonly int attack;
        private readonly int defence;
        private readonly int special_attack;
        private readonly int special_defence;
        private readonly int speed;

        private readonly TYPE type;

        private readonly int catch_rate;
        private readonly int xp_yield;
        private readonly GROWTH_RATE growth_rate;

        public DemonStats (string SPECIES, int HP, int MP, int attack, int defence, int special_attack, 
                           int special_defence, int speed, TYPE type,int catch_rate, int xp_yield, GROWTH_RATE growth_rate)
        {
            this.SPECIES = SPECIES;
            this.HP = HP;
            this.MP = MP;
            this.attack = attack;
            this.defence = defence;
            this.special_attack = special_attack;
            this.special_defence = special_defence;
            this.speed = speed;
            this.type = type;
            this.catch_rate = catch_rate;
            this.xp_yield = xp_yield;
            this.growth_rate = growth_rate;
        }

        public string Species { get { return SPECIES; } }
        public int Hp { get { return HP; } }
        public int Mp { get { return MP; } }
        public int atk { get { return attack; } }
        public int def { get { return defence; } }
        public int spatk { get { return special_attack; } }
        public int spdef { get { return special_defence; } }
        public int spd { get { return speed; } }
        public TYPE Type{ get { return type; } }
        public int catchRate { get { return catch_rate; } }
        public int xpYield { get { return xp_yield; } }
        public GROWTH_RATE growthRate { get { return growth_rate; } }
    }
    
    public static readonly IList<DemonStats> DemonDex = new ReadOnlyCollection<DemonStats>
        (new[] {
            new DemonStats (/*name*/ "Fire Snake",
                             /*HP*/    39,
                             /*MP*/    25, 
                             /*ATK*/   52, 
                             /*DEF*/   43, 
                             /*SPATK*/ 60, 
                             /*SPDEF*/ 50, 
                             /*SPEED*/ 65,
                             /*TYPE*/  TYPE.FIRE,
                             /*CATCH*/ 45,
                             /*EXP*/   65,
                             /*GROWTH*/GROWTH_RATE.MEDIUM_SLOW),

            new DemonStats (/*name*/ "Fire Kindred Flame",
                             /*HP*/    58,
                             /*MP*/    40, 
                             /*ATK*/   64, 
                             /*DEF*/   58, 
                             /*SPATK*/ 80, 
                             /*SPDEF*/ 65, 
                             /*SPEED*/ 80,
                             /*TYPE*/  TYPE.FIRE,
                             /*CATCH*/ 45,
                             /*EXP*/   142,
                             /*GROWTH*/GROWTH_RATE.MEDIUM_SLOW),

            new DemonStats (/*name*/ "Fire Dragonspawn",
                             /*HP*/    78,
                             /*MP*/    70, 
                             /*ATK*/   84, 
                             /*DEF*/   78, 
                             /*SPATK*/ 109, 
                             /*SPDEF*/ 85, 
                             /*SPEED*/ 100,
                             /*TYPE*/  TYPE.FIRE,
                             /*CATCH*/ 45,
                             /*EXP*/   209,
                             /*GROWTH*/GROWTH_RATE.MEDIUM_SLOW),
            
            new DemonStats (/*name*/ "Toxic Carnivorous Plant A",
                             /*HP*/    45,
                             /*MP*/    50, 
                             /*ATK*/   49, 
                             /*DEF*/   49, 
                             /*SPATK*/ 65, 
                             /*SPDEF*/ 65, 
                             /*SPEED*/ 45,
                             /*TYPE*/  TYPE.GRASS,
                             /*CATCH*/ 45,
                             /*EXP*/   64,
                             /*GROWTH*/GROWTH_RATE.MEDIUM_SLOW),

            new DemonStats (/*name*/ "Toxic Carnivorous Plant B",
                             /*HP*/    60,
                             /*MP*/    70, 
                             /*ATK*/   62, 
                             /*DEF*/   63, 
                             /*SPATK*/ 80, 
                             /*SPDEF*/ 80, 
                             /*SPEED*/ 60,
                             /*TYPE*/  TYPE.GRASS,
                             /*CATCH*/ 45,
                             /*EXP*/   141,
                             /*GROWTH*/GROWTH_RATE.MEDIUM_SLOW),

            new DemonStats (/*name*/ "Toxic Carnivorous Plant C",
                             /*HP*/    80,
                             /*MP*/    85, 
                             /*ATK*/   82, 
                             /*DEF*/   83, 
                             /*SPATK*/ 100, 
                             /*SPDEF*/ 100, 
                             /*SPEED*/ 80,
                             /*TYPE*/  TYPE.GRASS,
                             /*CATCH*/ 45,
                             /*EXP*/   208,
                             /*GROWTH*/GROWTH_RATE.MEDIUM_SLOW),
            
            new DemonStats (/*name*/ "Sea Crab",
                             /*HP*/    44,
                             /*MP*/    35, 
                             /*ATK*/   48, 
                             /*DEF*/   65, 
                             /*SPATK*/ 50, 
                             /*SPDEF*/ 64, 
                             /*SPEED*/ 43,
                             /*TYPE*/  TYPE.WATER,
                             /*CATCH*/ 45,
                             /*EXP*/   66,
                             /*GROWTH*/GROWTH_RATE.MEDIUM_SLOW),

            new DemonStats (/*name*/ "Spider Crab",
                             /*HP*/    59,
                             /*MP*/    50, 
                             /*ATK*/   63, 
                             /*DEF*/   80, 
                             /*SPATK*/ 65, 
                             /*SPDEF*/ 80, 
                             /*SPEED*/ 58,
                             /*TYPE*/  TYPE.WATER,
                             /*CATCH*/ 45,
                             /*EXP*/   143,
                             /*GROWTH*/GROWTH_RATE.MEDIUM_SLOW),

            new DemonStats (/*name*/ "Shell Claw",
                             /*HP*/    79,
                             /*MP*/    70, 
                             /*ATK*/   83, 
                             /*DEF*/   100, 
                             /*SPATK*/ 85, 
                             /*SPDEF*/ 105, 
                             /*SPEED*/ 78,
                             /*TYPE*/  TYPE.WATER,
                             /*CATCH*/ 45,
                             /*EXP*/   210,
                             /*GROWTH*/GROWTH_RATE.MEDIUM_SLOW),
            
            new DemonStats (/*name*/ "Rabbit Bandit",
                             /*HP*/    30,
                             /*MP*/    15, 
                             /*ATK*/   56, 
                             /*DEF*/   35, 
                             /*SPATK*/ 25, 
                             /*SPDEF*/ 35, 
                             /*SPEED*/ 72,
                             /*TYPE*/  TYPE.NORMAL,
                             /*CATCH*/ 255,
                             /*EXP*/   57,
                             /*GROWTH*/GROWTH_RATE.MEDIUM),

            new DemonStats (/*name*/ "Rabbit Knight",
                             /*HP*/    55,
                             /*MP*/    25, 
                             /*ATK*/   81, 
                             /*DEF*/   60, 
                             /*SPATK*/ 50, 
                             /*SPDEF*/ 70, 
                             /*SPEED*/ 97,
                             /*TYPE*/  TYPE.NORMAL,
                             /*CATCH*/ 127,
                             /*EXP*/   116,
                             /*GROWTH*/GROWTH_RATE.MEDIUM)
        });
    /**
    * Retorna DemonStats pelo nome da espécie do demônio
    */
    public static DemonStats SearchDex(string demonName){
        for (int i = 0; i < DemonDex.Count; i++)
        {
            if(DemonDex[i].Species == demonName){
                return DemonDex[i];
            }
        }
        return DemonDex[0];
    }
}