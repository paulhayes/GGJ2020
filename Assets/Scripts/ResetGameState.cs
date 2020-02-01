using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameState : MonoBehaviour
{
   [SerializeField]
   GameState gameState;
    void Start()
    {
        gameState.Reset();
    }

    void OnDestroy(){
        gameState.Stopped();
    }


}
