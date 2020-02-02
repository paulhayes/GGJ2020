using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCharacter : MonoBehaviour
{
    public const int AbsoluteMaximumCarriableItems = 5;
    public static RobotCharacter _instance;

    [SerializeField] RobotPlayerData _playerData;

    [SerializeField] GameState _gameState;

    [SerializeField] float _moveSpeed;

    [SerializeField] Rigidbody2D _body;

    [SerializeField] Ship _ship;

    [SerializeField] Transform _startPosition;

    [SerializeField] AudioSource _source;
    [SerializeField] LayerMask _itemMask;

    ItemSlot[] _itemSlots = new ItemSlot[AbsoluteMaximumCarriableItems];
    int _itemsHeld = 0;
    int _maxItems = 3;

    float _speedMultiplier = 1;


    void Awake()
    {
        for(int i=0;i<AbsoluteMaximumCarriableItems;i++){
            _itemSlots[i] = new ItemSlot();
        }
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
        transform.position = _startPosition.position;
    }

    void FixedUpdate()
    {
        Vector3 vel = Vector3.zero;

        if (_gameState.State==States.Countdown)
        {
             vel = (_ship.transform.position - transform.position).normalized * _moveSpeed;            
        }
        else if (_gameState.State==States.Scavenge)
        {
            var dx = Input.GetAxisRaw("Horizontal");
            var dy = Input.GetAxisRaw("Vertical");

            vel = new Vector3(dx, dy, 0).normalized * _moveSpeed * _speedMultiplier;
        }

        _body.velocity = 2*vel / (GetSumItemMass() +_body.mass);
    }

    void LateUpdate()
    {
        for(int i=0;i<_itemsHeld;i++)
        {
            var slot = _itemSlots[i];
            if (slot == null)
                break;

            slot.item.transform.position = transform.position + (Vector3)slot.dragOffset;
        }
    }

    void OnStateChanged(States oldState, States newState)
    {
        if(newState==States.Launch)
        {
            gameObject.SetActive(false);
        }
        else if(newState==States.Scavenge) 
        {
            gameObject.SetActive(true);
            gameObject.transform.position = _startPosition.position;
            ResetPowerUps();
        }
    }


    public void Pickup(Item item)
    {
        if (_itemsHeld >= _maxItems)
            return;

        item.OnPickedUp();

        _itemSlots[_itemsHeld].item = item;
        _itemSlots[_itemsHeld].dragOffset = item.transform.position-transform.position;

        _itemsHeld++;

        _source.Play();
    }

    public void BankItems(Ship ship)
    {
        Vector3 scoreDelta = Vector3.zero;

        for (int i = 0; i <_itemsHeld; i++)
        {
            Item item = _itemSlots[i].item;
            _itemSlots[i].item = null;

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

    float GetSumItemMass()
    {
        float sum = 0;
        for(int i=0;i<_itemsHeld;i++){
            sum += +_itemSlots[i].item.weight;
        }
        return sum;
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
                
                break;
            case PowerUpType.BatteryIncrease:
                _playerData.timeRemaining = Mathf.Clamp((_playerData.timeRemaining + _playerData.levelDuration / 3f), 0, _playerData.levelDuration);
                break;
        }
    }

    void ResetPowerUps()
    {
        _speedMultiplier = 1;
        _maxItems = 3;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_itemMask == (_itemMask | 1 << collision.gameObject.layer))
        {

            Item item = collision.gameObject.GetComponent<Item>();

            
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

[System.Serializable]
public class ItemSlot 
{
    public Vector2 dragOffset;
    public Item item;
}