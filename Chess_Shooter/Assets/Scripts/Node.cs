//basic integer singly linked list component, to get the functions figured out
public class Node
{
    public int value;
    public Node next;

    public Node(int val)
    {

        value = val;
        next = null;
    }

    public void Append(Node newNode)
    {
        Node current = this; //make reference to the current node



        while (current.next != null) //traverse to the end of the list
        {
            current = current.next;
        }

        

        current.next = newNode; //add the node in question to the very end
    }


    public void Insert(Node newNode, int index)
    {
        Node current = this;

        if (index <= 0) return; //linked lists inherently don't support negative indecies and I'm not dealing with prependations right now

        for(int i = 0; i < index - 1 && current != null; i++) //traversing to right in front of the designated new node position. Make sure not to careen into the void!
        {
            current = current.next;
        }

        if(current == null) return; // what did I say about the void, hmm?

        newNode.next = current.next; //place the new node in place BEFORE changing the current node's reference
        current.next = newNode;
    }

    public void Remove(int index)
    {
        Node current = this;

        if (index <= 0) return; //I'm not dealing with whatever index zero would entail here, either

        for (int i = 0; i < index - 1 && current != null; i++) //traversing same way as insert
        {
            current = current.next;
        }

        if (current == null) return;

        current.next = current.next.next;

        //this is C# so I'll just let the garbage collector deal with the abandoned node
    }
}
