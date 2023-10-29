using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private int maxCollidors = 10; // How many overlapping colliders can be checked

    private Vector3 startPosition;
    private Vector3 startRotation;
    public Droppable currentDroppable; // The Droppable Object it's currently in
    public Droppable defaultDroppable; // Handles drops, when this Draggable isn't in a Droppable

    void Start()
    {
        // Make sure that the draggable Object has a Box Collider 
        Debug.Assert(GetComponent<Collider2D>() != null, "Draggable Object is missing a 2D Collider!", this);

        // Find and set default Droppable
        defaultDroppable = GetComponentInParent<DefaultDroppable>();
        Debug.Assert(defaultDroppable != null, "Couldn't find default Droppable", this);

        // Check whether Object is inside a Droppable
        (int colAmount, Collider2D[] colliders) = GetOverlappedDroppable();
        Debug.Assert(colAmount < 2, "Multiple Start-Droppables found", this);
        if (colAmount > 0) currentDroppable = colliders[0].GetComponent<Droppable>();
        else currentDroppable = defaultDroppable;
    }

    void OnMouseDown()
    {
        SavePosition();
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -1;
        transform.position = mousePos;
    }

    void OnMouseUp() 
    {        
        (int colAmount, Collider2D[] colliders) = GetOverlappedDroppable();
        Droppable newDroppable;
        if(colAmount == 0) // Not dropped into a Droppable
        {
            newDroppable = defaultDroppable;
        }
        else
        {
            Debug.Assert(colliders[0].GetComponent<Droppable>() != null, "Destination Object doesn't have Droppable Component", colliders[0]);
            newDroppable = colliders[0].GetComponent<Droppable>();
        }

        if (newDroppable == currentDroppable)
        {
            RestorePosition();
        }
        else
        {
            if (newDroppable.OnDrop(this))
            {
                currentDroppable.OnLeave(this);
                currentDroppable = newDroppable;
            }
            else
            {
                RestorePosition();
            }
        }
    }

    public void SavePosition()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    public void RestorePosition()
    {
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }

    public (int, Collider2D[]) GetOverlappedDroppable()
    {
        // Create Mask to Filter items that aren't on layer "Droppable"
        int dropLayer = LayerMask.NameToLayer("Droppable");
        Debug.Assert(dropLayer > 0, "Droppable Layer not found");
        LayerMask layerMask = 1 << dropLayer;

        // Find all overlapping Colliders
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(layerMask);
        Collider2D[] colliders = new Collider2D[maxCollidors];
        int collidersAmount = GetComponent<Collider2D>().OverlapCollider(contactFilter, colliders);

        return (collidersAmount, colliders);
    }
        
}
