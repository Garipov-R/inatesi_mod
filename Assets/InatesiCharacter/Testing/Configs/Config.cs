using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.Configs
{
    public static class Config
    {
        public const string c_CharacterPath = "Prefabs/Characters/doomms 5";  //doomms 5      creep

        public const float c_AttackDelay = .5f;

        public const string c_SwayBobItemPath = "ItemSO/SwayBob/swaybob";

        public const string c_InputPath = "Prefabs/Inputs/[Input2]";
        public const string c_CameraPath = "Prefabs/Camera/[Camera]";

        public const string c_DefaultModel = "Models/Characters/DefaultModel/DefaultModel";



        public static LayerMask s_DamageLayerMask = LayerMask.GetMask( "Default", "CharacterHitCollider", "Player", "Character", "Platform");
        public static LayerMask s_DamageCharacterLayerMask = LayerMask.GetMask( "CharacterHitCollider", "Player", "Character");
        public static LayerMask s_DefaultLayerMask = LayerMask.GetMask("Default");
    }
}
