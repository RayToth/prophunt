using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFader : MonoBehaviour
{
    public float fadeDuration = 1f;
    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Color c = img.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(1, 0, t / fadeDuration);
            img.color = c;
            yield return null;
        }
        c.a = 0;
        img.color = c;
        gameObject.SetActive(false);
    }
}
