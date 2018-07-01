using Entitas;
using UnityEngine;
using MidcoreTemplate.Data;

namespace MidcoreTemplate.Game.Components
{
    [Game]
    public class TimedCommandComponent : IComponent
    {
        public float remainingTime;
        public float maxTime;
        public FloatRange timeout;
    }
}