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

    [SerializeField] LevelSettings levelSettings;

    GameObject[] _spawnedItems;
    GameObject[] _spawnedCraters;


    void Awake()
    {
      _spawnedItems = new GameObject[_objectGrid.x * _objectGrid.y];
      _spawnedCraters = new GameObject[_craterGrid.x * _craterGrid.y];

    }
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

        if (newState == States.Countdown)
        {
            foreach (var spawnedObject in _spawnedItems)
            {
              if( spawnedObject==null )
                continue;

              var component = spawnedObject.GetComponent<Collider2D>();
              if (component != null)
              {
                  component.isTrigger = true;
              }
            }
        }
    }

    private void RemoveSpawned()
    {
        
        for(int i=0;i<_spawnedItems.Length;i++){
          Destroy(_spawnedItems[i]);
          _spawnedItems[i] = null;
        }
        for(int i=0;i<_spawnedCraters.Length;i++){
          Destroy(_spawnedCraters[i]);
          _spawnedCraters[i] = null;
        }        
    }

    private void SetupMoonFeatures()
    {
        //Calculate our cell sizes.
        var cellSize = new Vector2(_levelBoundary.width / _craterGrid.x, _levelBoundary.height / _craterGrid.y);
        
        //Randomly distribute craters around the level, removing any dead zones.
        //var gridPositions = GetShuffledCellQueue(_craterGrid, cellSize);
        var availableSpaces = _craterGrid.x * _craterGrid.y;
        var numCells = NumCellsAvailbile(_craterGrid, cellSize);
        int spawned = 0;
        //Create craters in final positions.
        CreatePrefabs(_crater, availableSpaces, _spawnedCraters, ref spawned);
        Shuffle(_spawnedCraters,0,availableSpaces);
        PlaceObjectsInGrid(_craterGrid, cellSize, _spawnedCraters);
        AddJitter(_spawnedCraters, _craterPositionJitter);
    }
    
    private void SetupLevel()
    {
        Debug.Log("Spawning Items");

        //Calculate our sizes.
        var cellSize = new Vector2(_levelBoundary.width / _objectGrid.x, _levelBoundary.height / _objectGrid.y);
        
        //Randomly distribute objects around the level.
        //var gridPositions = GetShuffledCellQueue(_objectGrid, cellSize);
        var availableSpaces = NumCellsAvailbile(_objectGrid,cellSize);
        var numCells = _objectGrid.x * _objectGrid.y;
        //Create objects in final positions.
        //Debug.LogFormat("Item cells availbie {0}/{1}",availableSpaces,numCells);
        float total = 0;
        foreach (var objectRatio in _objectRatios)
        {
          total += objectRatio.Ratio;
        }
        //Debug.LogFormat("Ratio total {0}", total);
        int spawned = 0;
        foreach (var objectRatio in _objectRatios)
        {
          CreatePrefabs(objectRatio, availableSpaces,_spawnedItems, ref spawned);
          //Debug.LogFormat("spawning {0} {1}",objectRatio.Object.name,spawned);
        }
        //Debug.LogFormat("spawned items total {0}",spawned);
        Shuffle(_spawnedItems,0,availableSpaces);
        var ip = PlaceObjectsInGrid(_objectGrid,cellSize,_spawnedItems);
        //Debug.LogFormat("items positioned {0}",ip);
        AddJitter(_spawnedItems,_objectPositionJitter);

    }

    private void PositionItem(){
      
      
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

    private static void Shuffle<T>(T[] arr, int start=0, int end=-1)
    {
        var count = arr.Length;
        if(end == -1){
          end = count;
        }
        for (var i = start; i < end; ++i)
        {
            var r = Random.Range(i, end);
            var tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }

    private void CreatePrefabs(ObjectRatio objectRatio, int totalObjects, GameObject[] spawnedList, ref int startIndex)
    {
        int safeNumberToCreate = Mathf.FloorToInt( totalObjects * objectRatio.Ratio );
        for (var i = 0; i < safeNumberToCreate; i++)
        {                      
          var obj = spawnedList[i+startIndex] = Instantiate(objectRatio.Object, Vector3.zero, objectRatio.Rotate ? Quaternion.Euler(0, 0, Random.value * 360) : Quaternion.identity);          
        }
        startIndex+=safeNumberToCreate;
        
    }

    private int NumCellsAvailbile(Vector2Int grid, Vector2 cellSize)
    {
      int blockedCells = 0;
      for (var x = 0; x < grid.x; x++)
      {
          for (var y = 0; y < grid.y; y++)
          {
            var cell = new Rect(_levelBoundary.min.x + x * cellSize.x, _levelBoundary.min.y + y * cellSize.y, cellSize.x, cellSize.y);
            
            if(IsCellBlocked(cell)){
              blockedCells++;                  
            }
          }
      }
      return (grid.x * grid.y) - blockedCells;
    }



    private int PlaceObjectsInGrid(Vector2Int grid, Vector2 cellSize, GameObject[] objects )
    {
        int objIndex = 0;
        int objectsPositioned = 0;
        int cellsBlocked = 0;
        for (var i = 0; i < grid.x; i++)
        {
            for (var j = 0; j < grid.y; j++)
            {
                var cell = new Rect(_levelBoundary.min.x + i * cellSize.x, _levelBoundary.min.y + j * cellSize.y, cellSize.x, cellSize.y);
                //Debug.LogFormat("{0},{1} {2} {3} :{4}:",i,j,cell,IsCellBlocked(cell),objects[objIndex]!=null);                
                if(IsCellBlocked(cell))
                {
                  cellsBlocked++;
                  continue;
                }
                if(objects[objIndex]!=null)
                {
                  var x = cell.center.x;
                  var y = cell.center.y;
                  //Debug.LogFormat("Placing {0} @ {1},{2}",objects[objIndex].name,x,y);
                  objects[objIndex].transform.position = new Vector3(x,y,0);
                  objectsPositioned++;
                }
                objIndex++;
            }
        }
        //Debug.LogFormat("Cells blocked {0}",cellsBlocked);
        //Debug.LogFormat("Cells iterated over {0}",objIndex);
        return objectsPositioned;
    }

    private void AddJitter(GameObject[] objects, Vector2 jitter)
    {
      foreach(var obj in objects){
        if(obj==null)
          continue;
        var xJitter = Random.Range(-jitter.x, jitter.x);
        var yJitter = Random.Range(-jitter.y, jitter.y);                  
        obj.transform.position += new Vector3(xJitter,yJitter,0);
      }
    }

/*
    private Queue<Rect> GetShuffledCellQueue(Vector2Int grid, Vector2 cellSize)
    {
        var validCells = new List<Rect>();
        for (var x = 0; x < grid.x; x++)
        {
            for (var y = 0; y < grid.y; y++)
            {
                var cell = new Rect(_levelBoundary.min.x + x * cellSize.x, _levelBoundary.min.y + y * cellSize.y, cellSize.x, cellSize.y);
                
                if (!IsCellBlocked(cell))
                {
                    validCells.Add(cell);
                }
            }
        }
        Shuffle(validCells);
        return new Queue<Rect>(validCells);
    }
*/

    private bool IsCellBlocked(Rect cell)
    {
      foreach (var deadZone in _deadZones)
      {
          var bounds = deadZone.bounds;
          var rocketBounds = new Rect(bounds.min, bounds.size);
          if (rocketBounds.Overlaps(cell))
          {
            return true;
          }
      }
      return false;
    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireCube(_levelBoundary.center,_levelBoundary.size);
        
    }
}