using System;
using System.Collections.Generic;
using System.Linq;
using Moon;
using Parts;
using UnityEngine;
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
    [SerializeField] private int _maxCraters; //TODO: These maxes are the actual values at the moment, should we instead randomise within a range?
    [SerializeField] private int _maxMoonRocks;
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
        //Calculate our sizes.
        var gridWidth = (int)(_levelBoundary.width / _craterPrefab.Size.x);
        var gridHeight = (int) (_levelBoundary.height / _craterPrefab.Size.y);
        var totalSize = gridWidth * gridHeight;
        Debug.Assert(totalSize > _maxCraters, $"Total grid size ({totalSize}) must be greater than the number of craters ({_maxCraters})"); //Warn about an index out of bounds.
        
        //Randomly distribute craters around the level.
        var flatCraterGrid = Enumerable.Repeat(false, totalSize).ToList();
        for (var i = 0; i < _maxCraters; ++i)
        {
            flatCraterGrid[i] = true;
        }
        flatCraterGrid.Reverse(); //This reverse prevents the high odds of one of the items ending up in the first position.
        flatCraterGrid.Sort((_1, _2) => Random.Range(-1, 1));
        
        //Create craters in final positions.
        for(var i = 0; i < totalSize; ++i)
        {
            if (!flatCraterGrid[i]) //No crater here, skip on. 
            {
                continue;
            }

            var xJitter = Random.Range(-_craterPositionJitter.x, _craterPositionJitter.x);
            var yJitter = Random.Range(-_craterPositionJitter.y, _craterPositionJitter.y);
            var x = _levelBoundary.x + _levelBoundary.width * i % gridWidth / gridWidth + xJitter + _craterPrefab.Size.x / 2;
            var y = _levelBoundary.y - _levelBoundary.height * i / gridWidth / gridHeight + yJitter + _craterPrefab.Size.y / 2;
            var crater = Instantiate(_craterPrefab, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    private enum LevelObject
    {
        Rock,
        EnginePart,
        FuelPart,
        BodyPart,
        None
    }
    
    private void SetupLevel()
    {
        //Calculate our sizes.
        var gridWidth = (int)(_levelBoundary.width / Mathf.Max(_rockPrefab.Size.x, _enginePartPrefab.Size.x, _fuelPartPrefab.Size.x, _bodyPartPrefab.Size.x));
        var gridHeight = (int)(_levelBoundary.height / Mathf.Max(_rockPrefab.Size.y, _enginePartPrefab.Size.y, _fuelPartPrefab.Size.y, _bodyPartPrefab.Size.y));
        var totalSize = gridWidth * gridHeight;
        var totalObjects = _maxMoonRocks + _maxEngineParts + _maxFuelParts + _maxBodyParts;
        Debug.Assert(totalSize > totalObjects, $"Total grid size ({totalSize}) must be greater than the total number of objects ({totalObjects})."); //Warn about the index out of bounds.
        
        //Randomly distribute objects around the level.
        var flatGrid = Concatenate(
            Enumerable.Repeat(LevelObject.Rock, _maxMoonRocks),
            Enumerable.Repeat(LevelObject.EnginePart, _maxEngineParts),
            Enumerable.Repeat(LevelObject.FuelPart, _maxFuelParts),
            Enumerable.Repeat(LevelObject.BodyPart, _maxBodyParts),
            Enumerable.Repeat(LevelObject.None, totalSize - totalObjects))
            .ToList();
        flatGrid.Reverse(); //This reverse prevents the high odds of one of the items ending up in the first position.
        flatGrid.Sort((_1, _2) => Random.Range(-1, 1));
        
        //Create objects in final positions.
        for(var i = 0; i < totalSize; ++i)
        {
            var xJitter = Random.Range(-_objectPositionJitter.x, _objectPositionJitter.x);
            var yJitter = Random.Range(-_objectPositionJitter.y, _objectPositionJitter.y);
            var relativeX = (float)i % gridWidth / gridWidth;
            var relativeY = (float)i / gridWidth / gridHeight;
            var x = _levelBoundary.x + _levelBoundary.width * relativeX + xJitter;
            var y = _levelBoundary.y - _levelBoundary.height * relativeY + yJitter;
            switch (flatGrid[i]) //No crater here, skip on. 
            {
                case LevelObject.Rock:
                    var rock = Instantiate(_rockPrefab, new Vector3(x + _rockPrefab.Size.x / 2, y + _rockPrefab.Size.y / 2, 0), Quaternion.identity);
                    break;
                case LevelObject.EnginePart:
                    var enginePart = Instantiate(_enginePartPrefab, new Vector3(x + _enginePartPrefab.Size.x / 2, y + _enginePartPrefab.Size.y / 2, 0), Quaternion.identity);
                    break;
                case LevelObject.FuelPart:
                    var fuelPart = Instantiate(_fuelPartPrefab, new Vector3(x + _fuelPartPrefab.Size.x / 2, y + _fuelPartPrefab.Size.y / 2, 0), Quaternion.identity);
                    break;
                case LevelObject.BodyPart:
                    var bodyPart = Instantiate(_bodyPartPrefab, new Vector3(x + _bodyPartPrefab.Size.x / 2, y + _bodyPartPrefab.Size.y / 2, 0), Quaternion.identity);
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