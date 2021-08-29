using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldLoot : MonoBehaviour
{
    public int ID; // cada item deve ter um ID diferente para saber quais items devem ser deletados da scene de overworld toda vez que entra na scene
    [System.Serializable]
    public class Loot
    {
        public int amount;
        public int ID;
        public Loot(int ID, int amount){
            this.ID = ID;
            this.amount = amount;
        }
    }
    public Loot item;
}
