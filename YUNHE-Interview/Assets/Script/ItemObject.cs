using UnityEngine;

public class ItemObject : MonoBehaviour
{
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
}