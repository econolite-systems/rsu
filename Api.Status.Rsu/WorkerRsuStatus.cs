// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Models.Rsu.Status;
using Econolite.Ode.Repository.Rsu;
using Econolite.Ode.Status.Rsu;
using IRsuStatusConsumer = Econolite.Ode.Status.Rsu.Messaging.IRsuStatusConsumer;

namespace Api.Status.Rsu;

public class WorkerRsuStatus : BackgroundService
{
    private readonly IRsuStatusConsumer _rsuStatusConsumer;
    private readonly ILogger<WorkerRsuStatus> _logger;
    private readonly IRsuStatusRepository _rsuStatusRepository;

    public WorkerRsuStatus(IRsuStatusConsumer rsuStatusConsumer, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        var serviceScope = serviceProvider.CreateScope();
        _rsuStatusConsumer = rsuStatusConsumer;
        _logger = loggerFactory.CreateLogger<WorkerRsuStatus>();
        _rsuStatusRepository = serviceScope.ServiceProvider.GetRequiredService<IRsuStatusRepository>();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        
        await Task.Run(async () =>
        {
            try
            {
                _logger.LogInformation("Starting the main loop");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        try
                        {
                            var (result, status) = _rsuStatusConsumer.Consume(stoppingToken);
                            _rsuStatusRepository.Add(status.ToRsuStatus(result.Key));
                            var (success,errors) = await _rsuStatusRepository.DbContext.SaveChangesAsync();
                            if (success)
                            {
                                _rsuStatusConsumer.Complete(result);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unhandled exception while processing: {@MessageType}", nameof(RsuSystemStats));
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Exception thrown while trying to consume rsu status messages");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("WorkerRsuStatus stopping");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Exception thrown while processing rsu status messages");
            }
        }, stoppingToken);
    }
}