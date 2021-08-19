using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryObject : MonoBehaviour
{
    [HideInInspector] public ObjectFactory originFactory;

    private int objectId = int.MinValue;

    public int ObjectId
    {
        get
        {
            return objectId;
        }
        set
        {
            if (objectId == int.MinValue && value != int.MinValue)
            {
                objectId = value;
            }
        }
    }

    public virtual void Reclaim()
    {
        if (originFactory != null)
            originFactory.Reclaim(this);
        else
            Destroy(this.gameObject);
    }
}
