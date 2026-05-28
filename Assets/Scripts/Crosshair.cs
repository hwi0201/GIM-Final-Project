using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Image dot;
    public Color normalColor = Color.white;

    void Start()
    {
        SetColor(normalColor);
    }

    public void SetColor(Color color)
    {
        if (dot != null) dot.color = color;
    }
}
