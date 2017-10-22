//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Services.Game.Components.ViewComponent view { get { return (Services.Game.Components.ViewComponent)GetComponent(GameComponentsLookup.View); } }
    public bool hasView { get { return HasComponent(GameComponentsLookup.View); } }

    public void AddView(UnityEngine.GameObject newView) {
        var index = GameComponentsLookup.View;
        var component = CreateComponent<Services.Game.Components.ViewComponent>(index);
        component.view = newView;
        AddComponent(index, component);
    }

    public void ReplaceView(UnityEngine.GameObject newView) {
        var index = GameComponentsLookup.View;
        var component = CreateComponent<Services.Game.Components.ViewComponent>(index);
        component.view = newView;
        ReplaceComponent(index, component);
    }

    public void RemoveView() {
        RemoveComponent(GameComponentsLookup.View);
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

    static Entitas.IMatcher<GameEntity> _matcherView;

    public static Entitas.IMatcher<GameEntity> View {
        get {
            if (_matcherView == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.View);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherView = matcher;
            }

            return _matcherView;
        }
    }
}