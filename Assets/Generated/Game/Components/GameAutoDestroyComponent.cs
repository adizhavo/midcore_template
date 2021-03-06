//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Services.Game.Components.AutoDestroyComponent autoDestroy { get { return (Services.Game.Components.AutoDestroyComponent)GetComponent(GameComponentsLookup.AutoDestroy); } }
    public bool hasAutoDestroy { get { return HasComponent(GameComponentsLookup.AutoDestroy); } }

    public void AddAutoDestroy(float newRemainingTime, bool newIgnoreTimescale) {
        var index = GameComponentsLookup.AutoDestroy;
        var component = CreateComponent<Services.Game.Components.AutoDestroyComponent>(index);
        component.remainingTime = newRemainingTime;
        component.ignoreTimescale = newIgnoreTimescale;
        AddComponent(index, component);
    }

    public void ReplaceAutoDestroy(float newRemainingTime, bool newIgnoreTimescale) {
        var index = GameComponentsLookup.AutoDestroy;
        var component = CreateComponent<Services.Game.Components.AutoDestroyComponent>(index);
        component.remainingTime = newRemainingTime;
        component.ignoreTimescale = newIgnoreTimescale;
        ReplaceComponent(index, component);
    }

    public void RemoveAutoDestroy() {
        RemoveComponent(GameComponentsLookup.AutoDestroy);
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

    static Entitas.IMatcher<GameEntity> _matcherAutoDestroy;

    public static Entitas.IMatcher<GameEntity> AutoDestroy {
        get {
            if (_matcherAutoDestroy == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.AutoDestroy);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAutoDestroy = matcher;
            }

            return _matcherAutoDestroy;
        }
    }
}
