using System.Collections;
using UnityEngine;

public class ZombieDissolve : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SkinnedMeshRenderer zombieRenderer;

    [Header("Configuración")]
    [SerializeField] private float dissolveDuration = 2f;
    [SerializeField] private int bodyMaterialIndex = 1;

    public void StartDissolve()
    {
        StartCoroutine(DissolveCoroutine());
    }

    private IEnumerator DissolveCoroutine()
    {
        yield return new WaitForSeconds(2f);

        if (zombieRenderer == null) yield break;

        float elapsedTime = 0f;
        Material[] zombieMaterials = zombieRenderer.materials;

        if (bodyMaterialIndex >= 0 && bodyMaterialIndex < zombieMaterials.Length)
        {
            Material bodyMaterial = zombieMaterials[bodyMaterialIndex];
            string propertyName = "_DissolveAmount";

            while (elapsedTime < dissolveDuration)
            {
                elapsedTime += Time.deltaTime;
                float currentDissolve = Mathf.Lerp(0f, 1f, elapsedTime / dissolveDuration);

                bodyMaterial.SetFloat(propertyName, currentDissolve);

                yield return null;
            }
        }
    }
}