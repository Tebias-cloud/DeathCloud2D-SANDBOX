using UnityEngine;
using UnityEngine.EventSystems;

namespace DeathCloud.UI
{
    public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float transitionSpeed = 10f;
        
        private Vector3 originalScale;
        private Vector3 targetScale;

        private void Start()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;
        }

        private void Update()
        {
            // Transición suave del tamaño
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetScale = originalScale * hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = originalScale;
        }

        private void OnDisable()
        {
            // Resetear escala si se desactiva el panel
            transform.localScale = originalScale;
        }
    }
}
