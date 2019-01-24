//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Services.Game.Components.GameObjectComponent gameObject { get { return (Services.Game.Components.GameObjectComponent)GetComponent(GameComponentsLookup.GameObject); } }
    public bool hasGameObject { get { return HasComponent(GameComponentsLookup.GameObject); } }

    public void AddGameObject(string newObjectId, string newTypeId, int newUniqueId) {
        var index = GameComponentsLookup.GameObject;
        var component = CreateComponent<Services.Game.Components.GameObjectComponent>(index);
        component.objectId = newObjectId;
        component.typeId = newTypeId;
        component.uniqueId = newUniqueId;
        AddComponent(index, component);
    }

    public void ReplaceGameObject(string newObjectId, string newTypeId, int newUniqueId) {
        var index = GameComponentsLookup.GameObject;
        var component = CreateComponent<Services.Game.Components.GameObjectComponent>(index);
        component.objectId = newObjectId;
        component.typeId = newTypeId;
        component.uniqueId = newUniqueId;
        ReplaceComponent(index, component);
    }

    public void RemoveGameObject() {
        RemoveComponent(GameComponentsLookup.GameObject);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherGameObject;

    public static Entitas.IMatcher<GameEntity> GameObject {
        get {
            if (_matcherGameObject == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GameObject);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherGameObject = matcher;
            }

            return _matcherGameObject;
        }
    }
}
