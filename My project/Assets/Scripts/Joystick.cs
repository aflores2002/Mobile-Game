using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
    public class Joystick : MonoBehaviour, ISimpleInputDraggable
    {
        public SimpleInput.AxisInput xAxis = new SimpleInput.AxisInput("Horizontal");

        public float valueMultiplier = 1f;

        [Header("References")]
        public RectTransform knob; // The draggable inner oval

        [Header("Settings")]
        [Tooltip("Maximum horizontal distance the knob can move from the center")]
        public float horizontalLimit = 50f;

        private RectTransform rectTransform;
        private Vector2 initialKnobPosition;
        private Vector2 pointerDownPosition;
        private bool isDragging = false;

        private Vector2 m_value = Vector2.zero;
        public Vector2 Value { get { return m_value; } }

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            gameObject.AddComponent<SimpleInputDragListener>().Listener = this;

            if (knob == null)
            {
                Debug.LogError("Knob reference is missing!");
                return;
            }

            initialKnobPosition = knob.anchoredPosition;
        }

        private void OnEnable()
        {
            xAxis.StartTracking();
        }

        private void OnDisable()
        {
            xAxis.StopTracking();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out pointerDownPosition
            );
            CalculateInput(eventData, true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            CalculateInput(eventData, false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            m_value = Vector2.zero;
            xAxis.value = 0f;

            // Smoothly lerp back to center
            LeanTween.cancel(knob.gameObject);
            LeanTween.move(knob, initialKnobPosition, 0.1f).setEaseOutQuad();
        }

        private void CalculateInput(PointerEventData eventData, bool isInitial)
        {
            Vector2 currentPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out currentPosition
            );

            // If this is the initial touch, don't move the knob to the touch position
            if (isInitial) return;

            // Calculate the delta from the initial touch position
            Vector2 delta = currentPosition - pointerDownPosition;
            delta.y = 0; // Restrict to horizontal movement

            // Clamp the horizontal movement
            float clampedX = Mathf.Clamp(delta.x, -horizontalLimit, horizontalLimit);

            // Update knob position relative to its initial position
            Vector2 newPosition = initialKnobPosition + new Vector2(clampedX, 0);
            knob.anchoredPosition = newPosition;

            // Calculate input value (-1 to 1)
            float normalizedX = clampedX / horizontalLimit;
            m_value.x = normalizedX * valueMultiplier;

            // Update the input axis
            xAxis.value = m_value.x;
        }
    }
}