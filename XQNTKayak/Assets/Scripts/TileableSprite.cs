using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayakGame
{
    public class TileableSprite : MonoBehaviour
    {
        [SerializeField] private GameObject trackedObject;
        [SerializeField] private LinkedList<GameObject> sprites = new LinkedList<GameObject>();

        private Vector3 startPosition;
        private float spriteHeight;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                sprites.AddLast(transform.GetChild(i).gameObject);
            }

            var sprite = sprites.First.Value;
            spriteHeight = sprite.transform.localScale.y;
        }

        private void Update()
        {
            UpdateTilePosition();
        }

        private void UpdateTilePosition()
        {
            var update = true;

            // Place the top sprite in the bottom
            while (update)
            {
                var middleSpriteNode = sprites.First.Next;
                var middleSprite = middleSpriteNode.Value; // Get the one from the middle;

                if (trackedObject.transform.position.y < (middleSprite.transform.position.y - (spriteHeight * 0.5f)))
                {
                    var first = sprites.First;

                    var newPosition = first.Value.transform.position - new Vector3(0, spriteHeight * 3, 0);
                    first.Value.transform.position = newPosition;

                    sprites.RemoveFirst();
                    sprites.AddLast(first);



                }
                else if (trackedObject.transform.position.y > (middleSprite.transform.position.y + (spriteHeight * 0.5f)))
                {
                    var last = sprites.Last;

                    var newPosition = last.Value.transform.position + new Vector3(0, spriteHeight * 3, 0);
                    last.Value.transform.position = newPosition;

                    sprites.RemoveLast();
                    sprites.AddFirst(last);
                }
                else
                {
                    update = false;
                }
            }

        }
    }

}
