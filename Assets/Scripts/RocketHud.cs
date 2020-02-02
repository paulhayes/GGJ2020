using UnityEngine;

public class RocketHud : MonoBehaviour
{
    [SerializeField] Rocket rocket;

    [SerializeField] Canvas canvas;

    Vector3 _hudRelativePos;

    void Start()
    {
        _hudRelativePos = canvas.transform.position - rocket.transform.position;
    }
    
    void Update()
    {
        canvas.transform.position = rocket.transform.position + _hudRelativePos;
        if( canvas.enabled != rocket.gameObject.activeSelf)
        {
            canvas.enabled = rocket.gameObject.activeSelf;
        }
    }
}