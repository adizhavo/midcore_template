//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Services.Game.Components.ResourceComponent resource { get { return (Services.Game.Components.ResourceComponent)GetComponent(GameComponentsLookup.Resource); } }
    public bool hasResource { get { return HasComponent(GameComponentsLookup.Resource); } }

    public void AddResource(string newPath) {
        var index = GameComponentsLookup.Resource;
        var component = CreateComponent<Services.Game.Components.ResourceComponent>(index);
        component.path = newPath;
        AddComponent(index, component);
    }

    public void ReplaceResource(string newPath) {
        var index = GameComponentsLookup.Resource;
        var component = CreateComponent<Services.Game.Components.ResourceComponent>(index);
        component.path = newPath;
        ReplaceComponent(index, component);
    }

    public void RemoveResource() {
        RemoveComponent(GameComponentsLookup.Resource);
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

    static Entitas.IMatcher<GameEntity> _matcherResource;

    public static Entitas.IMatcher<GameEntity> Resource {
        get {
            if (_matcherResource == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Resource);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherResource = matcher;
            }

            return _matcherResource;
        }
    }
}