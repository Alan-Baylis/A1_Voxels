using UnityEngine;

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