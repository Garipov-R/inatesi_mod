using InatesiCharacter.Testing.Character.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    [Serializable]
    public class CarriableObjectData
    {
        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private string _name = "default";
        [SerializeField] private int _Ammo = 30;
        [SerializeField] private int _TotalAmmo = 60;
        [SerializeField] private int _MagCapacity = 30;
        [SerializeField] private int _MaxAmmo = 99999;

        public int Ammo 
        { 
            get => _Ammo; 
            set 
            {
                _Ammo = Mathf.Clamp(value, 0, _MagCapacity);
            }
        }
        public string Name { get => _name; set => _name = value; }
        public int MagCapacity { get => _MagCapacity; set => _MagCapacity = value; }
        public int TotalAmmo 
        { 
            get => _TotalAmmo; 
            set
            {
                _TotalAmmo = Mathf.Clamp(value, 0, _MaxAmmo);
            }
        }
        public int MaxAmmo { get => _MaxAmmo; set => _MaxAmmo = value; }
        public WeaponType WeaponType { get => _weaponType; set => _weaponType = value; }

        public CarriableObjectData()
        {

        }

        public CarriableObjectData(CarriableObjectData other)
        {
            _Ammo = other.Ammo;
            _TotalAmmo = other.TotalAmmo;
            _MagCapacity = other.MagCapacity;
            _MaxAmmo = other.MaxAmmo;
            _name = other.Name;
            _weaponType = other.WeaponType;
        }
    }
}
