// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Net.Sockets;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Econolite.Ode.Snmp.Rsu;

public static class SnmpError
{
    public static string ToError(this SnmpException ex)
    {
        return ex.Message;
    }
    
    public static string ToError(this SocketException ex)
    {
        return ex.Message;
    }
    
    public static string ToError(this OperationException ex)
    {
        return ex.Message;
    }
    
    public static string ToError(this OperationCanceledException ex)
    {
        return "Timeout: Command timed out.";
    }
    
    public static string ToError(this ErrorException ex)
    {
        return ex.Message;
    }
}