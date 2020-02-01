using UnityEngine;

namespace Moon
{
    public class Crater : MonoBehaviour
    {
        [SerializeField] private Vector2 _size; //Just a temporary filler.
        public Vector2 Size => _size;
    }
}