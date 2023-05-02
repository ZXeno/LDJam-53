using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;

public class UITextDisplayElement : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    public string PropertyName = "PropertyName";
    public string NumberFormat = "0.00";

    // Start is called before the first frame update
    void Start()
    {
        this.VerifyComponentNotNull();
        Player.PropertyChangedNotification += this.HandlePropertyChangeEvent;
    }

    public void SetText(string text)
    {
        this.VerifyComponentNotNull();
        this.textMesh.SetText(string.IsNullOrWhiteSpace(text) ? string.Empty : text);
    }

    private void HandlePropertyChangeEvent(object sender, PlayerPropertyChangedNotificationArgs e)
    {
        if (e.PropertyName != this.PropertyName)
        {
            return;
        }

        if (e.NewValue is float f)
        {
            this.textMesh.SetText(f.ToString(this.NumberFormat));
            return;
        }

        this.textMesh.SetText(e.NewValue.ToString());
    }

    private void VerifyComponentNotNull()
    {
        if (this.textMesh == null)
        {
            this.textMesh = this.gameObject.GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnDestroy()
    {
        Player.PropertyChangedNotification -= this.HandlePropertyChangeEvent;
    }
}
