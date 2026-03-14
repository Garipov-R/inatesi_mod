using InatesiCharacter.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Components
{
    public struct PlayerComponent
    {
        public GameObject gameObject;
        public CameraMotion cameraMotion;
        public bool fpc;
        public bool inputEnabled;
        public UnityEngine.UIElements.UIDocument uiDocument;
    }
}
