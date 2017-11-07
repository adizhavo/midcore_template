using Entitas;
using UnityEngine;

namespace MergeWar.Game.Components
{
    [Game]
    public class TimedCommandComponent : IComponent
    {
        public float remainingTime;
        public float maxTime;
    }
}