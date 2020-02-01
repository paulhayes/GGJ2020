using System;
using System.Collections.Generic;
using System.Linq;
using Moon;
using Parts;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LevelSetup : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    
    [SerializeField] private Crater _craterPrefab;
    [SerializeField] private MoonRock _moonRockPrefab;
    [SerializeField] private EnginePart _enginePartPrefab;
    [SerializeField] private FuelPart _fuelPartPrefab;
    [SerializeField] private BodyPart _bodyPartPrefab;

    [SerializeField] private Rect _levelBoundary;
    [SerializeField] private int _maxCraters; //TODO: These maxes are the actual values at the moment, should we instead randomise within a range?
    [SerializeField] private int _maxMoonRocks;
    [SerializeField] private int _maxEngineParts;
    [SerializeField] private int _maxFuelParts;
    [SerializeField] private int _maxBodyParts;
    [SerializeField] private Vector2 _craterPositionJitter;
    [FormerlySerializedAs("_levelPositionJitter")] [SerializeField] private Vector2 _objectPositionJitter;

    private void Awake()
    {
        _gameState.StateChangedEvent += OnStateChange;
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
        //Calculate our sizes.
        var levelSize = new Vector2(_levelBoundary.width, _levelBoundary.height);
        var gridSizeX = (int)(levelSize.x / _craterPrefab.Size.x);
        var gridSizeY = (int) (levelSize.y / _craterPrefab.Size.y);
        var totalSize = gridSizeX * gridSizeY;
        Debug.Assert(totalSize > _maxCraters, "totalSize > _maxCraters"); //Avoid an index out of bounds.
        
        //Randomly distribute craters around the level.
        var flatCraterGrid = new List<bool>(totalSize);
        for (var i = 0; i < _maxCraters; ++i)
        {
            flatCraterGrid[i] = true;
        }
        flatCraterGrid.Sort((_1, _2) => Random.Range(-1, 1));
        
        //Create craters in final positions.
        for(var i = 0; i < totalSize; ++i)
        {
            if (!flatCraterGrid[i]) //No crater here, skip on. 
            {
                continue;
            }
            var x = i % gridSizeX + Random.Range(-_craterPositionJitter.x, _craterPositionJitter.x);
            var y = i - x + Random.Range(-_craterPositionJitter.y, _craterPositionJitter.y);
            Instantiate(_craterPrefab, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    private enum LevelObject
    {
        MoonRock,
        EnginePart,
        FuelPart,
        BodyPart,
        None
    }
    
    private void SetupLevel()
    {
        //Calculate our sizes.
        var levelSize = new Vector2(_levelBoundary.width, _levelBoundary.height);
        var gridSizeX = (int)(levelSize.x / Mathf.Max(_moonRockPrefab.Size.x, _enginePartPrefab.Size.x, _fuelPartPrefab.Size.x, _bodyPartPrefab.Size.x));
        var gridSizeY = (int)(levelSize.y / Mathf.Max(_moonRockPrefab.Size.y, _enginePartPrefab.Size.y, _fuelPartPrefab.Size.y, _bodyPartPrefab.Size.y));
        var totalSize = gridSizeX * gridSizeY;
        var totalObjects = _maxMoonRocks + _maxEngineParts + _maxFuelParts + _maxBodyParts;
        Debug.Assert(totalSize > totalObjects, "The max object values exceed the total size of the game grid"); //Warn and break to avoid an index out of bounds.
        
        //Randomly distribute objects around the level.
        var flatGrid = Concatenate(
            Enumerable.Range(0, _maxMoonRocks).Select(i => LevelObject.MoonRock),
            Enumerable.Range(0, _maxEngineParts).Select(i => LevelObject.EnginePart),
            Enumerable.Range(0, _maxFuelParts).Select(i => LevelObject.FuelPart),
            Enumerable.Range(0, _maxBodyParts).Select(i => LevelObject.BodyPart),
            Enumerable.Range(0, totalSize - totalObjects).Select(i => LevelObject.None))
            .ToList();
        flatGrid.Sort((_1, _2) => Random.Range(-1, 1));
        
        //Create objects in final positions.
        for(var i = 0; i < totalSize; ++i)
        {
            var x = i % gridSizeX + Random.Range(-_objectPositionJitter.x, _objectPositionJitter.x);
            var y = i - x + Random.Range(-_objectPositionJitter.y, _objectPositionJitter.y);
            switch (flatGrid[i]) //No crater here, skip on. 
            {
                case LevelObject.MoonRock:
                    Instantiate(_moonRockPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    break;
                case LevelObject.EnginePart:
                    Instantiate(_enginePartPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    break;
                case LevelObject.FuelPart:
                    Instantiate(_fuelPartPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    break;
                case LevelObject.BodyPart:
                    Instantiate(_bodyPartPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    break;
                case LevelObject.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] objects)
    {
        return objects.SelectMany(i => i);
    }
}