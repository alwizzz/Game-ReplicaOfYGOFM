using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PulseEffect : MonoBehaviour
{
    [SerializeField] private bool isPulsing;
    [SerializeField] private float pulseSpeed;
    [SerializeField] private float minAlpha;
    [SerializeField] private float maxAlpha;

    private float currentAlpha;
    private float currentAlphaNormalized;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        StartPulsing();
    }

    private void Update()
    {
        if (!isPulsing) return;
        Pulse();
    }

    private void Pulse()
    {
        currentAlpha = Mathf.PingPong(Time.time * pulseSpeed, maxAlpha - minAlpha) + minAlpha;
        currentAlphaNormalized = currentAlpha / 255;
        Color color = image.color;
        color.a = currentAlphaNormalized;
        image.color = color;
    }

    public void StartPulsing()
    {
        isPulsing = true;
        Reset();
    }

    public void StopPulsing()
    {
        isPulsing = false;
        Reset();
    }

    private void Reset()
    {
        Color color = image.color;
        color.a = maxAlpha;
        image.color = color;
    }


}
