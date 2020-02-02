using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] LayerMask _playerMask;

    [SerializeField] RobotPlayerData playerData;

    [SerializeField] AudioSource _source;
    [SerializeField] AudioSource[] repairSFX;


    public void AddScore(Vector3 scoreDelta)
    {
        playerData.Score += scoreDelta;

        if (scoreDelta != Vector3.zero){
            StartCoroutine( PlayRepairSFX(scoreDelta) );
        }
           
    }

    IEnumerator PlayRepairSFX(Vector3 score)
    {
        for(int i=0;i<3;i++){
            if(!repairSFX[i].clip)
                continue;
            if(score[i]>0){
                repairSFX[i].Play();
                yield return new WaitForSeconds( repairSFX[i].clip.length );
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_playerMask == (_playerMask | 1 << collision.gameObject.layer))
        {
            RobotCharacter._instance.BankItems(this);
        }
    }
}
