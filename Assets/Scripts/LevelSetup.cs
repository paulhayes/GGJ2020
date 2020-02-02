using System.Collections.Generic;
using Moon;
using Parts;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LevelSetup : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    [SerializeField] private Transform _surface;
    [SerializeField] private Crater _craterPrefab;
    [SerializeField] private Rock _rockPrefab;
    [SerializeField] private EnginePart _enginePartPrefab;
    [SerializeField] private FuelPart _fuelPartPrefab;
    [SerializeField] private BodyPart _bodyPartPrefab;

    [SerializeField] private Rect _levelBoundary;
    [SerializeField] private Rect[] _deadZones;
    [SerializeField] private Vector2Int _craterGrid;
    [SerializeField] private int _maxCraters; //TODO: These maxes are the actual values at the moment, should we instead randomise within a range?
    [SerializeField] private Vector2Int _objectGrid;
    [FormerlySerializedAs("_maxMoonRocks")] [SerializeField] private int _maxRocks;
    [SerializeField] private int _maxEngineParts;
    [SerializeField] private int _maxFuelParts;
    [SerializeField] private int _maxBodyParts;
    [SerializeField] private Vector2 _craterPositionJitter;
    [SerializeField] private Vector2 _objectPositionJitter;

    private void OnEnable()
    {
        _gameState.StateChangedEvent += OnStateChange;
    }

    private void OnDisable()
    {
        _gameState.StateChangedEvent -= OnStateChange;
    }

    private void OnStateChange(States oldState, States newState)
    {
        if(newState == States.Scavenge)
        {
            SetupMoonFeatures();
            SetupLevel();
        }
    }

    private void SetupMoonFeatures()
    {
        //Calculate our cell sizes.
        var gridCellWidth = _levelBoundary.width / _craterGrid.x;
        var gridCellHeight = _levelBoundary.height / _craterGrid.y;

        //Randomly distribute craters around the level, removing any dead zones.
        var validCells = new List<Rect>();
        for (var x = 0; x < _craterGrid.x; x++)
        {
            for (var y = 0; y < _craterGrid.y; y++)
            {
                var cell = new Rect(_levelBoundary.x + x * gridCellWidth, _levelBoundary.y - y * gridCellHeight, gridCellWidth, gridCellHeight);
                var touchesDeadZone = false;
                foreach (var deadZone in _deadZones)
                {
                    if (deadZone.Overlaps(cell, true))
                    {
                        touchesDeadZone = true;
                    }
                }

                if (!touchesDeadZone)
                {
                    validCells.Add(cell);
                }
            }
        }
        Shuffle(validCells);
        var gridPositions = new Queue<Rect>(validCells);
        
        //Create craters in final positions.
        CreatePrefabs(_maxCraters, gridPositions, _craterPositionJitter, _craterPrefab, _craterPrefab.Size);
    }
    
    private void SetupLevel()
    {
        //Calculate our sizes.
        var gridCellWidth = _levelBoundary.width / _objectGrid.x;
        var gridCellHeight = _levelBoundary.height / _objectGrid.y;
        
        //Randomly distribute objects around the level.
        var validCells = new List<Rect>();
        for (var x = 0; x < _objectGrid.x; x++)
        {
            for (var y = 0; y < _objectGrid.y; y++)
            {
                var cell = new Rect(_levelBoundary.x + x * gridCellWidth, _levelBoundary.y - y * gridCellHeight, gridCellWidth, gridCellHeight);
                var touchesDeadZone = false;
                foreach (var deadZone in _deadZones)
                {
                    if (deadZone.Overlaps(cell, true))
                    {
                        touchesDeadZone = true;
                    }
                }

                if (!touchesDeadZone)
                {
                    validCells.Add(cell);
                }
            }
        }
        Shuffle(validCells);
        var gridPositions = new Queue<Rect>(validCells);

        //Create objects in final positions.
        CreatePrefabs(_maxRocks, gridPositions, _objectPositionJitter, _rockPrefab, _rockPrefab.Size);
        CreatePrefabs(_maxBodyParts, gridPositions, _objectPositionJitter, _bodyPartPrefab, _bodyPartPrefab.Size);
        CreatePrefabs(_maxEngineParts, gridPositions, _objectPositionJitter, _enginePartPrefab, _enginePartPrefab.Size);
        CreatePrefabs(_maxFuelParts, gridPositions, _objectPositionJitter, _fuelPartPrefab, _fuelPartPrefab.Size);
    }
    
    //https://forum.unity.com/threads/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code.241052/
    public static void Shuffle<T>(IList<T> list)
    {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    private static void CreatePrefabs(int numberToCreate, Queue<Rect> grid, Vector2 jitter, MonoBehaviour prefab, Vector2 prefabSize)
    {
        var safeNumberToCreate = Mathf.Min(numberToCreate, grid.Count);
        for (var i = 0; i < safeNumberToCreate; i++)
        {
            var cellCoordinates = grid.Dequeue();
            var xJitter = Random.Range(-jitter.x, jitter.x);
            var yJitter = Random.Range(-jitter.y, jitter.y);
            var x = cellCoordinates.x + xJitter + prefabSize.x / 2;
            var y = cellCoordinates.y + yJitter + prefabSize.y / 2;
            Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
        }
    }
}