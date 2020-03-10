using System;

namespace ConsoleApp2
{
    [Flags]
    public enum EnumChannels
    {
        NotDefined = 0,
        Desktop = 1, 
        MSite = 2,
        Android = 4,
        Ios = 8,
        Agenzia = 16,
        All = Desktop | MSite | Android | Ios,
        Mobile = Android | Ios
    }
}