using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToPanel : MonoBehaviour, IPointerUpHandler
{
    public RectTransform TriggerPanel;
    public RectTransform targetPanel;
    public List<GameObject> showList;
    public List<GameObject> hideList;

    public void OnPointerUp(PointerEventData e)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(e, results);

        foreach (var r in results)
        {
            if (r.gameObject != null &&
                (r.gameObject.transform == TriggerPanel || r.gameObject.transform.IsChildOf(TriggerPanel)))
            {
                Trigger();
                return;
            }
        }
    }

    private void Trigger()
    {
        if (targetPanel != null)
        {
            targetPanel.gameObject.SetActive(true);
        }

        if (showList != null)
        {
            foreach (var go in showList)
            {
                if (go != null)
                {
                    go.SetActive(true);
                }
            }
        }

        if (hideList != null)
        {
            foreach (var go in hideList)
            {
                if (go != null)
                {
                    go.SetActive(false);
                }
            }
        }
    }
}
