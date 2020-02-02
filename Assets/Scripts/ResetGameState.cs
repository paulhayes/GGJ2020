using System;
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
    
    #if DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ++gameState.State;
        }
    }
#endif

    void OnDestroy(){
        gameState.Stopped();
    }


}
