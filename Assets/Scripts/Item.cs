using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    bool _pickedUp = false;

    [SerializeField] Collider2D _collider;

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
}
