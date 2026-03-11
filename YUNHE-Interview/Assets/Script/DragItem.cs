using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;

    public float spawnHeightOffset = 0.5f;
    public float previewAlpha = 0.5f;   // 預覽透明度

    private GameObject preview;

    public void OnBeginDrag(PointerEventData eventData)
    {
        preview = Instantiate(itemPrefab);

        // 關閉 Collider
        Collider col = preview.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // 關閉 Rigidbody
        Rigidbody rb = preview.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        MakeTransparent(preview);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = hit.point;
            pos.y += spawnHeightOffset;

            preview.transform.position = pos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            GameObject obj = Instantiate(itemPrefab, preview.transform.position, Quaternion.identity);

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        Destroy(preview);
    }

    void MakeTransparent(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            Material mat = r.material;

            Color c = mat.color;
            c.a = previewAlpha;
            mat.color = c;

            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }
}