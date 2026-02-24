using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float fadeSpeed = 2f;

    private TextMeshProUGUI text;
    private Color textColor;

    void Awake()
    {
        // 🔥 หา Text ในตัวเองและลูก
        text = GetComponentInChildren<TextMeshProUGUI>();

        if (text == null)
        {
            Debug.LogError("TextMeshProUGUI not found in FloatingText prefab!");
            return;
        }

        textColor = text.color;
    }

    void Start()
    {
        Destroy(gameObject, 1f);
    }

    void Update()
    {
        if (text == null) return;

        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        textColor.a -= fadeSpeed * Time.deltaTime;
        text.color = textColor;
    }

    public void SetText(string value)
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();

        text.text = value;
    }
}