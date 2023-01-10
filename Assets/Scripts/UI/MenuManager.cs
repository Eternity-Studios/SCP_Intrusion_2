using UnityEngine;

namespace UI
{
    [DisallowMultipleComponent]
    public class MenuManager : MonoBehaviour
    {
        public void Quit()
        {
            Application.Quit();
        }
    }
}