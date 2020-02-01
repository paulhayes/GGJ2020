using UnityEngine;

namespace Parts
{
    public class FuelPart : MonoBehaviour
    {
        [SerializeField] private Vector2 _size; 
        public Vector2 Size => _size;
    }
}