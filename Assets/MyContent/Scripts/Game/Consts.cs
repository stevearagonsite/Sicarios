using System;
using UnityEngine;

namespace MyContent.Scripts{
    public static class Consts{
        public static readonly Action NOOB = () => {};
        
        // Sicario
        public static readonly Color AGENT_SICARIO_COLOR = Color.red;
        public const float AGENT_SICARIO_MIN_MOVEMENT_SPEED = 1;
        public const float AGENT_SICARIO_MAX_MOVEMENT_SPEED = 5;

        // Agent
        public static readonly Color AGENT_DETECTIVE_COLOR = Color.blue;
        public const float AGENT_DETECTIVE_MIN_MOVEMENT_SPEED = 1;
        public const float AGENT_DETECTIVE_MAX_MOVEMENT_SPEED = 5;
        
        // Person
        public static readonly Color AGENT_PERSON_COLOR = Color.white;
        public const float AGENT_PERSON_MIN_MOVEMENT_SPEED = 1;
        public const float AGENT_PERSON_MAX_MOVEMENT_SPEED = 5;
        
        // Target
        public static readonly Color AGENT_TARGET_COLOR = Color.green;
        public const float AGENT_TARGET_MIN_MOVEMENT_SPEED = 1;
        public const float AGENT_TARGET_MAX_MOVEMENT_SPEED = 5;
        
        public static readonly Color AGENT_DEAD_COLOR = Color.black;
    }
    
    public static class Layers{
        public const int TERRAIN_NUM_LAYER = 8;
        public const string TERRAIN_LABEL_LAYER = "Terrain";
        public const int AGENT_NUM_LAYER  = 11;
        public const string AGENT_LABEL_LAYER  = "Agent";
    }

}