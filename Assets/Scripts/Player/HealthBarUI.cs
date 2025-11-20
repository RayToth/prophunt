using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void UpdateBar(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
}