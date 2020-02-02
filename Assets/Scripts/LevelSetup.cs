using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelSetup : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    [SerializeField] private Rect _levelBoundary;
    [SerializeField] private SpriteRenderer[] _deadZones;
    
    [SerializeField] private Vector2Int _craterGrid;
    [SerializeField] private ObjectRatio _crater;
    [SerializeField] private Vector2 _craterPositionJitter;
    
    [SerializeField] private Vector2Int _objectGrid;
    [SerializeField] private ObjectRatio[] _objectRatios;
    [SerializeField] private Vector2 _objectPositionJitter;

    List<GameObject> _spawnedObjects = new List<GameObject>(1024);

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
        if(newState == States.Begining)
        {
            if (_objectRatios.Sum(ratio => ratio.Ratio) > 1)
            {
                Debug.LogError("Some items will not be assigned because the object ratios are oversubscribed.");
            }
            RemoveSpawned();
            SetupMoonFeatures();
            SetupLevel();
        }
    }

    private void RemoveSpawned()
    {
        foreach(var spawned in _spawnedObjects){
            Destroy(spawned);
        }
        _spawnedObjects.Clear();
    }

    private void SetupMoonFeatures()
    {
        //Calculate our cell sizes.
        var cellSize = new Vector2(_levelBoundary.width / _craterGrid.x, _levelBoundary.height / _craterGrid.y);
        
        //Randomly distribute craters around the level, removing any dead zones.
        var gridPositions = GetShuffledCellQueue(_craterGrid, cellSize);
        var availableSpaces = gridPositions.Count;
        
        //Create craters in final positions.
        CreatePrefabs(gridPositions, availableSpaces, _craterPositionJitter, _crater, cellSize, false);
    }
    
    private void SetupLevel()
    {
        //Calculate our sizes.
        var cellSize = new Vector2(_levelBoundary.width / _objectGrid.x, _levelBoundary.height / _objectGrid.y);
        
        //Randomly distribute objects around the level.
        var gridPositions = GetShuffledCellQueue(_objectGrid, cellSize);
        var availableSpaces = gridPositions.Count;

        //Create objects in final positions.
        foreach (var objectRatio in _objectRatios)
        {
            CreatePrefabs(gridPositions, availableSpaces, _objectPositionJitter, objectRatio, cellSize, true);
        }
    }
    
    //https://forum.unity.com/threads/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code.241052/
    private static void Shuffle<T>(IList<T> list)
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

    private void CreatePrefabs(Queue<Rect> grid, int availableSpaces, Vector2 jitter, ObjectRatio objectRatio, Vector2 cellSize, bool rotate)
    {
        var safeNumberToCreate = Mathf.Min(availableSpaces * objectRatio.Ratio, grid.Count);
        for (var i = 0; i < safeNumberToCreate; i++)
        {
            var cellCoordinates = grid.Dequeue();
            var xJitter = Random.Range(-jitter.x, jitter.x);
            var yJitter = Random.Range(-jitter.y, jitter.y);
            var x = cellCoordinates.x + xJitter + cellSize.x / 2;
            var y = cellCoordinates.y + yJitter - cellSize.y / 2;
            _spawnedObjects.Add( Instantiate(objectRatio.Object, new Vector3(x, y, 0), rotate ? Quaternion.Euler(0, 0, Random.value * 360) : Quaternion.identity));
        }
    }

    private Queue<Rect> GetShuffledCellQueue(Vector2Int grid, Vector2 cellSize)
    {
        var validCells = new List<Rect>();
        for (var x = 0; x < grid.x; x++)
        {
            for (var y = 0; y < grid.y; y++)
            {
                var cell = new Rect(_levelBoundary.x + x * cellSize.x, _levelBoundary.y - y * cellSize.y, cellSize.x, cellSize.y);
                var isInDeadZone = false;
                foreach (var deadZone in _deadZones)
                {
                    var bounds = deadZone.bounds;
                    var rocketBounds = new Rect(bounds.min, bounds.size);
                    if (rocketBounds.Overlaps(cell))
                    {
                        isInDeadZone = true;
                    }
                }
                if (!isInDeadZone)
                {
                    validCells.Add(cell);
                }
            }
        }
        Shuffle(validCells);
        return new Queue<Rect>(validCells);
    }
}