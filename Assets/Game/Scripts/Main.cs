using Zenject;
using Entitas;
using UnityEngine;
using Services.Core;
using Services.Core.Data;
using Services.Core.DataVersion;
using Services.Core.Event;
using Services.Core.Gesture;
using Services.Core.GUI;
using Services.Game;

public class Main : MonoBehaviour
{
    private Systems gameSystems;
    private DiContainer container;

    private void Awake()
    {
        // initialise the di container and installers
        container = new DiContainer();
        CoreServicesInstaller.Install(container);
        GameServiceInstaller.Install(container);

        // add core services
        gameSystems = new Systems()
            .Add(container.Resolve<DatabaseService>())
            .Add(container.Resolve<DataVersionService>())
            .Add(container.Resolve<GestureService>())
            .Add(container.Resolve<AssetManifestReader>())
            .Add(container.Resolve<GUIService>());

        gameSystems.Initialize();
    }

    private void Update()
    {
        gameSystems.Execute();
    }
}