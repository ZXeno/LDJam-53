namespace DefaultNamespace
{
    using UnityEngine;

    public static class Utils
    {
        public static bool WithinThreshold(this float val, float threshHold)
        {
            return val <= threshHold && val >= -threshHold;
        }

        public static bool OutsideLimits(this float val, float boundary)
        {
            return val >= boundary || val <= -boundary;
        }

        public static bool WorldPositionIsInViewport(Vector3 vector)
        {
            Vector3 targetScreenPoint = Camera.main.WorldToScreenPoint(vector);

            Rect cameraRect = Camera.main.rect;
            Vector3 targetViewportPoint = Camera.main.ScreenToViewportPoint(targetScreenPoint);
            return cameraRect.Contains(targetViewportPoint);
        }
    }
}