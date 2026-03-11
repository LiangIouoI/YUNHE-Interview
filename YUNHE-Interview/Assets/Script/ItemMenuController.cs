using UnityEngine;
using UnityEngine.UI;

public class ItemMenuController : MonoBehaviour
{
    public static ItemMenuController Instance;

    public GameObject menuRoot;
    public Button deleteButton;

    private ItemObject currentTarget;

    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;

        menuRoot.SetActive(false);

        deleteButton.onClick.AddListener(DeleteCurrentTarget);
    }

    private void Update()
    {
        // 如果選單開著，讓它持續跟著目標物件旁邊
        if (menuRoot.activeSelf && currentTarget != null)
        {
            UpdateMenuPosition();
        }

        // 點空白處可關閉選單（可選）
        if (Input.GetMouseButtonDown(0))
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if (!Physics.Raycast(ray, out RaycastHit hit) || hit.collider.GetComponent<ItemObject>() == null)
                {
                    HideMenu();
                }
            }
        }
    }

    public void ShowMenu(ItemObject target)
    {
        currentTarget = target;
        menuRoot.SetActive(true);
        UpdateMenuPosition();
    }

    public void HideMenu()
    {
        currentTarget = null;
        menuRoot.SetActive(false);
    }

    private void UpdateMenuPosition()
    {
        Vector3 worldPos = currentTarget.transform.position + new Vector3(1.0f, 1.0f, 0f);
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        menuRoot.transform.position = screenPos;
    }

    private void DeleteCurrentTarget()
    {
        if (currentTarget != null)
        {
            Destroy(currentTarget.gameObject);
            HideMenu();
        }
    }
}