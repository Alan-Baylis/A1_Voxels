using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    protected bool m_isCompleted = false;
    public abstract void Execute();
    public abstract bool IsCompleted();
    public abstract void Undo();
}

public abstract class BlockCommand : Command
{
    public static GameController s_world;
    protected IntPos m_targetPosition;
}

public class AddBlockCommand : BlockCommand
{
    char blockType;

    public AddBlockCommand(char placedBlockType, IntPos targetPosition)
    {
        m_targetPosition = targetPosition;
        blockType = placedBlockType;
    }
    public AddBlockCommand(char placedBlockType, Vector3 targetPosition)
    {
        m_targetPosition = new IntPos(targetPosition);
        blockType = placedBlockType;
    }

    override public void Execute()
    {
        s_world.PlaceBlock(blockType, m_targetPosition);
        m_isCompleted = true;
    }
    public override void Undo()
    {
        if(m_isCompleted)
        {
            s_world.RemoveBlock(m_targetPosition);
            m_isCompleted = false;
        }
    }

    public override bool IsCompleted()
    {
        return m_isCompleted;
    }
}

public class RemoveBlockCommand : BlockCommand
{
    char blockTypeToRemove; 

    public RemoveBlockCommand(Vector3 targetPosition)
    {
        m_targetPosition = new IntPos(targetPosition);
        blockTypeToRemove = s_world.GetBlockID(m_targetPosition);
    }

    override public void Execute()
    {
        s_world.RemoveBlock(m_targetPosition);
        m_isCompleted = true;
    }

    public override bool IsCompleted()
    {
        return m_isCompleted;
    }
    public override void Undo()
    {
        if (m_isCompleted)
        {
            s_world.PlaceBlock(blockTypeToRemove, m_targetPosition);
            m_isCompleted = false;
        }
    }
}

public class BlockEditingSuite : MonoBehaviour {

    private LinkedList<Command> commandList;

    public float m_maxBlockPlacingRange = 6.0f;

    public BLOCK_ID blockTypeSelection = BLOCK_ID.DIRT;

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            blockTypeSelection = BLOCK_ID.DIRT;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            blockTypeSelection = BLOCK_ID.GRASS;
        }

        // Place Block
        if (Input.GetButtonDown("PlaceBlock"))
        {
            if(raycastConnected)
            {
                Debug.Log("Placing block!");
                Execute(new AddBlockCommand((char)blockTypeSelection, (hit.point) + (hit.normal * 0.5f) + new Vector3(0.5f, 0.5f, 0.5f)));
            } else
            {
                //Debug.Log("No surface to place block on");
            }
        }

        // Remove Block
        if (Input.GetButtonDown("RemoveBlock"))
        {
            if (raycastConnected)
            {
                Debug.Log("Removing block!");
                Execute(new RemoveBlockCommand((hit.point) + (hit.normal * -0.5f) + new Vector3(0.5f, 0.5f, 0.5f)));
            }
            else
            {
                //Debug.Log("No surface to place block on");
            }
        }

        // Undo Last
        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (commandList.Count > 0)
            {
                var lastAction = commandList.Last;

                if(lastAction.Value.IsCompleted())
                {
                    lastAction.Value.Undo();
                } else
                {
                    Debug.Log("Could not Undo! Action not completed! Removing uncompleted action");
                }

                commandList.RemoveLast();
            }
            else
            {
                Debug.Log("Could not Undo! Nothing to Undo!");
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
