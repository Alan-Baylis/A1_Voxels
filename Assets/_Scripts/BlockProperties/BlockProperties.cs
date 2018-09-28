using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockProperties: MonoBehaviour
{
   public BlockProperties()
    {

    }

    public BlockProperties(GameObject prefab, bool transparent = false, bool isSolid = true)
    {
        m_prefab = prefab;
        m_isTransparent = transparent;
        m_canBePlacedUpon = isSolid;
    }

    public GameObject m_prefab = null; //{ get; protected set; }

    // can anything be seen beyond it?
    public bool m_isTransparent = false; //{ get; private set; }

    // can a block be placed onto it?
    public bool m_canBePlacedUpon = true; //{ get; private set; }

    public string m_description = ""; //{ get; set; }
}
