using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jamey
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Camera _camera;
        [FormerlySerializedAs("_particleSystem")] [SerializeField] private ParticleSystem _trail;
        [SerializeField] private ParticleSystem _blaze;
        [SerializeField] private float _cameraZoomOutScale;

        private float _initialYPosition;

        private void Start()
        {
            _initialYPosition = transform.position.y;
            _trail.Stop();
            _blaze.Stop();
        }

        private void Update()
        {
            UpdateRocketPosition();
            UpdateCameraPosition();
        }

        private void UpdateRocketPosition()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _rigidbody2D.AddRelativeForce(Vector2.up * 0.001f, ForceMode2D.Impulse);
                _trail.Play();
                _blaze.Play();
            }
            else
            {
                _trail.Stop();
                _blaze.Stop();
            }
        }

        private void UpdateCameraPosition()
        {
            const float offsetToAllowRocketToFitInShot = -1f;
            var cameraTransform = _camera.transform;
            var cameraPosition = cameraTransform.position;
            var cameraZPosition = offsetToAllowRocketToFitInShot + (transform.position.y - _initialYPosition) * -_cameraZoomOutScale;
            cameraZPosition = Mathf.Min(cameraZPosition, 3.5f); //TODO: What is the significance of 3.5?
            cameraPosition = new Vector3(cameraPosition.x, cameraPosition.y, cameraZPosition);
            cameraTransform.position = cameraPosition;
        }
    }
}