using System;
using UnityEngine;

namespace _Scripts.Testing.Luna
{
    public class SuperSpeedMover : MonoBehaviour
    {
        [SerializeField] private float _baseMovementSpeed;

        private void FixedUpdate()
        {
            transform.position += new Vector3(_baseMovementSpeed * Time.fixedDeltaTime, 0, 0);
        }
    }
}