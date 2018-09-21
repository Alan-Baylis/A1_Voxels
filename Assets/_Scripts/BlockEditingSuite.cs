using System.Collections.Generic;
using UnityEngine;


public abstract class Command
{
    public abstract bool Execute();
}

public class AddBlockCommand : Command
{
    public GameController world;

    IntPos target_position;
    char blockType;

    public AddBlockCommand(char placedBlockType, Vector3 targetPosition)
    {
        target_position = new IntPos(targetPosition);
        blockType = placedBlockType;
    }

    override public bool Execute()
    {
        world.PlaceBlock(blockType, target_position);
        return true;
    }
}

public class BlockEditingSuite : MonoBehaviour {

    private List<Command> commandList;

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
