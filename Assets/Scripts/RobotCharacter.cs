using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCharacter : MonoBehaviour
{
    public static RobotCharacter _instance;

    [SerializeField] float _moveSpeed;

    [SerializeField] Rigidbody2D _body;

    [SerializeField] Ship _ship;

    [SerializeField] Transform _startPosition;

    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip _partPickupSfx;

    bool _inputEnabled = true;
    bool _returnToShip = false;

    Item[] _itemSlots;
    int _itemsHeld;

    public void ReturnToShip () {
        _returnToShip = true;
        _inputEnabled = false;
    }

    public bool IsReturningToShip ()
    {
        return _returnToShip;
    }

    void Awake()
    {
        _itemSlots = new Item[3];

        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        _returnToShip = false;
        _inputEnabled = true;
        transform.position = _startPosition.position;
    }

    void FixedUpdate()
    {
        Vector3 vel = Vector3.zero;

        if (_returnToShip)
        {
            if (Vector2.Distance(transform.position, _ship.transform.position) > 0.1f)
                vel = (_ship.transform.position - transform.position).normalized;
            else
                _returnToShip = false;
        }
        else if (_inputEnabled)
        {
            var dx = Input.GetAxisRaw("Horizontal");
            var dy = Input.GetAxisRaw("Vertical");

            vel = new Vector3(dx, dy, 0) * _moveSpeed;
        }

        _body.velocity = vel;
    }

    void LateUpdate()
    {
        foreach (var item in _itemSlots)
        {
            if (item == null)
                break;

            item.transform.position = transform.position + (Vector3)item._dragOffset;
        }
    }

    public void Pickup(Item item)
    {
        if (_itemsHeld >= _itemSlots.Length)
            return;

        item._pickedUp = true;
        item._dragOffset = item.transform.position-transform.position;

        _itemSlots[_itemsHeld] = item;
        _itemsHeld++;

        _source.PlayOneShot(_partPickupSfx);
    }

    public void BankItems(Ship ship)
    {
        Vector3 scoreDelta = Vector3.zero;

        for (int i = 0; i < _itemSlots.Length; i++)
        {
            Item item = _itemSlots[i];
            _itemSlots[i] = null;

            if (item == null)
                continue;

            scoreDelta += item._part.values;

            Destroy(item.gameObject);
        }

        ship.AddScore(scoreDelta);

        _itemsHeld = 0;
    }
}
