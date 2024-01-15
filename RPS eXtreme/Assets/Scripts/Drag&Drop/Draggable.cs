using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private int maxCollidors = 10; // How many overlapping colliders can be checked

    private Vector3 startPosition;
    private Vector3 startRotation;
    private Droppable currentDroppable; // The Droppable Object it's currently in
    private Droppable defaultDroppable; // Handles drops, when this Draggable isn't in a Droppable

    //TODO: In Unity, layer the card properly


    // ---------- Main Functions -------------------------------------------------------------------------------------------
    void Start()
    {
        // Make sure that the draggable Object has a Box Collider 
        Debug.Assert(GetComponent<Collider2D>() != null, "Draggable Object is missing a 2D Collider!", this);

        // Find and set default Droppable
        defaultDroppable = GetComponentInParent<DefaultDroppable>();
        Debug.Assert(defaultDroppable != null, "Couldn't find default Droppable", this);

        // Check whether Object is already inside a Droppable
        (int colAmount, Collider2D[] colliders) = GetOverlappedDroppable();
        Debug.Assert(colAmount < 2, "Multiple Start-Droppables found", this);
        if (colAmount > 0)
            currentDroppable = colliders[0].GetComponent<Droppable>();
        else
            currentDroppable = defaultDroppable;
    }

    // Is called on Pickup
    void OnMouseDown()
    {
        if (!enabled) return; //Prevents Dragging when this component is disabled
        SavePosition();
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    // Is called while Dragging
    void OnMouseDrag()
    {
        if (!enabled) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -7;
        transform.position = mousePos;
    }

    // Is called on Drop
    void OnMouseUp() 
    {
        if (!enabled) return;
        (int colAmount, Collider2D[] colliders) = GetOverlappedDroppable();
        Droppable newDroppable;

        // Check whether it was dropped inside a Droppable that's a child of the default Droppable
        if(colAmount == 0) // Not the case
        {
            newDroppable = defaultDroppable;
        }
        else // Yes the case
        {
            Debug.Assert(colliders[0].GetComponent<Droppable>() != null, "Destination Object doesn't have Droppable Component", colliders[0]);
            newDroppable = colliders[0].GetComponent<Droppable>();
        }

        // Handle the drop itself
        if (newDroppable == currentDroppable) RestorePosition();
        else
        {
            if (newDroppable.OnDrop(this))
            {
                currentDroppable.OnLeave(this);
                currentDroppable = newDroppable;
            }
            else RestorePosition();
        }
    }

    // ---------- Helper Functions -------------------------------------------------------------------------------------------
    // Saves the current global position to the startPosition/Rotation Variables
    public void SavePosition()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    // Restores the position from the startPosition/Rotation Variables
    public void RestorePosition()
    {
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }

    // Returns all Droppable Components on the "Droppable" Layer, that collide with this Draggable
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

        // Filter out null entries and inactive droppables
        colliders = colliders.Where(c => c != null).ToArray();
        if (collidersAmount > 0) colliders = colliders.Where(c => c.GetComponent<Droppable>().DropActive).ToArray(); //Filter out inactive droppables
        if (collidersAmount > 0) colliders = colliders.Where(c => c.transform.IsChildOf(defaultDroppable.GetTransform())).ToArray(); //Filter out droppables from other parents
        collidersAmount = colliders.Length;

        return (collidersAmount, colliders);
    }
        
}
