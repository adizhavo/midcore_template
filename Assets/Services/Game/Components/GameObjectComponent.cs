using Entitas;

namespace Services.Game.Components
{
    [Game]
    public class GameObjectComponent : IComponent
    {
        public string objectId;
        public int uniqueId;
    }
}