using System;
using NServiceBus.Saga;

namespace NServiceBus.Sagas.Impl
{
    /// <summary>
    /// Class used to bridge the dependency between Saga{T} in NServiceBus.dll 
    /// </summary>
    public class ReplyingToNullOriginatorDispatcher : IHandleReplyingToNullOriginator
    {
        void IHandleReplyingToNullOriginator.TriedToReplyToNullOriginator()
        {
            if (Configure.Logger.IsDebugEnabled)
                throw new InvalidOperationException
                    (
                    "Originator of saga has not provided a return address - cannot reply.");
        }
    }
}