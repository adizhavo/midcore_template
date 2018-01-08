using Entitas;
using UnityEngine;

namespace MidcoreTemplate.Game.Components
{
    [Game]
    public class TimedCommandComponent : IComponent
    {
        public float remainingTime;
        public float maxTime;
    }
}