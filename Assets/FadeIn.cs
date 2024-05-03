using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public float totalTime = 2f;
    float step;

    Image img;

    private void Start()
    {
        img = GetComponent<Image>();
        step = 1f / totalTime * Time.deltaTime;
    }

    private void Update()
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Lerp(img.color.a, 0f, step));
        if (img.color.a < 0.1f) Destroy(gameObject);
    }
}
