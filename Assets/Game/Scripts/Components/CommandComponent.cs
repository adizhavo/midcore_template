using Entitas;

namespace MidcoreTemplate.Game.Components
{
    [Game]
    public class CommandComponent : IComponent
    {
        public string onSpawnCommand;
        public string onDragEndCommand;
        public string onTapCommand;
        public string onDestroyCommand;
        public string onTimeoutCommand;
    }
}
