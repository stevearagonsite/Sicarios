using System;
using UnityEngine;

namespace Items {
    public class Item : MonoBehaviour
    {
        public string type = "With out type";
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
    
        private void Start () {
            var nav = Navigation.instance;
            if (nav == null) throw new Exception("Navigation not found");
            // Prevent errors about async loading
            if (nav.waypointsCount > 0) {
                UpdateWaypoints();
            }
            nav.SubscribeWaypointsChange(this);
        }

        public void UpdateWaypoints() {
            _wp = Navigation.instance.NearestTo(transform.position);
            _wp.nearbyItems.Add(this);	
        }

        public void Kill()
        {
            throw new NotImplementedException();
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
}
