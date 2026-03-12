using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [Header("Rotate")]
    public float rotateAngle = 45f;

    [Header("Scale")]
    public float scaleMultiplier = 1.2f;
    public float scaleMin = 0.2f;
    public float scaleMax = 5f;

    private void OnMouseDown()
    {
        if (ItemMenuController.Instance != null)
        {
            ItemMenuController.Instance.ShowMenu(this);
        }
    }

    public void DeleteSelf()
    {
        Destroy(gameObject);
    }

    public void RotateSelf()
    {
        transform.Rotate(0f, rotateAngle, 0f);
    }

    public void ScaleUp()
    {
        Vector3 newScale = transform.localScale * scaleMultiplier;

        newScale.x = Mathf.Min(newScale.x, scaleMax);
        newScale.y = Mathf.Min(newScale.y, scaleMax);
        newScale.z = Mathf.Min(newScale.z, scaleMax);

        transform.localScale = newScale;
    }

    public void ScaleDown()
    {
        Vector3 newScale = transform.localScale / scaleMultiplier;

        newScale.x = Mathf.Max(newScale.x, scaleMin);
        newScale.y = Mathf.Max(newScale.y, scaleMin);
        newScale.z = Mathf.Max(newScale.z, scaleMin);

        transform.localScale = newScale;
    }

    public void ChangeColor()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            r.material.color = Random.ColorHSV(
                0f, 1f,
                0.7f, 1f,
                0.7f, 1f
            );
        }
    }
}