//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Services.Game.Components.CellComponent cell { get { return (Services.Game.Components.CellComponent)GetComponent(GameComponentsLookup.Cell); } }
    public bool hasCell { get { return HasComponent(GameComponentsLookup.Cell); } }

    public void AddCell(int newRow, int newColumn, GameEntity newOccupant) {
        var index = GameComponentsLookup.Cell;
        var component = CreateComponent<Services.Game.Components.CellComponent>(index);
        component.row = newRow;
        component.column = newColumn;
        component.occupant = newOccupant;
        AddComponent(index, component);
    }

    public void ReplaceCell(int newRow, int newColumn, GameEntity newOccupant) {
        var index = GameComponentsLookup.Cell;
        var component = CreateComponent<Services.Game.Components.CellComponent>(index);
        component.row = newRow;
        component.column = newColumn;
        component.occupant = newOccupant;
        ReplaceComponent(index, component);
    }

    public void RemoveCell() {
        RemoveComponent(GameComponentsLookup.Cell);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherCell;

    public static Entitas.IMatcher<GameEntity> Cell {
        get {
            if (_matcherCell == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Cell);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherCell = matcher;
            }

            return _matcherCell;
        }
    }
}
