using System.Collections;
using System.Collections.Generic;
using UnityEngine;

private List<Command> commandList;

public abstract class Command
{

}

public class AddBlockCommand : Command
{

}

public class BlockEditingSuite : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void execute(Command command)
    {
        commandList.Add(command);
    }

}
