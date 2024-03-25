using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdvancedFeedbackRatingForm
{
    public class SSHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Texture2D texture2D;
        public Image image;
        public Button imageButton;
        public Button deleteButton;
        [SerializeField] private GameObject editTextObject;

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = Vector3.one * 1.1f;
            editTextObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
            editTextObject.SetActive(false);
        }

        void OnDestroy()
        {
            Destroy(texture2D);
        }
    }
}