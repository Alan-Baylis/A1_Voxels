using System.Collections.Generic;
using UnityEngine;
using IObserverPattern;
using UnityEngine.UI;

[System.Serializable]
public abstract class Command
{
    protected bool m_isCompleted = false;
    public abstract bool Execute();
    public abstract bool IsCompleted();
    public abstract bool Undo();
}

[System.Serializable]
public abstract class BlockCommand : Command
{
    public static WorldController s_world;
    protected IntPos m_targetPosition;
    protected IntPos m_targetOrientation;
}



public class BlockEditingSuite : IObservable
{
    public WorldController world;
    public BlockDatabase blockDatabase;

    private LinkedList<Command> commandList;

    public float m_maxBlockPlacingRange = 6.0f;

    public BLOCK_ID blockTypeSelection = BLOCK_ID.DIRT;

    public GhostBlockProbe ghostBlock;

    public Text blockDescriptionUI;

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

        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                blockTypeSelection = BLOCK_ID.DIRT;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                blockTypeSelection = BLOCK_ID.GRASS;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                blockTypeSelection = BLOCK_ID.MARBLE;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                blockTypeSelection = BLOCK_ID.COLUMN_BASE;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                blockTypeSelection = BLOCK_ID.COLUMN_MID;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                blockTypeSelection = BLOCK_ID.COLUMN_TOP;
            }
        }

        // if selection within range
        if (raycastHit)
        {
            // determine if block place position is too close to the player
            Vector3 blockPlacePosition = new IntPos((hit.point) + (hit.normal * 0.5f) + new Vector3(0.5f, 0.5f, 0.5f)).Vec3();//(new IntPos(((hit.point) + (hit.normal * 0.1f))).Vec3() + new Vector3(0.5f, 0.5f, 0.5f));
            IntPos integerPlacePosition = new IntPos(blockPlacePosition);
            

            // position of the block the raycast hit
            IntPos hitBlockPosition = new IntPos((hit.point) + (hit.normal * -0.5f) + new Vector3(0.5f, 0.5f, 0.5f));

            byte hitBlockType = world.GetBlockID(hitBlockPosition);

            // get all properties of the block the raycast hit
            BlockProperties hitBlockProperties = blockDatabase.GetProperties((BLOCK_ID)hitBlockType);

            // show description text for block
            blockDescriptionUI.text = hitBlockProperties.m_description;

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
                if (!ghostBlock.IsColliding() && hitBlockProperties.m_canBePlacedUpon)
                {
                    Debug.Log("Placing block!");
                    Command cmd = new AddBlockCommand((byte)blockTypeSelection, integerPlacePosition);

                    if (Execute(ref cmd))
                    {
                        // send notification that a Block was placed
                        NotifyAll(gameObject, OBSERVER_EVENT.PLACED_BLOCK);
                    }

                }   // endif ghostBlock colliding
                else
                {
                    Debug.Log("Cannot place block--Entity is in the way!");
                }
            }

            // Remove Block
            if (Input.GetButtonDown("RemoveBlock"))
            {
                

                Debug.Log("Removing block!");
                Command cmd = new RemoveBlockCommand(hitBlockType, hitBlockPosition);
                Execute(ref cmd);

            }
        }
        else // endif raycast hit
        {
            ghostBlock.gameObject.SetActive(false);
            blockDescriptionUI.text = "";
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
    public bool Execute(ref Command command)
    {
        bool success = command.Execute();
        if (success)
        {
            commandList.AddLast(command);
        }
        return success;
    }

    public void Add(Command command)
    {
        commandList.AddLast(command);
    }
}
