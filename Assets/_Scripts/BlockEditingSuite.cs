using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public static GameController world;

    protected bool m_isCompleted = false;
    public abstract void Execute();
    public abstract bool IsCompleted();
}

public class AddBlockCommand : Command
{
    IntPos target_position;
    char blockType;

    public AddBlockCommand(char placedBlockType, IntPos targetPosition)
    {
        target_position = targetPosition;
        blockType = placedBlockType;
    }
    public AddBlockCommand(char placedBlockType, Vector3 targetPosition)
    {
        target_position = new IntPos(targetPosition);
        blockType = placedBlockType;
    }

    override public void Execute()
    {
        world.PlaceBlock(blockType, target_position);
        m_isCompleted = true;
    }

    public override bool IsCompleted()
    {
        return m_isCompleted;
    }

}

public class BlockEditingSuite : MonoBehaviour {

    private LinkedList<Command> commandList;

    public float m_maxBlockPlacingRange = 5.0f;

    public char blockTypeSelection = (char)1;

    // Use this for initialization
    void Start () {
        commandList = new LinkedList<Command>();
    }
	
	// Update is called once per frame
	void Update () {

        bool raycastConnected = false;

        RaycastHit hit;

        // only collide raycast with Blocks layer
        int layerMaskBlocksOnly = 1 << 9;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, m_maxBlockPlacingRange, layerMaskBlocksOnly))
        {
            raycastConnected = true;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.white);
        }


        // Place Block
        if (Input.GetButtonDown("PlaceBlock") || Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(raycastConnected)
            {
                Debug.Log("Placing block!");
                Execute(new AddBlockCommand(blockTypeSelection, (hit.point) + (hit.normal * 0.5f) + new Vector3(0.5f, 0.5f, 0.5f)));
            } else
            {
                //Debug.Log("No surface to place block on");
            }
        }

    }

    public void Execute(Command command)
    {
        commandList.AddLast(command);
        commandList.Last.Value.Execute();
    }

    public void Add(Command command)
    {
        commandList.AddLast(command);
    }

}
