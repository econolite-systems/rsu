// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Models.Rsu.Status;

public class StoreRepeatMessage
{
    /// <summary>
    /// Store and Repeat Message Index.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Store and Repeat Message PSID.
    /// </summary>
    public string Psid { get; set; }
    
    /// <summary>
    /// DSRC Message ID as defined in J2735.
    /// For the J2735 version 2016-03, some sample values are:
    /// MAP = 18, SPAT = 19, TIM = 31
    /// </summary>
    public J2735MsgId MsgId { get; set; }
    
    /// <summary>
    /// DSRC mode set for Store and Repeat Message transmit, Continuous (0) or Alternating (1)
    /// </summary>
    public TransmitMode TxMode { get; set; }
    
    /// <summary>
    /// DSRC channel set for Store and Repeat Message transmit. (172..184)
    /// </summary>
    public int TxChannel { get; set; }
    
    /// <summary>
    /// Time interval in milliseconds between two successive Store and Repeat Messages
    /// </summary>
    public TimeSpan TxInterval { get; set; }
    
    /// <summary>
    /// Store and Repeat Message delivery start time.
    /// This time is supplied in the format of the first
    /// 6 octets in the DateAndTime field as defined in RFC2579.
    /// Example: October 7, 2017 at 11:34 PM UTC would be encoded
    /// as 07e10a071722
    /// </summary>
    public DateTime DeliveryStart { get; set; }
    
    /// <summary>
    /// Store and Repeat Message delivery stop time.
    /// </summary>
    public DateTime DeliveryStop { get; set; }
    
    /// <summary>
    /// Payload of Store and Repeat message. 
    /// For SAE J2735-032016 messages, this object includes encoded MessageFrame.
    /// Length limit derived from dot3MIB.
    /// </summary>
    public string Payload { get; set; }
    
    /// <summary>
    /// Set this bit to enable transmission of the message
    /// 0=off, 1=on
    /// </summary>
    public bool Enable { get; set; }
}