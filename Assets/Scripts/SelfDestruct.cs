using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public enum Method
    {
        Invoke,
        Startup
    }

    public enum Mode
    {
        Disable,
        Destroy
    }

    public Mode mode = Mode.Disable;
    public Method method = Method.Invoke;
    public float lifetime = 1;

    // Start is called before the first frame update
    void Start()
    {
        if(method == Method.Startup)
        {
            StartCoroutine(Countdown());
        }
    }


    public void setup(float t, Mode mode, Method method)
    {
        lifetime = t;
        this.mode = mode;
        this.method = method;
        if (method == Method.Startup) StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSecondsRealtime(lifetime);
        switch (mode)
        {
            case Mode.Disable:
                gameObject.SetActive(false); 
                break;
            case Mode.Destroy: 
                Destroy(gameObject);  
                break;
        }
    }

    public void StartCountdown()
    {
        StartCoroutine(Countdown());
    }
}
