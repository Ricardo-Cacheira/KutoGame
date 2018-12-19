using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [System.Serializable]
 public class Ability{
 
     public int ID;
     public string name;
     public string description;
 
     public Ability(int ID, string name, string description){
         this.ID = ID;
         this.name = name;
         this.description = description;
     }
 }