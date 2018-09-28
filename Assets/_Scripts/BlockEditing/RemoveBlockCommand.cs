using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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