using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public bool _pickedUp = false;
    public Part _part;

    public Vector2 _dragOffset;

    [SerializeField] PartType _partType;

    [SerializeField] LayerMask _canCollect;

    [SerializeField] Rigidbody2D _rb;
    //[SerializeField] TextMeshPro _scoreText;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector2.one;// * _part.values[(int)_partType];
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

        RobotCharacter._instance.Pickup(this);
    }
}
