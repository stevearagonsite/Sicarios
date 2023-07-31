using System;
using UnityEngine;

namespace MyContent.Scripts{
    public static class Consts{
        public static readonly Action NOOB = () => {};
        public const float AGENT_MIN_MOVEMENT_SPEED = 2;
        public const float AGENT_MAX_MOVEMENT_SPEED = 3;
        public static readonly Color AGENT_SICARIO_COLOR = Color.red;
        public static readonly Color AGENT_DETECTIVE_COLOR = Color.blue;
        public static readonly Color AGENT_PERSON_COLOR = Color.white;
        public static readonly Color AGENT_TARGET_COLOR = Color.green;
        public static readonly Color AGENT_DEAD_COLOR = Color.black;
    }
    
    public static class Layers{
        public const int TERRAIN_NUM_LAYER = 8;
        public const string TERRAIN_LABEL_LAYER = "Terrain";
        public const int AGENT_NUM_LAYER  = 10;
        public const string AGENT_LABEL_LAYER  = "Agent";
    }

}