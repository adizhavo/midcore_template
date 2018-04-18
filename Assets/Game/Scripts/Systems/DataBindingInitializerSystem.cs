using Entitas;
using Zenject;
using Services.Core.Databinding;
using UnityEngine;

namespace MidcoreTemplate.Game.Systems
{
    /// <summary>
    /// Will setup all binding data with default values
    /// </summary>
    
    public class DataBindingInitializerSystem : IInitializeSystem
    {
        [Inject] DataBindingService dataBinding;
        
        public void Initialize()
        {
            dataBinding
            // Add more bindings here
            .AddData<bool>("game.camera.camp", true)
            .AddData<Camera>(Constants.DATABINDING_CAMERA_ACTIVE);
        }
    }
}