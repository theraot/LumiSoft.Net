﻿namespace LumiSoft.Net.STUN.Message
{
    internal enum AttributeType
    {
        MappedAddress = 0x0001,
        ResponseAddress = 0x0002,
        ChangeRequest = 0x0003,
        SourceAddress = 0x0004,
        ChangedAddress = 0x0005,
        Username = 0x0006,
        Password = 0x0007,
        MessageIntegrity = 0x0008,
        ErrorCode = 0x0009,
        UnknownAttribute = 0x000A,
        ReflectedFrom = 0x000B,
        XorOnly = 0x0021,
        XorMappedAddress = 0x8020,
        ServerName = 0x8022
    }
}