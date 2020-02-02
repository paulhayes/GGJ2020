using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    bool _pickedUp = false;

    public float weight = 1;

    [SerializeField] Collider2D _collider;
    [SerializeField] SpriteRenderer _renderer;

    void Start()
    {
        //what was this for?
        //transform.localScale = Vector2.one;
    }

    public void OnPickedUp()
    {
        _collider.enabled = false;
        _pickedUp = true;
    }

    public Vector2 GetSize()
    {
        return _renderer.bounds.size;
    }
}
