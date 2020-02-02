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

    [SerializeField] AudioSource wakeUpSFX;
    [SerializeField] LayerMask _itemMask;

    [SerializeField] float _itemTrailStart;
    [SerializeField] float _itemTrailPadding;

    [SerializeField] GameObject _emptySlotPfb;


    ItemSlot[] _itemSlots = new ItemSlot[AbsoluteMaximumCarriableItems];
    GameObject[] _emptySlotGfx = new GameObject[AbsoluteMaximumCarriableItems];

    int _itemsHeld = 0;
    int _maxItems = 3;

    float _speedMultiplier = 1;

    Vector3 _trailDir;

    void Awake()
    {
        _gameState.StateChangedEvent += OnStateChanged;
        
        for(int i=0;i<AbsoluteMaximumCarriableItems;i++){
            _itemSlots[i] = new ItemSlot();
            _emptySlotGfx[i] = Instantiate(_emptySlotPfb);
            _emptySlotGfx[i].SetActive(false);
        }
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _gameState.StateChangedEvent -= OnStateChanged;
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        transform.position = _startPosition.position;
    }

    void FixedUpdate()
    {
        Vector3 vel = Vector3.zero;

        if(_gameState.State==States.Begining)
        {
            _body.rotation = 180;
        }
        if (_gameState.State==States.Scavenge && IsReady())
        {
            _body.rotation = 0;
            var dx = Input.GetAxisRaw("Horizontal");
            var dy = Input.GetAxisRaw("Vertical");

            vel = new Vector3(dx, dy, 0).normalized * _moveSpeed * _speedMultiplier;

            if (Math.Abs(dx) > 0.001f || Math.Abs(dy) > 0.001f && IsReady())
            {
                _hasMoved = true;
            }
        }
        else if (_gameState.State==States.Countdown)
        {
             vel = (_ship.transform.position - transform.position).normalized * _moveSpeed;            
        }


        if (vel != Vector3.zero)
            _trailDir = -vel.normalized;

        _body.velocity = 2*vel / (GetSumItemMass() +_body.mass);
    }

    void LateUpdate()
    {
        //for(int i=0;i<_itemsHeld;i++)
        //{
        //    var slot = _itemSlots[i];
        //    if (slot == null)
        //        break;

        //    slot.item.transform.position = transform.position + (Vector3)slot.dragOffset;
        //}

        UpdateItemTrail();
    }

    void OnStateChanged(States oldState, States newState)
    {
        if(newState==States.Launch)
        {
            gameObject.SetActive(false);
            _isReady = false;
        }
        else if(newState==States.Begining) 
        {
            gameObject.SetActive(true);
            gameObject.transform.position = _startPosition.position;
        }
        else if (newState == States.Scavenge)
        {
            _hasMoved = false;
            StartCoroutine(GainConsciousness());
            ResetPowerUps();
            //Physics2D.SetLayerCollisionMask(gameObject.layer, _normalCollisionMask);
        }
        else if (newState == States.Countdown)
        {
            //Physics2D.SetLayerCollisionMask(gameObject.layer, _countdownCollisionMask);
        }
    }

    private IEnumerator GainConsciousness()
    {
        wakeUpSFX.Play();
        var flipVector = new Vector2(-1, 1);
        var wiggleFactor = 0.01f;
        gameObject.transform.localScale *= flipVector;
        yield return new WaitForSeconds(0.18f);
        gameObject.transform.localScale *= flipVector;
        yield return new WaitForSeconds(0.18f);
        gameObject.transform.localScale *= flipVector;
        yield return new WaitForSeconds(0.18f);
        gameObject.transform.position += Vector3.left * wiggleFactor;
        yield return new WaitForSeconds(0.06f);
        gameObject.transform.position += Vector3.right * wiggleFactor;
        yield return new WaitForSeconds(0.06f);
        gameObject.transform.position += Vector3.left * wiggleFactor;
        yield return new WaitForSeconds(0.06f);
        gameObject.transform.position += Vector3.right * wiggleFactor;
        yield return new WaitForSeconds(0.25f);
        gameObject.transform.localScale *= flipVector;
        gameObject.transform.rotation = Quaternion.identity;
        
        _isReady = true;
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

    private bool _hasMoved = false;
    void UpdateItemTrail()
    {
        Vector3 currentPos = transform.position + (_trailDir * _itemTrailStart);

        for (int i = 0; i < _maxItems; i++)
        {
            Transform slotTransform;

            float multiplier;

            if (_itemSlots[i].item == null)
            {
                _emptySlotGfx[i].SetActive(_gameState.State == States.Scavenge && IsReady() && _hasMoved);
                slotTransform = _emptySlotGfx[i].transform;

                multiplier = 0.15f;
            }
            else
            {
                _emptySlotGfx[i].SetActive(false);
                slotTransform = _itemSlots[i].item.transform;

                var size = _itemSlots[i].item.GetSize();

                multiplier = size.y;
            }



            currentPos += (_trailDir * multiplier * 0.5f) /*+ (_trailDir * _itemTrailPadding)*/;
            slotTransform.position = _emptySlotGfx[i].transform.position = Vector3.Lerp(slotTransform.position, currentPos, 0.025f);

            currentPos += (_trailDir * multiplier * 0.5f) /*+ (_trailDir * _itemTrailPadding)*/;
        }
    }

    void SetSlotPos ()
    {

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

    private bool _isReady;
    public bool IsReady()
    {
        return _isReady;
    }
}

[System.Serializable]
public class ItemSlot 
{
    public Vector2 dragOffset;
    public Item item;
}