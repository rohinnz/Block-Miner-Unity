using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTest : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D rigidBody2D;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = player.transform.position;
    }

    public void AnimateJump()
    {
        player.transform.position = startPosition;
        rigidBody2D.gravityScale = 1;
    }

    private void Update()
    {
        
        if (player.transform.position.y < -4f)
        {
            var pos = player.transform.position;
            pos.y = -4f;
            player.transform.position = pos;

            rigidBody2D.gravityScale = 0;
            rigidBody2D.velocity = Vector2.zero;
        }
    }
}
