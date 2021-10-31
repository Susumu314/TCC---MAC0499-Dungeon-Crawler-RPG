using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Demonomicon : MonoBehaviour
{
    [System.Serializable]
    public class SavedDemon
    {
        //por enquanto isso aqui vai ser tudo public, depois dos testes trocar para private readonly
        public string species;
        public int totalExp;
        public string nickname;
        public int[] skills;

        public SavedDemon(string species, int totalExp, string nickname, int[] skills){
            this.species = species;
            this.totalExp = totalExp;
            this.nickname = nickname;
            this.skills = skills;
        }
        
        public string SPECIES{ get { return species; } }
        public int TotalExp{ get { return totalExp; } }
        public string Nickname{ get { return nickname; } }
        public int[] Skills{ get { return skills; } }
    }

    public List<SavedDemon> demonomicon = new List<SavedDemon>(); 

    /**
    * Adiciona demonio ao demonomicon
    * @param unit Unidade a ser adicionada ao demonomicon
    */
    public void AddDemon(Unit unit){
        demonomicon.Add(new SavedDemon(unit.species, unit.totalExp, unit.unitName, unit.skillList));
    }

    /**
    * Remove demonio ao demonomicon
    * @param demon Unidade a ser removida do demonomicon
    */
    public void RemoveDemon(SavedDemon demon){
        demonomicon.Remove(demon);
    }
}
