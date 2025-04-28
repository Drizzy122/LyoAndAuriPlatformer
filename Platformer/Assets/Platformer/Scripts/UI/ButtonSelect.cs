using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Platformer
{
    public class ButtonSelect : MonoBehaviour
    {
        [SerializeField] private Button primaryButton;
        
        private void Awake()
        {
            SelectButton();
        }
        private void SelectButton()
        {
            // Ensure there is a primary button assigned
            if (primaryButton != null)
            {
                // Set the primary button as the selected UI element
                EventSystem.current.SetSelectedGameObject(primaryButton.gameObject);
            }
        }
        private void OnEnable()
        {
            SelectButton();
        }
    }
}