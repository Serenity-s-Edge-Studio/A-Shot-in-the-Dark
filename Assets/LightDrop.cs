using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightDrop : MonoBehaviour
{
    [SerializeField]
    private Light2D light;
    [SerializeField]
    private float decayTime;
    [SerializeField]
    private float decaySpeed;
    private float remainingTime;
    public Rigidbody2D rigidbody;
    private void Start()
    {
        remainingTime = decayTime;
        rigidbody = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        remainingTime = Mathf.Max(0, remainingTime - Time.deltaTime * decaySpeed);
        if (remainingTime < .01f) Destroy(gameObject);
        light.intensity = remainingTime / decayTime;
    }
}
