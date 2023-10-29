using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Droppable
{
    public bool OnDrop(Draggable draggedObject); // Function that is called when a Draggable object is dropped into this object
    public void OnLeave(Draggable draggedObject); // Is called when a Draggable Object that started in this Droppable is dropped into another Droppable

    //public void OnPickup(Draggable draggedObject); // Is called when an object is picked up from this Droppable
    //public void OnEmptyDrop(Draggable draggedObject); // Is called if it wasn't dropped into another object after being picked up from this Droppable
}
