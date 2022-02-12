using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KayakGame
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent onLeftPaddleMoveStart;
        [SerializeField] private UnityEvent onLeftPaddleMovePerformed;
        [SerializeField] private UnityEvent onRightPaddleMoveStart;
        [SerializeField] private UnityEvent onRightPaddleMovePerformed;

        [SerializeField] private KeyCode rightRow;
        [SerializeField] private KeyCode leftRow;

        private void Update()
        {
            if (Input.GetKeyDown(rightRow))
            {
                onRightPaddleMoveStart?.Invoke();
            }
            if (Input.GetKeyDown(leftRow))
            {
                onLeftPaddleMoveStart?.Invoke();
            }
            if (Input.GetKeyUp(rightRow))
            {
                onRightPaddleMovePerformed?.Invoke();
            }
            if (Input.GetKeyUp(leftRow))
            {
                onLeftPaddleMovePerformed?.Invoke();
            }
        }
    }
}

