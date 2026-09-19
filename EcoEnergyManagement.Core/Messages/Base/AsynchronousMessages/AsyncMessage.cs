using CommunityToolkit.Mvvm.Messaging.Messages;
using EcoEnergyManagement.Core.Abstractions;
using EcoEnergyManagement.Core.Miscellaneous;

namespace EcoEnergyManagement.Core.Messages.Base.AsynchronousMessages
{
    public class AsyncMessage(object sender) : AsyncRequestMessage<Unit>, IDefaultCheck
    {
        public object Sender { get; init; } = sender;

        public bool IsDefault() => Sender is null;
    }
}
