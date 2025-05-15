using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.Testing.InatesiArch
{
    public struct TimeSince
    {
        private float timeSince;

        public TimeSince(float ts)
        {
            timeSince = ts;
            //UnityEngine.Debug.Log("Timce ");
        }

        public static bool operator ==(TimeSince a, TimeSince b) 
        { 
            return a.Equals(b); 
        }

        public static bool operator !=(TimeSince a, TimeSince b) => !a.Equals(b);
        public static implicit operator float (TimeSince ts) => ts.timeSince;
        public static implicit operator TimeSince(float ts) { return new TimeSince(ts); }
        public override string ToString()
        {
            return timeSince.ToString();
        }
    }
}
