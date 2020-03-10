using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    public interface IProcessBus : IEventPublisher, ICommandPublisher
    {
        bool HaveAnyHandlerRegisteredForEvent(string classNameEvent);

        bool IsHandlerRegistered<T>(string channelName) where T : IMessage;
        void RegisterHandler<T>(Action<T> handler, string handlerName) where T : IMessage;
        void UnRegisterHandler<T>(string handlerName) where T : IMessage;
        void UnRegisterHandler(Type t, string handlerName);


        void RelaunchEventsNotExecutedForRegisteredHandler(string handlerName,
            List<string> eventsTypeNameToRelaunch, bool raiseException);


        //For test only
        void RemoveJobCoordinator();
    }
}