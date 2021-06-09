using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/**
* Classe que guarda os valores dos atributos base de todos os demônios do jogos
* Os Atributos de cada demônio do jogo são calculados a partir de seus niveis e de seus atributos base
*/
public class Item {
    public enum TYPE {NORMAL, FIRE, GRASS, WATER, AIR, DARK, LIGHT, ICE, ROCK, POISON, DRAGON, BUG};
    public enum PRIORITY {LOW, NORMAL, HIGH, MAX};

    //Isso aqui tem que ser exatamente igual ao skillTarget Type para funcionar corretamente
    public enum TARGET_TYPE {SINGLE, ROW, PARTY, SINGLE_ALLY, ALLY_ROW, ALLY_PARTY};
    public enum STATUS_EFFECT {NULL, ATK_UP, ATK_DOWN, DEF_UP, DEF_DOWN, SPEED_UP, SPEED_DOWN,
                               POISON, BURN, FREEZE, PARALYSIS, CAPTURE}

    
    public struct ItemData{
        private readonly string name;
        private readonly TYPE type;
        private readonly TARGET_TYPE target_type;
        private readonly PRIORITY priority;
        private readonly int power;
        private readonly int accuracy;
        private readonly int cost;
        private readonly STATUS_EFFECT status_effect;
        private readonly bool isSpecial;
        private readonly bool isRanged;
        private readonly int skill_ID;
        private readonly string vfx;
        private readonly Color vfx_color;
        private readonly string description;
        public ItemData (string name, TYPE type, TARGET_TYPE target_type, PRIORITY priority, int power, int accuracy, int cost,
                          STATUS_EFFECT status_effect, bool isSpecial, bool isRanged, int skill_ID, string vfx, Color vfx_color, string description)
        {
            this.name = name;
            this.type = type;
            this.target_type = target_type;
            this.priority = priority;
            this.power = power;
            this.accuracy = accuracy;
            this.cost = cost;
            this.status_effect = status_effect;
            this.isSpecial = isSpecial;
            this.isRanged = isRanged;
            this.skill_ID = skill_ID;
            this.vfx = vfx;
            this.vfx_color = vfx_color;
            this.description = description;
        }
        public string Name { get { return name; } }
        public TYPE Type { get { return type; } }
        public TARGET_TYPE Target_type { get { return target_type; } }
        public PRIORITY Priority { get { return priority; } }
        public int Power { get { return power; } }
        public int Accuracy { get { return accuracy; } }
        public int Cost { get { return cost; } }//Custo de itens devem sempre ser zero
        public STATUS_EFFECT Status_effect { get { return status_effect; } }
        public bool IsSpecial { get { return isSpecial; } }
        public bool IsRanged { get { return isRanged; } }//Deve sempre ser ranged
        public string VFX { get { return vfx; } }
        public Color COLOR { get { return vfx_color; } }
        public string Description { get { return description; } }
    }
    
    public static readonly IList<ItemData> ItemList= new ReadOnlyCollection<ItemData>
        (new[] {
            new ItemData (/*name*/        "Potion", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE_ALLY, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -40, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           0,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Healing Potion that recovers a small amount of HP."),

            new ItemData (/*name*/        "H_Potion", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.ALLY_ROW, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -20, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           1,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Medicine that recovers a small amount HP of an entire row."),

            new ItemData (/*name*/        "GreatPotion", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE_ALLY, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -60, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           2,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Medicine that recovers a great amount of HP."),

            new ItemData (/*name*/        "H_GreatPotion", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.ALLY_ROW, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -30, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           3,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Medicine that recovers a great amount HP of an entire row."),
            
            new ItemData (/*name*/        "UltraPotion", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE_ALLY, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -80, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           4,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Medicine that recovers a large amount of HP."),

            new ItemData (/*name*/        "H_UltraPotion", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.ALLY_ROW, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        -40, 
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*status_effect*/STATUS_EFFECT.NULL, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           5,
                           /*VFX*/          "Heal",
                           /*VFX_COLOR*/    new Color(0.6f, 1.0f, 0.92f, 1.0f),
                           /*DESC*/         "Medicine that recovers a great amount HP of an entire row."),
            
            new ItemData (/*name*/        "DemonSeal", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        10, //nesse caso o poder se torna o bonus na chance de captura vezes 10
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*status_effect*/STATUS_EFFECT.CAPTURE, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           6,
                           /*VFX*/          "Barrier",
                           /*VFX_COLOR*/    new Color(0.25f, 0.75f, 0.96f, 1f),
                           /*DESC*/         "A tool for taming Demons."),
            
            new ItemData (/*name*/        "QuickDemonSeal", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.HIGH, 
                           /*power*/        10, //nesse caso o poder se torna o bonus na chance de captura vezes 10
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*status_effect*/STATUS_EFFECT.CAPTURE, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           7,
                           /*VFX*/          "Barrier",
                           /*VFX_COLOR*/    Color.red,
                           /*DESC*/         "A tool with high priority for taming Demons."),
            
            new ItemData (/*name*/        "GreatDemonSeal", 
                           /*type*/         TYPE.NORMAL, 
                           /*target_type*/  TARGET_TYPE.SINGLE, 
                           /*priority*/     PRIORITY.NORMAL, 
                           /*power*/        10, //nesse caso o poder se torna o bonus na chance de captura vezes 10
                           /*accuracy*/     100, 
                           /*cost*/         0,
                           /*status_effect*/STATUS_EFFECT.CAPTURE, 
                           /*isSpecial*/    true, 
                           /*isRanged*/     true,
                           /*ID*/           8,
                           /*VFX*/          "Barrier",
                           /*VFX_COLOR*/    new Color(0.25f, 0.75f, 0.96f, 1f),
                           /*DESC*/         "A tool with a decent success rate for taming Demons.")
        });
}