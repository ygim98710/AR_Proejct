using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class word : MonoBehaviour
{
    public char myWord;

    void Start()
    {
        myWord = ' ';
    }

    void OnDrawGizmos()
    {
        if (myWord == 'F')
            Gizmos.color = Color.blue;
        else if (myWord == 'A')
            Gizmos.color = Color.red;
        else if (myWord == 'N')
            Gizmos.color = Color.cyan;
        else if (myWord == 'G')
            Gizmos.color = Color.green;
        else if (myWord == 'E')
            Gizmos.color = Color.white;
        else if (myWord == 'B')
            Gizmos.color = Color.black;
        else if (myWord == 'Y')
            Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 2);
    }
}
