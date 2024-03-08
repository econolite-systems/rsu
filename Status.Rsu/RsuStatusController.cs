// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using Microsoft.AspNetCore.Mvc;
using Econolite.Ode.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Econolite.Ode.Domain.Rsu;
using Econolite.Ode.Models.Rsu.Status;

namespace Econolite.Ode.Service.Rsu.Status
{

    [ApiController]
    [Route("rsu-status")]
    [AuthorizeOde(MoundRoadRole.ReadOnly)]
    public class RsuStatusController : ControllerBase
    {
        private readonly IRsuStatusService _rsuStatusService;

        /// <summary>
        /// Constructs a Road Side Unit Status controller
        /// </summary>
        public RsuStatusController(IRsuStatusService rsuStatusService)
        {
            _rsuStatusService = rsuStatusService;
        }

        /// <summary>
        /// Finds devices with rsu statuses matching the given query parameters
        /// </summary>
        /// <remarks>
        /// The start date is mandatory, but the end date is optional. If no end date is given, all status entries with a
        /// timestamp from the start date up to the latest will be returned. If an end date is provided, only status entries
        /// within the date range will be returned.
        ///
        /// If no device ID parameters are given, then the query will *not* filter on any device IDs, so statuses for
        /// any device will be returned. The device ID parameter may be provided multiple times to filter on multiple
        /// device IDs. If any device IDs are given, only statuses for the given device IDs will be returned.
        /// </remarks>
        /// <param name="deviceIds">Optional device IDs to filter on</param>
        /// <param name="startDate">Required start date</param>
        /// <param name="endDate">Optional end date</param>
        /// <response code="200">Returns a list of road side unit status entries matching the given query parameters</response>
        [HttpGet("find")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RsuStatusDto>))]
        public async Task<IActionResult> Find([FromQuery] List<Guid> deviceIds, [BindRequired] DateTime startDate, DateTime? endDate)
        {
            return Ok(await _rsuStatusService.Find(deviceIds, startDate, endDate));
        }
    }
}
