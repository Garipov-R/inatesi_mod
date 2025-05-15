using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities.Attacks
{
    public interface IInventory
    {
        //abstract bool Add(GameObject ent, bool makeactive = false);
        abstract bool Add(Carriable ent, bool makeactive = false);
        abstract bool Contains(GameObject ent);
        abstract int Count();
        abstract void DeleteContents();
        abstract bool Drop(GameObject ent);
        abstract GameObject DropActive();
        abstract int GetActiveSlot();
        abstract Carriable GetSlot(int i);
        abstract void OnChildAdded(GameObject child);
        abstract void OnChildRemoved(GameObject child);
        abstract bool SetActive(GameObject ent);
        abstract bool SetActiveSlot(int i, bool allowempty);
        abstract bool SwitchActiveSlot(int idelta, bool loop);
    }
}
