using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayakGame
{
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private Transform followTransform;
        [SerializeField] private Vector2 offset;

        private void LateUpdate()
        {
            var followPos = followTransform.transform.position;
            transform.position = new Vector3(followPos.x + offset.x, followPos.y + offset.y, -10f);
        }
    }
}