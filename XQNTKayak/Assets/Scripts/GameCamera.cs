using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayakGame
{
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private Transform followTransform;

        private void LateUpdate()
        {
            var followPos = followTransform.transform.position;
            transform.position = new Vector3(followPos.x, followPos.y, -10f);
        }
    }
}