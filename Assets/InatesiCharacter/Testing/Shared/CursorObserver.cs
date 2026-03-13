using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    public class CursorObserver : MonoBehaviour
    {

        private void OnMouseDrag()
        {
            
#if UNITY_WEBGL
            if (G.IsPause)
            {
                G.SetVisibleCursor(true);
            }
            else
            {
                G.SetVisibleCursor(false);
            }
#endif
        }
        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                if (G.IsPause == false)
                {
                    G.SetVisibleCursor(false);
                }
                else
                {
                    G.SetVisibleCursor(true);
                }
            }
            else
            {
                G.SetVisibleCursor(true);
                //G.IsPause = true;
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                G.SetVisibleCursor(true);
                //G.IsPause = true;
            }
            else
            {
                if (G.IsPause == true)
                {
                    G.SetVisibleCursor(true);
                }
                else
                {
                    G.SetVisibleCursor(false);
                }
            }
        }
    }
}