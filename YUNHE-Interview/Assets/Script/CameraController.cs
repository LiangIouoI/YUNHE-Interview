using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public int edgeSize = 20; // 距離螢幕邊緣幾像素開始移動

    public Vector2 limitX = new Vector2(-50, 50);
    public Vector2 limitZ = new Vector2(-50, 50);

    void Update()
    {
        Vector3 move = Vector3.zero;

        Vector3 mousePos = Input.mousePosition;

        // 左
        if (mousePos.x <= edgeSize)
        {
            move.x = -1;
        }

        // 右
        if (mousePos.x >= Screen.width - edgeSize)
        {
            move.x = 1;
        }

        // 下
        if (mousePos.y <= edgeSize)
        {
            move.z = -1;
        }

        // 上
        if (mousePos.y >= Screen.height - edgeSize)
        {
            move.z = 1;
        }

        transform.position += move * moveSpeed * Time.deltaTime;

        // 限制攝影機範圍
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, limitX.x, limitX.y);
        pos.z = Mathf.Clamp(pos.z, limitZ.x, limitZ.y);

        transform.position = pos;
    }
}