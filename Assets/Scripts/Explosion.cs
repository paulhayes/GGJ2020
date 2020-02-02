using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] SpriteFrame[] _frames;

    [SerializeField] SpriteRenderer _renderer;

    public IEnumerator Explode(){
        _renderer.enabled = true;
        for(int i=0;i<_frames.Length;i++){
            _renderer.sprite = _frames[i].sprite;
            yield return new WaitForSeconds(_frames[i].duration);
        }
        _renderer.enabled = false;

    }
}

[System.Serializable]
public class SpriteFrame 
{
    public Sprite sprite;
    public float duration;
}
