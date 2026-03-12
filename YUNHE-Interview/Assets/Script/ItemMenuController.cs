using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemMenuController : MonoBehaviour
{
    public static ItemMenuController Instance;

    [Header("UI")]
    public GameObject menuRoot;
    public Button deleteButton;
    public Button rotateButton;
    public Button scaleUpButton;
    public Button scaleDownButton;
    public Button colorButton;

    [Header("Menu Offset")]
    public Vector3 worldOffset = new Vector3(1f, 1f, 0f);

    private ItemObject currentTarget;
    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;

        if (menuRoot != null)
            menuRoot.SetActive(false);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(DeleteCurrentTarget);

        if (rotateButton != null)
            rotateButton.onClick.AddListener(RotateCurrentTarget);

        if (scaleUpButton != null)
            scaleUpButton.onClick.AddListener(ScaleUpCurrentTarget);

        if (scaleDownButton != null)
            scaleDownButton.onClick.AddListener(ScaleDownCurrentTarget);

        if (colorButton != null)
            colorButton.onClick.AddListener(ChangeColorCurrentTarget);
    }

    private void Update()
    {
        if (menuRoot != null && menuRoot.activeSelf && currentTarget != null)
        {
            UpdateMenuPosition();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ItemObject item = hit.collider.GetComponentInParent<ItemObject>();

                if (item != null)
                {
                    ShowMenu(item);
                }
                else
                {
                    HideMenu();
                }
            }
            else
            {
                HideMenu();
            }
        }
    }

    public void ShowMenu(ItemObject target)
    {
        currentTarget = target;

        if (menuRoot != null)
        {
            menuRoot.SetActive(true);
            UpdateMenuPosition();
        }
    }

    public void HideMenu()
    {
        currentTarget = null;

        if (menuRoot != null)
            menuRoot.SetActive(false);
    }

    private void UpdateMenuPosition()
    {
        if (currentTarget == null || menuRoot == null || mainCam == null)
            return;

        Vector3 worldPos = currentTarget.transform.position + worldOffset;
        Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

        menuRoot.transform.position = screenPos;
    }

    private void DeleteCurrentTarget()
    {
        if (currentTarget == null) return;

        currentTarget.DeleteSelf();
        HideMenu();
    }

    private void RotateCurrentTarget()
    {
        if (currentTarget == null) return;

        currentTarget.RotateSelf();
        UpdateMenuPosition();
    }

    private void ScaleUpCurrentTarget()
    {
        if (currentTarget == null) return;

        currentTarget.ScaleUp();
        UpdateMenuPosition();
    }

    private void ScaleDownCurrentTarget()
    {
        if (currentTarget == null) return;

        currentTarget.ScaleDown();
        UpdateMenuPosition();
    }

    private void ChangeColorCurrentTarget()
    {
        if (currentTarget == null) return;

        currentTarget.ChangeColor();
    }
}