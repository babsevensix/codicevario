using System;

namespace ConsoleApp2
{
    /// <summary>
    /// An event represents something that took place in the domain. 
    /// They are always named with a past-participle verb, such as OrderConfirmed. 
    /// It's not unusual, but not required, for an event to name an aggregate
    ///  or entity that it relates to; let the domain language be your guide.
    /// Since an event represents something in the past, it can be considered 
    /// a statement of fact and used to take decisions in other parts of the system.
    /// </summary>
    public interface IEvent : IMessage
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        int UidEvent { get; set; }

        /// <summary>
        /// Generation Event's Timestamp.
        /// </summary>
        DateTime TimeStamp { get; set; }

        /// <summary>
        /// Command indentifier of command that raise event (optional).
        /// </summary>
        int? FromUidCommand { get; set; }
    }
}