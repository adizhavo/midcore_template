using Entitas;
using UnityEngine;

namespace Services.Game.Components
{
    public class CellComponent : IComponent
    {
        public int row;
        public int column;
        public string typeId;
        public string objectId;
        public GameEntity occupant;
    }
}