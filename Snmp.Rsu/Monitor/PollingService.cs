// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Econolite.Ode.Common.Scheduler.Base.Timers;
using Econolite.Ode.Common.Scheduler.Base.Timers.Impl;

namespace Econolite.Ode.Snmp.Rsu;

public interface IPollingService
{
    void RegisterPoll(IPoll poll);
    void UnregisterPoll(IPoll poll);
    void Clear();
}

public class PollingService : IPollingService
{
    private IList<IPoll> _pollList = new List<IPoll>();

    public PollingService(IPeriodicTimerFactory timerFactory)
    {
        var timer = timerFactory.CreateTopOfMinuteTimer();
        timer.Start(Run);
    }
    
    public void RegisterPoll(IPoll poll)
    {
        _pollList.Add(poll);
    }

    public void UnregisterPoll(IPoll poll)
    {
        _pollList = _pollList.Where(p => p.Id != poll.Id).ToList();
    }

    public void Clear()
    {
        _pollList.Clear();
    }

    private async Task Run()
    {
        await Task.WhenAll(_pollList.Select(p => p.Poll()));
    }
}