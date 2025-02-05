using UnityEngine;
using UnityEngine.Events;

public class SwipeDetector : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float minSwipeDistance = 50f;
    public float maxSwipeTime = 0.5f;
    public float minVerticalRatio = 0.7f;

    [Header("Events")]
    public UnityEvent onUpwardSwipe;

    private Vector2 touchStart;
    private float touchStartTime;
    private bool isSwiping = false;

    void Update()
    {
        // Handle mobile touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartSwipe(touch.position);
                    break;

                case TouchPhase.Ended:
                    EndSwipe(touch.position);
                    break;
            }
        }
    }

    private void StartSwipe(Vector2 position)
    {
        touchStart = position;
        touchStartTime = Time.time;
        isSwiping = true;
    }

    private void EndSwipe(Vector2 position)
    {
        if (!isSwiping) return;
        isSwiping = false;

        // Calculate swipe properties
        float swipeTime = Time.time - touchStartTime;
        Vector2 swipeDelta = position - touchStart;

        // Check if swipe was fast enough
        if (swipeTime > maxSwipeTime) return;

        // Check if swipe was long enough
        float swipeDistance = swipeDelta.magnitude;
        if (swipeDistance < minSwipeDistance) return;

        // Calculate how vertical the swipe was (1 = perfectly vertical, 0 = perfectly horizontal)
        float verticalRatio = Mathf.Abs(swipeDelta.y) / swipeDistance;

        // Check if swipe was vertical enough and going upward
        if (verticalRatio >= minVerticalRatio && swipeDelta.y > 0)
        {
            onUpwardSwipe.Invoke();
        }
    }

    // Optional: Add debug visualization
    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUILayout.Label($"Swipe Detector Active");
            if (isSwiping)
            {
                GUILayout.Label($"Swiping in progress...");
            }
        }
    }
}