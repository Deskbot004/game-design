using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    // How many overlapping colliders can be checked
    // This is needed, because Unity's collider overlap function needs an initiated array of this size
    private int maxCollidors = 10; 

    private Vector3 positionBeforePickup;
    private Vector3 rotationBeforePickup;
    private Droppable currentDroppable;

    #region Main Functions ---------------------------------------------------------------------------------------------
    
    void Start() {
        Debug.Assert(GetComponent<Collider2D>() != null, "Draggable Object is missing a 2D Collider!", this);
        FindStartingDroppable();
    }

    // Is called on Pickup
    void OnMouseDown() {
        if (!enabled || !GetComponent<Card>().BelongsToPlayer()) return; //Prevents Dragging when this component is disabled 
        SavePosition();
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    // Is called while Dragging
    void OnMouseDrag() {
        if (!enabled || !GetComponent<Card>().BelongsToPlayer()) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -7;
        transform.position = mousePosition;
    }

    // Is called on Drop
    void OnMouseUp() {
        if (!enabled || !GetComponent<Card>().BelongsToPlayer()) return;
        Collider2D[] colliders = GetOverlappedDroppables();
        foreach (Collider2D collider in colliders) {
            Droppable newDroppable = collider.GetComponent<Droppable>();
            bool dropSuccess = DropInto(newDroppable);
            if (dropSuccess)
                return;
        }
        RestorePosition();
    }
    
    #endregion

    #region Helper Functions ---------------------------------------------------------------------------------------------
    
    public void SavePosition() {
        positionBeforePickup = transform.position;
        rotationBeforePickup = transform.eulerAngles;
    }

    public void RestorePosition() {
        transform.position = positionBeforePickup;
        transform.eulerAngles = rotationBeforePickup;
    }

    public Collider2D[] GetOverlappedDroppables() { 
        // Find all overlapping Colliders on Droppable layer
        Collider2D[] colliders = new Collider2D[maxCollidors];
        LayerMask droppableLayerMask = LayerMask.GetMask("Droppable");
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(droppableLayerMask);
        GetComponent<Collider2D>().OverlapCollider(contactFilter, colliders);

        // Filter and Sort
        colliders = colliders.Where(c => c != null).ToArray(); // null entries
        colliders = colliders.Where(c => c.GetComponent<Droppable>() != null).ToArray(); // non droppables
        colliders = colliders.Where(c => c.GetComponent<Droppable>().DropActive).ToArray(); // inactive doppables
        colliders = colliders.OrderBy(c => c.GetComponent<Droppable>().Priority).ToArray();

        return  colliders;
    }

    public bool DropInto(Droppable newDroppable) {
        if (newDroppable == currentDroppable) {
            RestorePosition();
            return true;
        }

        bool dropSuccess = newDroppable.OnDrop(this);
        if (dropSuccess) {
            currentDroppable.OnLeave(this);
            currentDroppable = newDroppable;
        }
        return dropSuccess;
    }

    #endregion

    #region Getter und Setter ---------------------------------------------------------------------------------------------

    public void FindStartingDroppable() {
        Collider2D[] colliders = GetOverlappedDroppables();
        int colAmount = colliders.Length;
        Debug.Assert(colAmount < 2, "Multiple Start-Droppables found", this);
        if (colAmount > 0)
            currentDroppable = colliders[0].GetComponent<Droppable>();

        Debug.Assert(1 == 1, "test");
    }

    // TODO: This shouldn't be needed anymore after refactoring -> DropInto should call it
    public void SetCurrentDroppable(Droppable droppable) {
        currentDroppable = droppable;
    }

    #endregion
        
}
