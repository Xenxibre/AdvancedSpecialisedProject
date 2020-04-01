using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    interface IMayRequireTangent
    {
        NeededCoordinateSpace RequiresTangent(ShaderStageCapability stageCapability = ShaderStageCapability.All);
    }

    static class MayRequireTangentExtensions
    {
        public static NeededCoordinateSpace RequiresTangent(this ISlot slot)
        {
            var mayRequireTangent = slot as IMayRequireTangent;
            return mayRequireTangent != null ? mayRequireTangent.RequiresTangent() : NeededCoordinateSpace.None;
        }
    }
}
