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
    public static WorldController s_world;
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
        m_isCompleted = s_world.PlaceBlock(blockType, m_targetPosition);
    }
    public override void Undo()
    {
        if (m_isCompleted)
        {
            if (s_world.RemoveBlock(m_targetPosition))
            {
                m_isCompleted = false;
            }
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
        m_isCompleted = s_world.RemoveBlock(m_targetPosition);
        Debug.Log("Remove block command executed! block type: " + (int)blockTypeToRemove + "position: " + m_targetPosition.x + "," + m_targetPosition.y + "," + m_targetPosition.z);
    }

    public override bool IsCompleted()
    {
        return m_isCompleted;
    }
    public override void Undo()
    {
        if (m_isCompleted)
        {
            if (s_world.PlaceBlock(blockTypeToRemove, m_targetPosition))
            {
                m_isCompleted = false;
            }
        }
    }
}

public class BlockEditingSuite : MonoBehaviour
{

    private LinkedList<Command> commandList;

    public float m_maxBlockPlacingRange = 6.0f;

    public BLOCK_ID blockTypeSelection = BLOCK_ID.DIRT;

    public GhostBlockProbe ghostBlock;

    // Use this for initialization
    void Start()
    {
        commandList = new LinkedList<Command>();
    }

    // Update is called once per frame
    void Update()
    {

        bool raycastHit = false;

        RaycastHit hit;

        // only collide raycast with Blocks layer
        int layerMaskBlocksOnly = LayerMask.GetMask("Blocks");

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, m_maxBlockPlacingRange, layerMaskBlocksOnly))
        {
            raycastHit = true;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            blockTypeSelection = BLOCK_ID.DIRT;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            blockTypeSelection = BLOCK_ID.GRASS;
        }

        // if selection within range
        if (raycastHit)
        {
            // determine if block place position is too close to the player
            Vector3 blockPlacePosition = new IntPos((hit.point) + (hit.normal * 0.5f) + new Vector3(0.5f, 0.5f, 0.5f)).Vec3();//(new IntPos(((hit.point) + (hit.normal * 0.1f))).Vec3() + new Vector3(0.5f, 0.5f, 0.5f));

            // put visible ghost block there and make it visible
            ghostBlock.transform.position = blockPlacePosition;
            ghostBlock.gameObject.SetActive(true);

            { // Debug block selection
                Debug.DrawRay(blockPlacePosition, new Vector3(0.5f, 0.0f, 0.0f), Color.red);
                Debug.DrawRay(blockPlacePosition, new Vector3(0.0f, 0.5f, 0.0f), Color.green);
                Debug.DrawRay(blockPlacePosition, new Vector3(0.0f, 0.0f, 0.5f), Color.blue);

                Debug.DrawRay(blockPlacePosition, new Vector3(0.0f, -0.5f, 0.0f), Color.yellow);
            }


            // Place Block
            if (Input.GetButtonDown("PlaceBlock"))
            {
                // determine if block place position is too close to the player

                if (!ghostBlock.isColliding)//!boxCastHit)
                {
                    Debug.Log("Placing block!");
                    Command cmd = new AddBlockCommand((char)blockTypeSelection, blockPlacePosition);
                    Execute(ref cmd);

                }   // endif ghostBlock colliding
                else
                {
                    Debug.Log("Cannot place block--Entity is in the way!");
                }
            }

            // Remove Block
            if (Input.GetButtonDown("RemoveBlock"))
            {
                Vector3 blockRemovePosition = (hit.point) + (hit.normal * -0.5f) + new Vector3(0.5f, 0.5f, 0.5f);

                Debug.Log("Removing block!");
                Command cmd = new RemoveBlockCommand(blockRemovePosition);
                Execute(ref cmd);

            }
        }
        else // endif raycast hit
        {
            ghostBlock.gameObject.SetActive(false);
        }

        // Undo Last
        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (commandList.Count > 0)
            {
                var lastAction = commandList.Last;

                if (lastAction.Value.IsCompleted())
                {
                    lastAction.Value.Undo();
                }
                else
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

    /// <summary>
    /// executes passed command
    /// </summary>
    /// <param name="command"></param>
    public void Execute(ref Command command)
    {
        command.Execute();
        if (command.IsCompleted())
        {
            commandList.AddLast(command);
        }
    }

    public void Add(Command command)
    {
        commandList.AddLast(command);
    }
}
