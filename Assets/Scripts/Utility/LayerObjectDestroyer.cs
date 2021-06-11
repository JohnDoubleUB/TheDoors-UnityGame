using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LayerObjectDestroyer : MonoBehaviour
{
    public SingleUnityLayer culledLayer;

    private BoxCollider2D boxCollider2D;
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (culledLayer != null && culledLayer.LayerIndex == collision.gameObject.layer) Destroy(collision.gameObject);
    }
}
