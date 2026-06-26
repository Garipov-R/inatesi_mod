using UnityEngine;

namespace InatesiCharacter.Testing.Stuff
{
    public class Class1 : MonoBehaviour
    {
        public int hp = 100;


        void Start ()
        {
            int result = 0;
            switch(hp)
            {
                case 1:
                    result = 1;
                    break;

                case 2:
                    result = 2;
                    break;

                default:
                    result = 0;
                    break;
            }


            System.Console.WriteLine(result);
            Debug.Log(result);
        }
    }
}
