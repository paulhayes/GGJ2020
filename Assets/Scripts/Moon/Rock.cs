using UnityEngine;

namespace Moon
{
    public class Rock : MonoBehaviour
    {
        [SerializeField] private Vector2 _size;
        public Vector2 Size => _size;
    }
}