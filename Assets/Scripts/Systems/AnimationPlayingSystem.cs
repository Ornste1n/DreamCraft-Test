using UnityEngine;
using Unity.Entities;

namespace Systems
{
    public partial struct AnimationPlayingSystem : ISystem
    {
        private static int _globalTimeProperty; // За счет GlobalTime шейдер анимации двигает текстуру

        public void OnCreate(ref SystemState state)
        {
            _globalTimeProperty = Shader.PropertyToID("_GlobalTime");
        }

        public void OnUpdate(ref SystemState state)
        {
            Shader.SetGlobalFloat(_globalTimeProperty, (float)SystemAPI.Time.ElapsedTime);
        }
    }
}