using UnityEngine;

/// <summary>
/// Keeps a rigidbody static at start and lets GameManager trigger its drop later.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class DelayedDrop : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [Tooltip("Optional physics material applied to all colliders when dropping.")]
    [SerializeField] private PhysicsMaterial2D physicsMaterial;

    private bool hasDropped;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Static;
    }

    /// <summary>
    /// Called by GameManager after the player finishes drawing.
    /// </summary>
    public void StartDrop(GameManager gameManager)
    {
        if (hasDropped || rb == null)
        {
            return;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;

        if (gameManager != null)
        {
            rb.gravityScale = gameManager.lineGravityScale;
            rb.mass = gameManager.lineMass;
            rb.drag = gameManager.lineDrag;
            rb.angularDrag = gameManager.lineAngularDrag;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        if (physicsMaterial == null && gameManager != null)
        {
            physicsMaterial = gameManager.linePhysicsMaterial;
        }

        if (physicsMaterial != null)
        {
            foreach (var col in GetComponentsInChildren<Collider2D>())
            {
                col.sharedMaterial = physicsMaterial;
            }
        }

        hasDropped = true;
    }
}

