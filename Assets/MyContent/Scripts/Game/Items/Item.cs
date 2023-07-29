using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Invalid,
    Key,
    Door,
    Entity,
    Mace,
    PastaFrola,
}


public class Item : MonoBehaviour
{
    public ItemType type;
    Waypoint _wp;
    bool insideInventory;
    
    public void OnInventoryAdd()
    {
        Destroy(GetComponent<Rigidbody>());
        insideInventory = true;
        if(_wp)
            _wp.nearbyItems.Remove(this);
    }

    public void OnInventoryRemove()
    {
        gameObject.AddComponent<Rigidbody>();
        insideInventory = false;
    }
    
    private void Start ()
    {
        _wp = Navigation.instance.NearestTo(transform.position);
        _wp.nearbyItems.Add(this);	
    }

    public void Kill()
    {
        // var ent = GetComponent<Entity>();
        // if(ent != null)
        // {
        //     foreach(var it in ent.RemoveAllitems())
        //         it.transform.parent = null;
        // }
        // DestroyObject(gameObject);
    }
    
    private void OnDestroy()
    {
        _wp.nearbyItems.Remove(this);
    }
    

    // Update is called once per frame
    private void Update () {
        if (insideInventory) return;
        _wp.nearbyItems.Remove(this);
        _wp = Navigation.instance.NearestTo(transform.position);
        _wp.nearbyItems.Add(this);
    }
}
