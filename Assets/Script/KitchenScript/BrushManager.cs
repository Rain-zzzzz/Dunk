using UnityEngine;
using System.Collections.Generic;

public class BrushManager : MonoBehaviour
{
    public GameObject brushCursor; // 刷子图标（换成自定义鼠标样式）
    public int totalZones = 6;
    private HashSet<string> paintedZones = new HashSet<string>();

    void Start()
    {
        // 隐藏系统鼠标，显示刷子图标
        Cursor.visible = false;
    }

    void Update()
    {
        // 让刷子跟随鼠标
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        brushCursor.transform.position = mousePos;

        // 鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.GetComponent<BrushZone>() != null)
            {
                string zoneName = hit.collider.gameObject.name;
                if (!paintedZones.Contains(zoneName))
                {
                    paintedZones.Add(zoneName);
                    Debug.Log($"涂了 {zoneName}，进度 {paintedZones.Count}/{totalZones}");
                    // 可以在这里播放刷子音效或粒子特效
                }

                // 判定是否全部涂完
                if (paintedZones.Count >= totalZones)
                {
                    GameData.isBrushDone = true;
                    Debug.Log("刷皮均匀完成！");
                }
            }
        }
    }

    // 退出场景时记得恢复鼠标
    void OnDestroy()
    {
        Cursor.visible = true;
    }
}