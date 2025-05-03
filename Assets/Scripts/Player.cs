using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public Action<GameObject> TouchItemEvent;

    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] SpriteRenderer spriteRenderer;
    private float gravityScale;

    public Rigidbody2D RigidBody => rigidBody;

    private void Awake()
    {
        gravityScale = rigidBody.gravityScale;
        rigidBody.gravityScale = 0;
    }

    public bool Gravity => rigidBody.gravityScale > 0;

    public void SetGravity(bool enable)
    {
        if (rigidBody.gravityScale > 0)
        {
            if (!enable)
            {
                rigidBody.gravityScale = 0;
                rigidBody.velocity = Vector3.zero;
            }
        }
        else
        {
            if (enable)
            {
                rigidBody.gravityScale = gravityScale;
                rigidBody.velocity = Vector3.zero;
            }
        }
    }

    public void SetFacingRight(bool isFacingRight)
    {
        spriteRenderer.flipX = !isFacingRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TouchItemEvent?.Invoke(collision.gameObject);
    }

    private void Reset()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
