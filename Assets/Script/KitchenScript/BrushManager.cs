using UnityEngine;
using System.Collections.Generic;

public class BrushManager : MonoBehaviour
{
    public GameObject brushCursor;
    public int totalZones = 6;
    private HashSet<string> paintedZones = new HashSet<string>();

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        brushCursor.transform.position = mousePos;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.GetComponent<BrushZone>() != null)
            {
                string zoneName = hit.collider.gameObject.name;
                if (!paintedZones.Contains(zoneName))
                {
                    paintedZones.Add(zoneName);
                }

                if (paintedZones.Count >= totalZones && !GameData.isBrushDone)
                {
                    GameData.isBrushDone = true;

                    // 刷皮全部完成：进度条走满
                    PanelManager panelManager = PanelManager.Instance;
                    if (panelManager == null) panelManager = FindObjectOfType<PanelManager>(true);
                    if (panelManager != null) panelManager.CompleteBrush();
                }
            }
        }
    }

    void OnDestroy()
    {
        Cursor.visible = true;
    }
}