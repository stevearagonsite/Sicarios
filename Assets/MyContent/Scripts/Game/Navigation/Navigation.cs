using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using LINQExtension;
using UnityEngine;
using Debug = Logger.Debug;

public class Navigation : MonoBehaviour {
    public static Navigation instance;
    private List<Waypoint> _waypoints = new List<Waypoint>();
    public int waypointsCount => _waypoints.Count;
    public event Action OnWaypointsChanged = delegate { };

    private void Awake() {
        // Singleton
        if (!instance) {
            instance = this;
        }
        else {
            instance = null;
            instance = this;
        }
    }

    private void Start() {
        // Get all waypoints
        _waypoints = this
            .GetComponentsInChildren<ConnectorWaypoints>()
            .SelectMany(cw => cw.AllWaypoints)
            .ToList();
        OnWaypointsChanged();
#if UNITY_EDITOR
        Debug.Log(this, "Waypoints: " + _waypoints.Count);
#endif
    }

    public IEnumerable<Item> AllInventories() {
        return AllItems()
            .Select(x => x.GetComponent<BaseAgent>())
            .Where(x => x != null)
            .Aggregate(FList.Create<Item>(), (a, x) => a + x.items);
    }


    public IEnumerable<Item> AllItems() {
        return All().Aggregate(FList.Create<Item>(), (a, wp) => a += wp.nearbyItems);
    }

    public bool Reachable(Vector3 from, Vector3 to, List<Tuple<Vector3, Vector3>> debugRayList) {
        var srcWp = NearestTo(from);
        var dstWp = NearestTo(to);

        Waypoint wp = srcWp;

        if (srcWp != dstWp) {
            var path = AStarNormal<Waypoint>.Run(
                srcWp
                , dstWp
                , (wa, wb) => Vector3.Distance(wa.transform.position, wb.transform.position)
                , w => w == dstWp
                , w =>
                    w.adyacent
                        //TODO: Pending (search items)!!
                        // .Where(a => a.nearbyItems.All(it => it.type != ItemType.Door))
                        .Select(a =>
                            new AStarNormal<Waypoint>.Arc(a,
                                Vector3.Distance(a.transform.position, w.transform.position)))
            );

            if (path == null) return false;

            wp = path.Last();
        }


        Debug.Log("Reachable from " + wp.name);
        if (debugRayList != null) debugRayList.Add(Tuple.Create(wp.transform.position, to));

        var delta = (to - wp.transform.position);
        var distance = delta.magnitude;

        return !Physics.Raycast(wp.transform.position, delta / distance, distance,
            LayerMask.GetMask(new[] { "Blocking" }));
    }

    public IEnumerable<Waypoint> All() {
        return _waypoints;
    }

    public Waypoint NearestTo(Vector3 pos) {
        return All()
            .OrderBy(wp => {
                var d = wp.transform.position - pos;
                d.y = 0;
                return d.sqrMagnitude;
            })
            .First();
    }

    public void SubscribeWaypointsChange(Item item) {
        OnWaypointsChanged += item.UpdateWaypoints;
    }
}