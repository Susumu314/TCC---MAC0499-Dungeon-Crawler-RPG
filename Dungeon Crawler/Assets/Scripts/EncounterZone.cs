using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Classe que armazena as informações de cada área que é possível se encontrar um grupo de demônios inimigos
*/

public class EncounterZone : MonoBehaviour
{   
    /**
    * O quanto andar na área incrementa o encounter counter
    */
    public int EncounterRate = 0; 

    /**
    * ID utilizado na Encounter Table para verificar quais Encounters são possíveis na área
    */
    public string ZoneID = "OverWorldTest_0";
}
