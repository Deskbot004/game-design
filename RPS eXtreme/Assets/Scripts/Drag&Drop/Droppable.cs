using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Droppable
{
    public bool OnDrop(Draggable draggedObject); // Is called when a Draggable object is dropped into this object
    public void OnLeave(Draggable draggedObject); // Is called when a Draggable Object that started in this Droppable is dropped into another Droppable
    public Transform GetTransform(); // Returns the Transform of this Object (needs to be a function, as an interface doesn't have a Transform itself)
}
