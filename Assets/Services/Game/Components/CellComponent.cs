﻿using Entitas;

namespace Services.Game.Components
{
    public class CellComponent : IComponent
    {
        public int row;
        public int column;
        public GameEntity occupant;
    }
}