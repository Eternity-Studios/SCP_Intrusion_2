using UnityEngine;

namespace UI
{
    public class TabsUI : MonoBehaviour
    {
        [HideInInspector]
        public int CurrentIndex;

        [SerializeField]
        GameObject[] tabs;

        private void OnEnable()
        {
            if (tabs.Length != 0)
                ActivateTab(0);
        }

        public void ActivateTab(int id)
        {
            foreach (GameObject go in tabs)
            {
                go.SetActive(false);
            }

            tabs[id].SetActive(true);

            CurrentIndex = id;
        }
    }
}
