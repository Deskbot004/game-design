using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum DroppablePriorities {
    CARD,
    SLOT,
    TABLE
}

public interface Droppable
{
    bool DropActive {get; set;}
    int Priority {get;}
    Transform transform {get;}

    public bool OnDrop(Draggable draggedObject); // Is called when a Draggable object is dropped into this object
    public void OnLeave(Draggable draggedObject); // Is called when a Draggable Object that started in this Droppable is dropped into another Droppable
}
