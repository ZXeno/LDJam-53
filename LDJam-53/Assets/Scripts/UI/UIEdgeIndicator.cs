using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEdgeIndicator : MonoBehaviour
{
    public Transform Player;
    public Transform Target;
    public int Xoffset = 20;
    public int Yoffset = 20;
    public bool updatepos = true;
    private Image sprite;
    private RectTransform uiCanvasRect;

    // Start is called before the first frame update
    void Start()
    {
        this.sprite = GetComponent<Image>();
        this.uiCanvasRect = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<RectTransform>();
    }

    void Update()
    {
        this.sprite.enabled = true;
        if (this.Target == null)
        {
            this.sprite.enabled = false;
            return;
        }

        if (this.Player == null)
        {
            this.sprite.enabled = false;
            return;
        }

        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(this.Player.position);
        Vector3 targetScreenPoint = Camera.main.WorldToScreenPoint(this.Target.position);

        Rect cameraRect = Camera.main.rect;
        Vector3 targetViewportPoint = Camera.main.ScreenToViewportPoint(targetScreenPoint);
        if (cameraRect.Contains(targetViewportPoint))
        {
            this.sprite.enabled = false;
            return;
        }

        // Get the direction towards the target
        Vector3 distance = targetScreenPoint - playerScreenPoint;

        // Calculate the angle to the target
        float angle = Mathf.Atan2 (distance.x, distance.y) * Mathf.Rad2Deg;

        // Apply our world rotation to match the angle to target (i.e. face it)
        this.gameObject.transform.eulerAngles = new Vector3 (0f, 0f, -angle);

        // if (this.sprite.enabled)
        // {
        //     Debug.Log($"{this.uiCanvasRect.rect}");
        // }

        // Position on screen
        // source: https://github.com/Omti90/Off-Screen-Target-Indicator-Tutorial/blob/main/Scripts/TargetIndicator.cs
        Vector3 indicatorPosition = new Vector3(targetScreenPoint.x, targetScreenPoint.y);

        //Calculate Center of Canvas and subtract from the indicator position to have indicatorCoordinates from the Canvas Center instead the bottom left!
        Vector3 canvasCenter = new Vector3(this.uiCanvasRect.rect.width / 2f, this.uiCanvasRect.rect.height / 2f, 0f) * this.uiCanvasRect.localScale.x;
        indicatorPosition -= canvasCenter;

        // Calculate if Vector to target intersects (first) with y border of canvas rect or if Vector intersects (first) with x border:
        // This is required to see which border needs to be set to the max value and at which border the indicator needs to be moved (up & down or left & right)
        float divX = (this.uiCanvasRect.rect.width / 2f - this.Xoffset) / Mathf.Abs(indicatorPosition.x);
        float divY = (this.uiCanvasRect.rect.height / 2f - this.Yoffset) / Mathf.Abs(indicatorPosition.y);

        // In case it intersects with x border first, put the x-one to the border and adjust the y-one accordingly (Trigonometry)
        if (divX < divY)
        {
            float signedAngle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);
            indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (this.uiCanvasRect.rect.width * 0.5f - this.Xoffset) * this.uiCanvasRect.localScale.x;
            indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * signedAngle) * indicatorPosition.x;
        }

        // In case it intersects with y border first, put the y-one to the border and adjust the x-one accordingly (Trigonometry)
        else
        {
            float signedAngle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);

            indicatorPosition.y = Mathf.Sign(indicatorPosition.y) * (this.uiCanvasRect.rect.height / 2f - this.Yoffset) * this.uiCanvasRect.localScale.y;
            indicatorPosition.x = -Mathf.Tan(Mathf.Deg2Rad * signedAngle) * indicatorPosition.y;
        }

        // Change the indicator Position back to the actual rectTransform coordinate system and return indicatorPosition
        this.transform.position = indicatorPosition + canvasCenter;
    }
}
