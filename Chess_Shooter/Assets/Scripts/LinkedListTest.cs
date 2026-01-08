using UnityEngine;

public class LinkedListTest : MonoBehaviour
{

    void Start()
    {
        Node head = new Node(61);
        head.Append(new Node(62));
        head.Append(new Node(63));
        head.Append(new Node(64));
        head.Append(new Node(65));
        head.Append(new Node(66));
        //Rule of Six... 

        PrintList(head);

        head.Remove(3);

        PrintList(head);

        head.Insert(new Node(44), 4);

        PrintList(head);
    }

    public void PrintList(Node head) //don't execute this function on a circular LinkedList unless you want to summon Ouroboros.
    {
        Node current = head;
        string output = "";
        while (current != null)
        {
            output += current.value.ToString() + " -> ";
            current = current.next;
        }
        Debug.Log(output);
    }


}
