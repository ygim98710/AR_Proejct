using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System;
public class tracking_test : MonoBehaviour
{
    //https://www.innerdrivestudios.com/blog/controlling-content-visibility-through-vumark-ids/
    //I have referred to this site.

    List<VuMarkBehaviour> registeredBehaviours = new List<VuMarkBehaviour>();

    public int index;
    private word script;

    private void Start()
    {
        TrackerManager.Instance.GetStateManager().GetVuMarkManager().
            RegisterVuMarkBehaviourDetectedCallback(onVuMarkBehaviourFound);

    }

    private void onVuMarkBehaviourFound(VuMarkBehaviour pVuMarkBehaviour)
    {
        if (registeredBehaviours.Contains(pVuMarkBehaviour)){
        //    Debug.Log("Previously tracked VumarkBehaviour found (" + pVuMarkBehaviour.name + ")");
        }
        else
        {
        //    Debug.Log("Newly tracked VumarkBehaviour found (" + pVuMarkBehaviour.name + ")");
        //    Debug.Log("Registering for VuMarkTargetAssignedCallbacks from " + pVuMarkBehaviour.name);

            //if we hadn't registered yet, we do so now
            registeredBehaviours.Add(pVuMarkBehaviour);

            pVuMarkBehaviour.RegisterVuMarkTargetAssignedCallback(
                () => vumarkTargetAssigned(pVuMarkBehaviour)
            );
        }
    }

    private void vumarkTargetAssigned(VuMarkBehaviour pVuMarkBehaviour)
    {
      //  Debug.Log("VuMarkTarget assigned to " + pVuMarkBehaviour.name + " with ID:" + pVuMarkBehaviour.VuMarkTarget.InstanceId.ToString());

        string myID = GetID(pVuMarkBehaviour.VuMarkTarget.InstanceId);
        
       // Debug.Log("Enabling object with ID:" + myID + " ....");

        foreach (Transform child in pVuMarkBehaviour.transform){
            //Debug.Log("Matching gameObject " + child.name + " with ID " + myID + " SetActive (" + (myID == child.name) + ")");
            //child.gameObject.SetActive(myID == child.name);
            //Debug.Log("Matching gameObject " + child.name + " with ID " + myID + " SetActive (true)");
            //Debug.Log("wwwww");
            child.gameObject.SetActive(true);
            script = child.GetComponent<word>();
        }
        
        IdCheck(myID);
    }


    public int IDLength = 30;
    private string GetID(InstanceId pInstanceId)
    {
        int inputLength = pInstanceId.StringValue.Length;
        int outputLength = Mathf.Min(IDLength, inputLength);
        string subString = pInstanceId.StringValue.Substring(0, outputLength);
        return subString;
    }

    void IdCheck(string id)
    {
        //Debug.Log(id);

        int id2int = Int32.Parse(id);
        int alpha = (Convert.ToInt32(string.Format("{0:00}", id2int))+'A');
        //Debug.Log("origin : " + script.myWord);
        script.myWord = (char)alpha;
        //Debug.Log("alpha(int) : " + alpha + ", alpha (char) : " + (char)alpha);
        Debug.Log("alpha(int) : " + alpha +", alpha (char) : "+(char)alpha+", word : "+script.myWord);
    }
}
