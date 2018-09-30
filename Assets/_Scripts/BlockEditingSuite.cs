using System.Collections.Generic;
using UnityEngine;
using IObserverPattern;
using UnityEngine.UI;

public abstract class Command
{
    protected bool m_isCompleted = false;
    public abstract bool Execute();
    public abstract bool IsCompleted();
    public abstract bool Undo();
}

public abstract class BlockCommand : Command
{
    public static WorldController s_world;
    protected IntPos m_targetPosition;
    protected IntPos m_targetOrientation;
}

public class AddBlockCommand : BlockCommand
{
    byte blockType;

    public AddBlockCommand(byte placedBlockType, IntPos targetPosition)
    {
        m_targetPosition = targetPosition;
        blockType = placedBlockType;
        m_targetOrientation = new IntPos(0, 1, 0);
    }

    public AddBlockCommand(byte placedBlockType, IntPos targetPosition, IntPos targetOrientation)
    {
        m_targetPosition = targetPosition;
        blockType = placedBlockType;
        m_targetOrientation = targetOrientation;
    }

    public AddBlockCommand(byte placedBlockType, Vector3 targetPosition)
    {
        m_targetPosition = new IntPos(targetPosition);
        blockType = placedBlockType;
        m_targetOrientation = new IntPos(0, 1, 0);
    }

    override public bool Execute()
    {
        m_isCompleted = s_world.PlaceBlock(blockType, m_targetPosition);
        return m_isCompleted;
    }
    public override bool Undo()
    {
        bool success = false;

        if (m_isCompleted)
        {
            if (s_world.RemoveBlock(m_targetPosition))
            {
                m_isCompleted = false;
                success = true;
            }
        }

        return success;
    }

    public override bool IsCompleted()
    {
        return m_isCompleted;
    }
}

public class RemoveBlockCommand : BlockCommand
{
    byte blockTypeToRemove;

    public RemoveBlockCommand(Vector3 targetPosition)
    {
        m_targetPosition = new IntPos(targetPosition);
        blockTypeToRemove = s_world.GetBlockID(m_targetPosition);
    }

    public RemoveBlockCommand(byte blockTypeAtPosition, IntPos targetPosition)
    {
        m_targetPosition = targetPosition;
        blockTypeToRemove = blockTypeAtPosition;
    }

    override public bool Execute()
    {
        m_isCompleted = s_world.RemoveBlock(m_targetPosition);
        Debug.Log("Remove block command executed! block type: " + (int)blockTypeToRemove + "position: " + m_targetPosition.x + "," + m_targetPosition.y + "," + m_targetPosition.z);
        return m_isCompleted;
    }

    public override bool IsCompleted()
    {
        return m_isCompleted;
    }

    /// <summary>
    /// Undo the Command if it was done already. Returns true if it was successfuly undone
    /// </summary>
    /// <returns></returns>
    public override bool Undo()
    {
        bool success = false;

        if (m_isCompleted)
        {
            if (s_world.PlaceBlock(blockTypeToRemove, m_targetPosition))
            {
                m_isCompleted = false;
                success = true;
            }
        }

        return success;
    }
}

[System.Serializable]
public class BlockEditingSuite : IObservable
{
    public WorldController world;
    public BlockDatabase blockDatabase;

    private LinkedList<Command> commandList;

    public float m_maxBlockPlacingRange = 6.0f;

    public BLOCK_ID blockTypeSelection = BLOCK_ID.DIRT;

    public GhostBlockProbe ghostBlock;

    public Text blockDescriptionUI;
    public Text currentlySelectedBlockUI;

    [Header("UI Items")]
    public List<GameObject> selectors;
    public List<GameObject> activeItems;
    public List<GameObject> equippedItems;

    public static bool itemsHaveChanged;

    // Use this for initialization
    void Start()
    {
        commandList = new LinkedList<Command>();
        itemsHaveChanged = false;
        _loadItemsFromEquippedList();
    }

    // Update is called once per frame
    void Update()
    {
        if(itemsHaveChanged) {
            _loadItemsFromEquippedList();
            itemsHaveChanged = false;
        }


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
            _hideSelectors();
            selectors[0].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            blockTypeSelection = BLOCK_ID.GRASS;
            _hideSelectors();
            selectors[1].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            blockTypeSelection = BLOCK_ID.MARBLE;
            _hideSelectors();
            selectors[2].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            blockTypeSelection = BLOCK_ID.COLUMN_BASE;
            _hideSelectors();
            selectors[3].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            blockTypeSelection = BLOCK_ID.COLUMN_MID;
            _hideSelectors();
            selectors[4].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            blockTypeSelection = BLOCK_ID.COLUMN_TOP;
            _hideSelectors();
            selectors[5].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            blockTypeSelection = BLOCK_ID.CLAY;
            _hideSelectors();
            selectors[6].SetActive(true);
        }

        /* 
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            blockTypeSelection = BLOCK_ID.BRONZE;
        }
        */

        currentlySelectedBlockUI.text = blockDatabase.GetProperties((BLOCK_ID)blockTypeSelection).m_description;



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

    // private methods

    private void _hideSelectors() {
        foreach (var selector in selectors)
        {
            selector.SetActive(false);
        }
    }

    private void _loadItemsFromEquippedList() {
        foreach (GameObject item in activeItems)
        {
            if(item.transform.childCount>0) {
                Destroy(item.transform.GetChild(0).gameObject);
            }
        }


        for (int count = 0; count < equippedItems.Count; count++)
        {
            if(equippedItems[count].transform.childCount > 0) {
                Transform itemToClone = equippedItems[count].transform.GetChild(0);
                Transform clonedItem = Instantiate(itemToClone);
                clonedItem.localScale = new Vector3(0.5f, 0.5f, 1.0f);
                clonedItem.SetParent(activeItems[count].transform);
            }
        }
    }
}
