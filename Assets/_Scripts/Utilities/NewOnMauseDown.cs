using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MouseClickType
{
    Left,
    Right
}

public interface IMouseObserver_Click
{
    public void MauseDown(MouseClickType mouseClickType);
}
public interface IMouseObserver_PressedOnObj
{
    public void MausePressedOnObj(MouseClickType mouseClickType);
}

public class NewOnMauseDown : MonoBehaviour
{
    Camera cam;
    private List<IMouseObserver_PressedOnObj> objectsForReactPressed = new List<IMouseObserver_PressedOnObj>();

    [SerializeField] private LayerMask UILayers;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        //Click on pbject
        if(Input.GetMouseButtonDown(0))
        {
            MouseClick(MouseClickType.Left);
        }
        if(Input.GetMouseButtonDown(1))
        {
            MouseClick(MouseClickType.Right);
        }

        //Press on object
        if(Input.GetMouseButton(0))
        {
            MousePressedObObjects(MouseClickType.Left);
        }
        if (Input.GetMouseButton(1))
        {
            MousePressedObObjects(MouseClickType.Right);
        }

        if(Input.GetMouseButtonUp(0))
        {
            objectsForReactPressed = new List<IMouseObserver_PressedOnObj>();
        }
        if (Input.GetMouseButtonUp(1))
        {
            objectsForReactPressed = new List<IMouseObserver_PressedOnObj>();
        }
    }

    private void MouseClick(MouseClickType button)
    {
        if (IsPointerOverUIObject())
            return;

        Vector2 mausePos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        RaycastHit2D[] click_results = Physics2D.RaycastAll(mausePos, Vector2.zero);
        foreach (var item in click_results)
        {
            if(item.collider.TryGetComponent<IMouseObserver_Click>(out IMouseObserver_Click obj))
            {
                obj.MauseDown(button);
            }
            else if(item.collider.TryGetComponent<IMouseObserver_PressedOnObj>(out IMouseObserver_PressedOnObj obj2))
            {
                objectsForReactPressed.Add(obj2);
            }
        }
    }
    private void MousePressedObObjects(MouseClickType button)
    {
        if (IsPointerOverUIObject())
            return;

        foreach (var item in objectsForReactPressed)
        {
            item.MausePressedOnObj(button);
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
