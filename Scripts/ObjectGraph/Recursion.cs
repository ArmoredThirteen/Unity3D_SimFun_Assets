using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recursion : MonoBehaviour
{
    public int recurseOneCount = 3;
    int recurseTwoCount = 2;


    [ContextMenu ("RecurseOne")]
    public void RecurseOne_Menu()
    {
        RecurseOne (recurseOneCount);
    }

    [ContextMenu ("RecurseTwo")]
    public void RecurseTwo_Menu()
    {
        RecurseTwo (recurseTwoCount);
    }



    public void RecurseOne(int iteration)
    {
        if (iteration <= 0)
            return;

        iteration = iteration - 1;

        Debug.Log ("First_" + iteration);
        RecurseOne (iteration);
    }

    
    public void RecurseTwo(int iteration)
    {
        if (iteration <= 0)
            return;

        iteration = iteration - 1;

        Debug.Log ("First_" + iteration);
        RecurseTwo (iteration);

        Debug.Log ("Second_" + iteration);
        RecurseTwo (iteration);
    }

}
