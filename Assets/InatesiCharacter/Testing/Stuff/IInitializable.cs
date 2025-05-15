using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.Testing.Stuff
{
    public interface IInitializable
    {
        public EcsWorld World { get; set; }
    }
}
