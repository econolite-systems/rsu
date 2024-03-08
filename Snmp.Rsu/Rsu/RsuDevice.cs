// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Net.Sockets;
using Econolite.Ode.Models.Rsu;
using Econolite.Ode.Models.Rsu.Status;
using Econolite.Ode.Snmp.Rsu.Commands;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Snmp.Rsu;

public class RsuDevice : Device
{
    private readonly ILogger<RsuDevice> _logger;
    private const string TimPsid = "8003";
    
    public SnmpDevice Device => _device;

    public RsuDevice(SnmpDevice device, ISnmpServer server, IPollingService pollingService, IPollProducer<IPollResponse> producer, ILogger<RsuDevice> logger)
        : base(device, server, pollingService, producer)
    {
        _logger = logger;
    }

    public async Task<ICommandResponse> SetTimMessage(TimMessageSnmp snmp, int? index)
    {
        try
        {
            if (!index.HasValue)
            {
                var rows = await GetTimRows();

                var timRows = rows as int[] ?? rows.ToArray();

                _logger.LogInformation("Tim rows found: {Rows}" , timRows.Length);
                index = timRows.Any() ? timRows.Last() + 1 : 2;
            }
            if (_device.RequireStandbyModeOnSet)
            {
               var setResult = await SetDataAsync(_server, _device, new List<Variable>()
               {
                   new Variable(new ObjectIdentifier("1.0.15628.4.1.99.0"), new Integer32(2))
               }); 
            }
            var setResultMain = await SetDataAsync(_server, _device, new List<Variable>()
            {
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.2.{index}"), new OctetString(TimPsid.ToByteArray())),
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.3.{index}"), new Integer32(snmp.Msgid)),
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.4.{index}"), new Integer32(snmp.Mode)),
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.5.{index}"), new Integer32(snmp.Channel)),
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.6.{index}"), new Integer32(snmp.Interval)), 
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.7.{index}"), new OctetString(snmp.Deliverystart.ToByteArray())),
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.8.{index}"), new OctetString(snmp.Deliverystop.ToByteArray())),
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.9.{index}"), new OctetString(snmp.Payload.ToByteArray())),
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.10.{index}"), new Integer32(snmp.Enable))
            });
            if (_device.RequireStandbyModeOnSet)
            {
                var setResult2 = await SetDataAsync(_server, _device, new List<Variable>()
                {
                    new Variable(new ObjectIdentifier("1.0.15628.4.1.99.0"), new Integer32(4))
                });
            }

            var storeRepeatMessage = await GetRow(index.Value);

            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Message = storeRepeatMessage
            };
        }
        catch (ErrorException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (OperationException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (SnmpException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (SocketException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (OperationCanceledException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (Exception ex)
        {
            return new StoreRepeatMessageCommandResponse() {
                Device = _device,
                Error = ex.Message
            };
        }
    }

    private async Task<IEnumerable<int>> GetTimRows()
    {
        var pollResponse = await GetSrmTable();

        if (pollResponse.Error != string.Empty)
        {
            _logger.LogError("Error getting SRM table: {Error}", pollResponse.Error);
            return Array.Empty<int>();
        }
        
        var rows = pollResponse.Variables.ToSRMData().ToArray();
        _logger.LogInformation("SRM rows found: {Rows}" , rows.Length);
        return rows.Where(r => r.MsgId == J2735MsgId.TIM).Select(t => t.Id);
    }

    public async Task<StoreRepeatMessage?> GetRow(int index)
    {
        var result = await GetPollResponse(StoreRepeatMessageSnmp.CreateVariableList(new []{ index }));
        return result.Variables.ToSRMData().FirstOrDefault();
    }
    
    public async Task<ICommandResponse> DeleteRow(int index)
    {
        try
        {
            var setResult = await SetDataAsync(_server, _device, new List<Variable>()
            {
                new Variable(new ObjectIdentifier("1.0.15628.4.1.99.0"), new Integer32(2))
            });
            var setResultMain = await SetDataAsync(_server, _device, new List<Variable>()
            {
                new Variable(new ObjectIdentifier($"1.0.15628.4.1.4.1.11.{index}"), new Integer32(6)),
            });
            var setResult2 = await SetDataAsync(_server, _device, new List<Variable>()
            {
                new Variable(new ObjectIdentifier("1.0.15628.4.1.99.0"), new Integer32(4))
            });
            
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                
            };
        }
        catch (ErrorException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (OperationException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (SnmpException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (SocketException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (OperationCanceledException ex)
        {
            return new StoreRepeatMessageCommandResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (Exception ex)
        {
            return new StoreRepeatMessageCommandResponse() {
                Device = _device,
                Error = ex.Message
            };
        }
    }

    public override async Task Poll()
    {
        var statusResult = await GetRsuStatus();
        var status = statusResult.ToRsuSystemStats();
        var result = await GetSrmTable();
        await _producer.Produce(statusResult);
    }

    private async Task<IPollResponse> GetRsuStatus()
    {
        try
        {
            var result = await GetPollResponse(RsuSystemStatus.CreateVariableList());
            return result;
        }
        catch (ErrorException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (OperationException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (SnmpException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (SocketException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (OperationCanceledException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (Exception ex)
        {
            return new PollResponse() {
                Device = _device,
                Error = ex.Message
            };
        }
    }
    
    public async Task<IEnumerable<StoreRepeatMessage>> GetSrmData()
    {
        var result = await GetSrmTable();
        if (result.Error != string.Empty)
        {
            return Array.Empty<StoreRepeatMessage>();
        }

        return result.Variables.ToSRMData();
    }
    
    private async Task<IPollResponse> GetSrmTable()
    {
        try
        {
            var indexes = await GetSetRowIndexes();
            var rowIndexes = indexes as int[] ?? indexes.ToArray();
            if (!rowIndexes.Any())
            {
                await _producer.Produce(new PollResponse()
                {
                    Device = _device
                });
            }

            var result = await GetPollResponse(StoreRepeatMessageSnmp.CreateVariableList(rowIndexes.ToArray()));
            return result;
        }
        catch (ErrorException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (OperationException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (SnmpException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (SocketException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (OperationCanceledException ex)
        {
            return new PollResponse()
            {
                Device = _device,
                Error = ex.ToError()
            };
        }
        catch (Exception ex)
        {
            return new PollResponse() {
                Device = _device,
                Error = ex.Message
            };
        }
    }

    private async Task<IPollResponse> GetPollResponse(IList<Variable> vList)
    {
        var results = await GetDataAsync(_server, _device, vList);
        return new PollResponse() {
            Device = _device,
            Variables = results
        };
    }

    private async Task<IEnumerable<int>> GetSetRowIndexes()
    {
        var result = await GetDataAsync(_server, _device, StoreRepeatMessageSnmp.CreateRowIndexVariables());

        var rows = result.Where(v => v.Data.ToString() == "1").Select(s => int.Parse(s.Id.ToString().Substring(21))).ToArray();
        return rows;
    }
}

public static class RsuDeviceExtensions
{
    public static byte[] ToByteArray(this string hex) {
        if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        var arr = new byte[hex.Length >> 1];

        for (var i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }

        return arr;
    }

    private static int GetHexVal(char hex) {
        var val = (int)hex;
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }
}