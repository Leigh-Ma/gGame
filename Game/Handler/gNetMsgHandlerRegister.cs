//Auto generated, do not modify unless you know clearly what you are doing.
using System;
using gNet;

public static partial class gNetMsgHandler
{
    public static void Register()
    {
        gMsgDispatch.AddNetMsgHandler (gNetMsgType.MT_LoginAck, Handle_LoginAck);
        gMsgDispatch.AddNetMsgHandler (gNetMsgType.MT_LogoutAck, Handle_LogoutAck);
    }
}
