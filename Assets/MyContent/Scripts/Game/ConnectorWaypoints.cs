using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class ConnectorWaypoints : MonoBehaviour{
    public ConnectorWaypoints toConnectRight;
    public ConnectorWaypoints toConnectLeft;
    public ConnectorWaypoints toConnectFront;
    public ConnectorWaypoints toConnectBack;

    public List<Waypoint> right;
    public List<Waypoint> left;
    public List<Waypoint> front;
    public List<Waypoint> back;

    private Dictionary<PositionConnect, List<Waypoint>> _dictionaryConnection =
        new Dictionary<PositionConnect, List<Waypoint>>() { };

    private Dictionary<PositionConnect, PositionConnect> _negativePositionDictionary =
        new Dictionary<PositionConnect, PositionConnect>() {
            {PositionConnect.RIGHT, PositionConnect.LEFT},
            {PositionConnect.LEFT, PositionConnect.RIGHT},
            {PositionConnect.FRONT, PositionConnect.BACK},
            {PositionConnect.BACK, PositionConnect.FRONT}
        };

    private Dictionary<PositionConnect, ConnectorWaypoints> _connectorDictionary;
    private ConnectorWaypoints[] _allConnections;

    private bool HasConnection {
        get { return _allConnections.Any(connector => connector != null); }
    }

    private void Awake() {
        _connectorDictionary = new Dictionary<PositionConnect, ConnectorWaypoints>() {
            {PositionConnect.RIGHT, toConnectRight},
            {PositionConnect.LEFT, toConnectLeft},
            {PositionConnect.FRONT, toConnectFront},
            {PositionConnect.BACK, toConnectBack}
        };

        _dictionaryConnection.Add(PositionConnect.RIGHT, right);
        _dictionaryConnection.Add(PositionConnect.LEFT, left);
        _dictionaryConnection.Add(PositionConnect.FRONT, front);
        _dictionaryConnection.Add(PositionConnect.BACK, back);

    }

    private void Start() {
        SetMainValues();
    }

    private void SetMainValues() {
        var listOfConnections = new FList<Waypoint>(right) + left + front + back;
        var hasEmptyValue = listOfConnections.Any(connection => connection == null);
        _allConnections = new[] {toConnectRight, toConnectLeft, toConnectFront, toConnectBack};

        if (hasEmptyValue) {
            Debug.LogError("Error has an empty value");
            return;
        }

        if (!HasConnection) {
            Debug.LogWarning($"The all connectors are not exist in {this.gameObject.name}");
            return;
        }
        
        VerifyAndCreateConnection();
    }

    private void OnDestroy() {
        VerifyAndDestroyConnection();
    }

    private void VerifyAndDestroyConnection() {
        if (toConnectRight != null) DestroyConnection(PositionConnect.RIGHT, toConnectRight);
        if (toConnectLeft != null) DestroyConnection(PositionConnect.LEFT, toConnectLeft);
        if (toConnectBack != null) DestroyConnection(PositionConnect.BACK, toConnectBack);
        if (toConnectFront != null) DestroyConnection(PositionConnect.FRONT, toConnectFront);
    }

    private void VerifyAndCreateConnection() {
        if (toConnectRight != null) CreateConnection(PositionConnect.RIGHT, toConnectRight);
        if (toConnectLeft != null) CreateConnection(PositionConnect.LEFT, toConnectLeft);
        if (toConnectBack != null) CreateConnection(PositionConnect.BACK, toConnectBack);
        if (toConnectFront != null) CreateConnection(PositionConnect.FRONT, toConnectFront);
    }

    public List<Waypoint> GetConnectionsByPosition(PositionConnect position) {
        _dictionaryConnection.TryGetValue(position, out var dst);
        return dst;
    }

    public void SetConnectionsByPosition(PositionConnect position, ConnectorWaypoints connector) {
        _dictionaryConnection[position] = new List<Waypoint>();
        SetMainValues();
    }

    private void CreateConnection(PositionConnect position, ConnectorWaypoints toConnect) {
        _dictionaryConnection.TryGetValue(position, out var myPositionsToConnect);
        _negativePositionDictionary.TryGetValue(position, out var negativePosition);
        var positionsToConnect = toConnect.GetConnectionsByPosition(negativePosition);
        
        if (myPositionsToConnect == null || positionsToConnect == null) {
            Debug.LogError($"The connection is not possible has a value null.\n" +
                           $"myPositionsToConnect: {myPositionsToConnect == null}\n" +
                           $"positionsToConnect: {myPositionsToConnect == null}."
            );
            return;
        }
        
        // TODO: Set connector to object toConnect
        for (var i = 0; i < myPositionsToConnect.Count; i++) {
            var toAdd = positionsToConnect[i];
            var toAddReflection = myPositionsToConnect[i];
            
            //Hotfix
            if (toAdd == null) continue;

            myPositionsToConnect[i].adyacent.Add(toAdd);
            positionsToConnect[i].adyacent.Add(toAddReflection);
            
        }
    }

    private void DestroyConnection(PositionConnect position, ConnectorWaypoints toDisconnect) {
        _dictionaryConnection.TryGetValue(position, out var myPositionsToDisconnect);
        var negativePosition = _negativePositionDictionary[position];
        var positionsToDisconnect = toDisconnect.GetConnectionsByPosition(negativePosition);

        if (myPositionsToDisconnect == null || positionsToDisconnect == null) {
            Debug.LogWarning($"The connection is not possible has a value null in {this.gameObject.name}.\n" +
                           $"myPositionsToDisconnect: {myPositionsToDisconnect == null}\n" +
                           $"positionsToDisconnect: {myPositionsToDisconnect == null}."
                           );
            return;
        }

        // TODO: Remove connector to object toDisconnect
        for (var i = 0; i < myPositionsToDisconnect.Count; i++) {
            var toRemove = positionsToDisconnect[i];
            var toRemoveReflection = myPositionsToDisconnect[i];
            
            //Hotfix
            if (toRemove == null) continue;

            myPositionsToDisconnect[i].adyacent.Remove(toRemove);
            positionsToDisconnect[i].adyacent.Remove(toRemove);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        foreach (var toConnect in _allConnections) {
            if (toConnect != null) {
                Gizmos.DrawLine(transform.position + Vector3.up, toConnect.transform.position + Vector3.up);
            }
        }
    }
}

public enum PositionConnect{
    RIGHT,
    LEFT,
    FRONT,
    BACK
}