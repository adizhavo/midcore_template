using Entitas;

namespace Services.Game.Components
{
    [Game]
    public class AutoDestroyComponent : IComponent
    {
        public float remainingTime;
        public bool ignoreTimescale;
    }
}

