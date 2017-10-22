using Entitas;

namespace Services.Game.Grid
{
    /// <summary> 
    /// Inherit and extend the 'GetCell' method to provide more type of cells
    /// </summary>

    public class CellFactory
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

    public class Cell
    {
        public int x;
        public int y;
        public string typeId;
        public string objectId;
        public Entity occupant;

        public Cell(string typeId)
        {
            this.typeId = typeId;
        }
    }

    public class NullCell : Cell
    {
        public NullCell() : base(string.Empty)
        {
            x = -1;
            y = -1;
        }
    }
}