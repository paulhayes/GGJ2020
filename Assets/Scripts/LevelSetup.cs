using System;
using Moon;
using Parts;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    private class GlobalGameState //Remove when the real thing arrives.
    {
        
    }
    
    [SerializeField] private GlobalGameState _globalGameState;
    [SerializeField] private Crater _craterPrefab;
    [SerializeField] private MoonRock _moonRockPrefab;
    [SerializeField] private EnginePart _enginePartPrefab;
    [SerializeField] private FuelPart _fuelPartPrefab;
    [SerializeField] private BodyPart _bodyPartPrefab;

    private void Awake()
    {
        // _globalGameState.OnChange += OnStateChange;
    }

    private void OnStateChange(GameState oldGameState, GameState newGameState)
    {
        // if(newGameState == Blackout)
        {
            SetupLevel();
        }
    }

    private void SetupLevel()
    {
        
    }
}