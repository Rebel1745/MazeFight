using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopupAnimation : MonoBehaviour
{
    public AnimationCurve OpacityCurve;
    public AnimationCurve ScaleCurve;
    public AnimationCurve HeightCurve;

    private TextMeshProUGUI tmp;
    private float time = 0f;
    private Vector3 origin;

    private Camera cam;

    private void Awake()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        origin = transform.position;

        cam = Camera.main;
    }

    void Update()
    {
        tmp.color = new Color(1, 1, 1, OpacityCurve.Evaluate(time));
        transform.localScale = Vector3.one * ScaleCurve.Evaluate(time);
        transform.position = origin + new Vector3(0f, HeightCurve.Evaluate(time), 0f);
        time += Time.deltaTime;

        // face the camera
        transform.forward = cam.transform.forward;
    }
}
