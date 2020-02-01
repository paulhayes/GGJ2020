using UnityEngine;

namespace Parts
{
    public class BodyPart : MonoBehaviour
    {
        [SerializeField] private Vector2 _size; 
        public Vector2 Size { get; }
    }
}