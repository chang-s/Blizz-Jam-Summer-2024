using System;
using UnityEngine;

namespace _Scripts.Testing.Luna
{
    public class SuperSpeedMover : MonoBehaviour
    {
        [SerializeField] private float _baseMovementSpeed;
        [SerializeField] private float _scale;

        private void FixedUpdate()
        {
            transform.position += new Vector3(_baseMovementSpeed * Time.fixedDeltaTime,0 , 0);
            transform.localScale += new Vector3(_scale * 2 * Time.fixedDeltaTime, _scale * Time.fixedDeltaTime);
        }
    }
}