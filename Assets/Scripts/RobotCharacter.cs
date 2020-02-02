using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCharacter : MonoBehaviour
{
    public static RobotCharacter _instance;

    [SerializeField] RobotPlayerData _playerData;

    [SerializeField] GameState _gameState;

    [SerializeField] float _moveSpeed;

    [SerializeField] Rigidbody2D _body;

    [SerializeField] Ship _ship;

    [SerializeField] Transform _startPosition;

    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip _partPickupSfx;

    [SerializeField] LayerMask _itemMask;

    bool _inputEnabled = true;

    Item[] _itemSlots;
    int _itemsHeld;
    int _maxItems = 3;

    float _speedMultiplier = 1;


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
        _gameState.StateChangedEvent += OnStateChanged;
    }

    public void Reset()
    {
        _inputEnabled = true;
        transform.position = _startPosition.position;
    }

    void FixedUpdate()
    {
        Vector3 vel = Vector3.zero;

        if (_gameState.State==States.Countdown)
        {
             vel = (_ship.transform.position - transform.position).normalized * _moveSpeed;            
        }
        else if (_inputEnabled)
        {
            var dx = Input.GetAxisRaw("Horizontal");
            var dy = Input.GetAxisRaw("Vertical");

            vel = new Vector3(dx, dy, 0).normalized * _moveSpeed * _speedMultiplier;
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

    void OnStateChanged(States oldState, States newState)
    {
        _inputEnabled = newState==States.Scavenge;
        if(newState==States.Launch)
        {
            gameObject.SetActive(false);
        }
        else if(newState==States.Scavenge) 
        {
            gameObject.SetActive(true);
        }
    }


    public void Pickup(Item item)
    {
        if (_itemsHeld >= _itemSlots.Length)
            return;

        item._pickedUp = true;
        item._dragOffset = item.transform.position-transform.position;

        item._collider.enabled = false;

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

            if (item is PartItem)
            {
                PartItem partItem = (PartItem)item;

                scoreDelta += partItem._part.values;
            }
            else if (item is PowerUpItem)
            {
                PowerUpItem powUpItem = (PowerUpItem)item;

                ApplyPowerUp(powUpItem);
                            
            }
            
            // get destroyed by the spawner at the begining of next level
            item.gameObject.SetActive(false);    
        }

        ship.AddScore(scoreDelta);

        _itemsHeld = 0;
    }

    void ApplyPowerUp(PowerUpItem powUpItem)
    {
        switch (powUpItem._type)
        {
            case PowerUpType.Speed:
                _speedMultiplier+=0.5f;
                break;
            case PowerUpType.SlotIncrease:
                _maxItems++;

                Item[] newSlots = new Item[_maxItems];
                for (int i = 0; i < _itemsHeld; i++)
                {
                    newSlots[i] = _itemSlots[i];
                }

                _itemSlots = newSlots;

                break;
            case PowerUpType.BatteryIncrease:
                _playerData.timeRemaining = Mathf.Clamp((_playerData.timeRemaining + _playerData.levelDuration / 3f), 0, _playerData.levelDuration);
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_itemMask == (_itemMask | 1 << collision.gameObject.layer))
        {

            Item item = collision.gameObject.GetComponent<Item>();

            if (item._pickedUp)
                return;

            Pickup(item);
        }

         
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(_gameState.State==States.Countdown) 
        {
            bool collidedWithShip = collision.gameObject.GetComponent<Ship>()!=null;
            if(collidedWithShip){
                Debug.Log("Reached ship");
                _gameState.State = States.Launch;
            }
        }
    }
}
