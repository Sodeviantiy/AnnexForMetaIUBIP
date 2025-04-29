ï»¿using UnityEngine;

public class AddColliders : MonoBehaviour
{
    void Start()
    {
        // Ïîëó÷àåì âñå äî÷åðíèå îáúåêòû ìîäåëè, âêëþ÷àÿ å¸ ñàìó
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            // Ïðîâåðÿåì, íåò ëè óæå êîëëàéäåðà
            if (meshFilter.gameObject.GetComponent<Collider>() == null)
            {
                // Äîáàâëÿåì Mesh Collider, åñëè åãî íåò
                MeshCollider collider = meshFilter.gameObject.AddComponent<MeshCollider>();
                collider.convex = false; // Îïöèÿ Convex, îñòàâëÿåì false äëÿ ñòàòè÷åñêèõ îáúåêòîâ
            }
        }
    }
}
