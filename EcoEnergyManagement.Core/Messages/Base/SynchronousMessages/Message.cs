using EcoEnergyManagement.Core.Abstractions;

namespace EcoEnergyManagement.Core.Messages.Base.SynchronousMessages
{
    public class Message(object sender) : IDefaultCheck
    {
        public object Sender { get; init; } = sender;

        public virtual bool IsDefault() => Sender is null;
    }
}
