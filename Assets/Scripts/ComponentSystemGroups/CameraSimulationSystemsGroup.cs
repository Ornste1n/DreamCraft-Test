using Unity.Entities;

namespace ComponentSystemGroups
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class CameraInitializationSystemsGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class CameraSimulationSystemsGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CameraPresentationSystemsGroup : ComponentSystemGroup { }
}