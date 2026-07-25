using UnityEngine;

namespace GMTK.MainMenu
{

    public class MenuController : MonoBehaviour
    {

        public void QuitButton()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif

            Application.Quit();
        }
        
    }

    
}

