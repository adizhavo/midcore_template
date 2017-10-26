using Entitas;
using Services.Game.Grid;

namespace Services.Game.Factory
{
    /// <summary> 
    /// Inherit and extend the 'GetCell' method to provide more type of cells
    /// with different behaviour
    /// </summary>

    public class FactoryCell
    {
        public T GetCell<T>(string typeId = "") where T : Cell
        {
            return (T)GetCell(typeId);
        }

        public virtual Cell GetCell(string typeId = "")
        {
            return string.IsNullOrEmpty(typeId) ? new Cell(typeId) : default(NullCell);
        }
    }
}