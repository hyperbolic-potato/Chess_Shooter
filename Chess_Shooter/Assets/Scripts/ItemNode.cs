using UnityEngine;
//for use in a doubly linked list of items
public class ItemNode
{
    public GameObject data;

    public ItemNode previous;
    public ItemNode next;

    

    public ItemNode(GameObject newItem)
    {
        data = newItem;
        previous = null;
        next = null;
    }

    public void Append(ItemNode newNode)
    {
        ItemNode current = this; //make reference to the current node



        while (current.next != null) //traverse to the end of the list
        {
            current = current.next;
        }



        current.next = newNode; //add the node in question to the very end
        newNode.previous = current;

    }

    public void Insert(ItemNode newNode, int index)
    {
        ItemNode current = this;

        if (index <= 0) return; //linked lists inherently don't support negative indecies and I'm not dealing with prependations right now

        for (int i = 0; i < index - 1 && current != null; i++) //traversing to right in front of the designated new node position. Make sure not to careen into the void!
        {
            current = current.next;
        }

        if (current == null) return; // what did I say about the void, hmm?

        newNode.next = current.next; //place the new node in place BEFORE changing the current node's reference
        current.next = newNode;

        newNode.next.previous = newNode;
        newNode.previous = current;
    }

    public void Remove(int index)
    {
        ItemNode current = this;

        if (index <= 0) return;

        //allowing index zero would make it possible to delete the head node so thats just not gonna be allowed.
        //(removing the head node, as far as I can tell) always needs some juggling outside the node class in question.

        for (int i = 0; i < index - 1 && current != null; i++) //traversing same way as insert
        {
            current = current.next;
        }

        if (current == null) return;

        

        current.next = current.next.next;

        current.next.previous = current;

        //this is C# so I'll just let the garbage collector deal with the abandoned node
    }
}
