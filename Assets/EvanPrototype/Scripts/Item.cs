using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public bool _pickedUp = false;
    public Part _part;

    [SerializeField] PartType _partType;

    [SerializeField] LayerMask _canCollect;

    [SerializeField] Rigidbody2D _rb;
    [SerializeField] TextMeshPro _scoreText;

    void Awake()
    {

        _scoreText.text = _part.values[(int)_partType].ToString("0.0");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector2 vel)
    {
        _rb.velocity = vel;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_pickedUp ||
            _canCollect != (_canCollect | 1 << collision.gameObject.layer))
            return;

        Player._instance.Pickup(this);
    }
}
