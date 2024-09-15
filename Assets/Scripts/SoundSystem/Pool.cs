using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool pattern that manages generic type TPoolObj of type Component
/// </summary>
public class Pool<TPoolObj> : MonoBehaviour where TPoolObj : Component
{
    private List<TPoolObj> m_Available;
    private List<TPoolObj> m_InUse;
    [SerializeField]
    protected TPoolObj m_PoolObjPrefab;

    protected void InitializePool(int startCount)
    {
        m_Available = new List<TPoolObj>();
        m_InUse = new List<TPoolObj>();
        for(int i = 0; i < startCount; i++)
        {
            TPoolObj obj = Instantiate<TPoolObj>(m_PoolObjPrefab, transform);
            m_Available.Add(obj);
        }
    }

    protected TPoolObj GetObject()
    {
        TPoolObj result;

        if (m_Available.Count != 0)
        {
            result = m_Available[^1];
            m_Available.RemoveAt(m_Available.Count - 1);
        }
        else 
            result = Instantiate(m_PoolObjPrefab, transform);
        
        m_InUse.Add(result);
        result.gameObject.SetActive(true);
        return result;
    }

    protected void ReleaseObject(TPoolObj poolObj)
    {
        poolObj.gameObject.SetActive(false);
        m_InUse.Remove(poolObj);
        m_Available.Add(poolObj);
    }
}
