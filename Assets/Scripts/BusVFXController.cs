
using UnityEngine;

public class BusVFXController : MonoBehaviour
{
    [Header("Prefabs VFX")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private GameObject collisionVFX;
    [SerializeField] private GameObject shieldBreakVFX;

    [Header("Puntos de aparición")]
    [SerializeField] private Transform explosionPoint;
    [SerializeField] private Transform collisionPoint;

    [Header("Escala de efectos")]
    [SerializeField, Min(0.01f)] private float explosionScale = 3f;
    [SerializeField, Min(0.01f)] private float collisionScale = 1.5f;
    [SerializeField, Min(0.01f)] private float shieldBreakScale = 2f;

    [Header("Configuración")]
    [SerializeField] private float vfxLifetime = 3f;

    public void PlayExplosion()
    {
        SpawnVFX(explosionVFX, explosionPoint, explosionScale);
    }

    public void PlayCollision()
    {
        SpawnVFX(collisionVFX, collisionPoint, collisionScale);
    }

    public void PlayShieldBreak()
    {
        SpawnVFX(shieldBreakVFX, collisionPoint, shieldBreakScale);
    }

    private void SpawnVFX(
        GameObject prefab,
        Transform spawnPoint,
        float scaleMultiplier)
    {
        if (prefab == null)
        {
            Debug.LogWarning("No se asignó el prefab VFX.", this);
            return;
        }

        Vector3 position = spawnPoint != null
            ? spawnPoint.position
            : transform.position;

        GameObject effect = Instantiate(
            prefab,
            position,
            prefab.transform.rotation
        );

        // Multiplicar la escala original del prefab.
        effect.transform.localScale =
            prefab.transform.localScale * scaleMultiplier;

        Destroy(effect, vfxLifetime);
    }
}
