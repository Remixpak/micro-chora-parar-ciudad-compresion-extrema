
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

    [Header("Configuración")]
    [SerializeField] private float vfxLifetime = 3f;

    public void PlayExplosion()
    {
        SpawnVFX(explosionVFX, explosionPoint);
    }

    public void PlayCollision()
    {
        SpawnVFX(collisionVFX, collisionPoint);
    }

    public void PlayShieldBreak()
    {
        SpawnVFX(shieldBreakVFX, collisionPoint);
    }

    private void SpawnVFX(GameObject prefab, Transform spawnPoint)
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

        Destroy(effect, vfxLifetime);
    }
}
