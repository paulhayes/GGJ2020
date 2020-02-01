using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    [HideInInspector] public bool _pickedUp = false;

    [HideInInspector] public Vector2 _dragOffset;

    public Collider2D _collider;

    [SerializeField] LayerMask _canCollect;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector2.one;
    }
}
