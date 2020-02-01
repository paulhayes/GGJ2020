using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player _instance;

    [SerializeField] float _moveSpeed;
    [SerializeField] Rigidbody2D _rb;

    Item[] _itemSlots;
    int _itemsHeld;

    void Awake()
    {
        _itemSlots = new Item[3];

        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);
    }
    
    void Update()
    {

    }

    void FixedUpdate()
    {
        Move(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void Pickup (Item item)
    {
        if (_itemsHeld >= _itemSlots.Length)
            return;

        item._pickedUp = true;

        _itemSlots[_itemsHeld] = item;
        _itemsHeld++;
    }

    public void BankItems (Rocket rocket)
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            Item item = _itemSlots[i];
            _itemSlots[i] = null;

            if (item == null)
                continue;

            rocket.AddScore(item._part.values);

            Destroy(item.gameObject);
        }

        _itemsHeld = 0;
    }

    void Move (float x, float y)
    {
        var vel = new Vector2(x, y).normalized * _moveSpeed;
        _rb.velocity = vel;

        foreach (var item in _itemSlots)
        {
            if (item == null)
                break;

            item.Move(vel);
        }
    }
}
