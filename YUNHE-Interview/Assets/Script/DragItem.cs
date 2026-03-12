using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Prefab")]
    public GameObject itemPrefab;

    [Header("Placement")]
    public LayerMask placementLayerMask;
    public float spawnHeightOffset = 0.5f;
    public float rayDistance = 1000f;

    [Header("Preview")]
    [Range(0.1f, 1f)]
    public float previewAlpha = 0.5f;

    [Header("Guide Line")]
    public bool useGuideLine = true;
    public float lineStartWidth = 0.05f;
    public float lineEndWidth = 0.02f;

    private GameObject preview;
    private LineRenderer guideLine;

    private bool hasValidPlacement = false;
    private Vector3 validSpawnPosition;
    private Vector3 currentHitPoint;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemPrefab == null)
            return;

        if (mainCam == null)
            mainCam = Camera.main;

        preview = Instantiate(itemPrefab);

        // 預覽物件關閉物理
        SetPhysicsState(preview, false);

        // 預覽半透明
        SetPreviewTransparent(preview, previewAlpha);

        // 一開始先隱藏
        SetPreviewVisible(false);

        hasValidPlacement = false;

        // 建立指引線
        if (useGuideLine)
        {
            CreateGuideLine();
            SetGuideLineVisible(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (preview == null || mainCam == null)
            return;

        UpdatePreviewAndPlacement();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (preview == null)
            return;

        // 只有有效位置才生成
        if (hasValidPlacement)
        {
            GameObject placedObj = Instantiate(itemPrefab, validSpawnPosition, preview.transform.rotation);

            // 生成後開啟物理
            SetPhysicsState(placedObj, true);
        }

        CleanupPreview();
    }

    private void UpdatePreviewAndPlacement()
    {
        // 在 UI 上 -> 無效
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            hasValidPlacement = false;
            SetPreviewVisible(false);
            SetGuideLineVisible(false);
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        // 只打指定場景 Layer
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, placementLayerMask))
        {
            currentHitPoint = hit.point;
            validSpawnPosition = hit.point + Vector3.up * spawnHeightOffset;
            hasValidPlacement = true;

            preview.transform.position = validSpawnPosition;
            SetPreviewVisible(true);

            if (useGuideLine && guideLine != null)
            {
                SetGuideLineVisible(true);

                // 線從預覽物件底下連到實際落點
                guideLine.SetPosition(0, preview.transform.position);
                guideLine.SetPosition(1, currentHitPoint);
            }
        }
        else
        {
            // 沒打到場景 -> 無效
            hasValidPlacement = false;
            SetPreviewVisible(false);
            SetGuideLineVisible(false);
        }
    }

    private void CleanupPreview()
    {
        if (guideLine != null)
        {
            Destroy(guideLine);
            guideLine = null;
        }

        Destroy(preview);
        preview = null;

        hasValidPlacement = false;
    }

    private void CreateGuideLine()
    {
        guideLine = preview.AddComponent<LineRenderer>();
        guideLine.positionCount = 2;
        guideLine.startWidth = lineStartWidth;
        guideLine.endWidth = lineEndWidth;
        guideLine.material = new Material(Shader.Find("Sprites/Default"));
        guideLine.useWorldSpace = true;

        guideLine.startColor = Color.yellow;
        guideLine.endColor = Color.red;
    }

    private void SetGuideLineVisible(bool visible)
    {
        if (guideLine != null)
        {
            guideLine.enabled = visible;
        }
    }

    private void SetPreviewVisible(bool visible)
    {
        if (preview == null)
            return;

        Renderer[] renderers = preview.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }
    }

    private void SetPhysicsState(GameObject obj, bool enabledState)
    {
        if (obj == null)
            return;

        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = enabledState;
        }

        Rigidbody[] rigidbodies = obj.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !enabledState;
            rb.useGravity = enabledState;
        }
    }

    private void SetPreviewTransparent(GameObject obj, float alpha)
    {
        if (obj == null)
            return;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            Material mat = r.material;
            Color c = mat.color;
            c.a = alpha;
            mat.color = c;

            // Standard Shader / Built-in 常用透明設定
            if (mat.HasProperty("_Mode"))
            {
                mat.SetFloat("_Mode", 3);
            }

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