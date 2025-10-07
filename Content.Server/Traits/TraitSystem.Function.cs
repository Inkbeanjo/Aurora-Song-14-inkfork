using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager;
using JetBrains.Annotations;
using Content.Shared.Prototypes;

namespace Content.Server.Traits;

/// Used for traits that add a Component upon spawning in, overwriting the pre-existing component if it already exists.
[UsedImplicitly]
public sealed partial class TraitReplaceComponent : TraitFunction
{
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry Components { get; private set; } = new();

    public override void OnPlayerSpawn(EntityUid uid,
        IComponentFactory factory,
        IEntityManager entityManager,
        ISerializationManager serializationManager)
    {
        foreach (var (_, data) in Components)
        {
            var comp = (Component)serializationManager.CreateCopy(data.Component, notNullableOverride: true);
            comp.Owner = uid;
            entityManager.AddComponent(uid, comp, true);
            if (!comp.GetType().HasCustomAttribute<NetworkedComponentAttribute>())
                continue;

            entityManager.Dirty(uid, comp);
        }
    }
}
/// This serves as a hook for trait functions to modify a player character upon spawning in.
[ImplicitDataDefinitionForInheritors]
public abstract partial class TraitFunction
{
    public abstract void OnPlayerSpawn(
        EntityUid mob,
        IComponentFactory factory,
        IEntityManager entityManager,
        ISerializationManager serializationManager);
}