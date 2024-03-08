// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

namespace Econolite.Ode.Snmp.Rsu
{
    public interface IPollProducer<T>
    {
        Task Produce(T item);
    }
}