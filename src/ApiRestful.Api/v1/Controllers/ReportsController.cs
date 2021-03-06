﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiRestful.Api.ViewModels;
using ApiRestful.Business.Interfaces;
using ApiRestful.Business.Interfaces.Services;
using ApiRestful.Business.Models;

namespace ApiRestful.Api.v1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ReportsController : MainController
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        public ReportsController(INotify notify, 
                                IReportService reportService, 
                                IMapper mapper,
                                IUser user) : base(notify, user)
        {
            _reportService = reportService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ReportViewModel>> All() => _mapper.Map<IEnumerable<ReportViewModel>>(await _reportService.All());

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ReportViewModel>> Show(Guid id)
        {
            var report = await GetReport(id);

            if (report == null) return NotFound();

            return FormattedResponse(report);
        }

        [HttpPost]
        public async Task<ActionResult<ReportViewModel>> Add(ReportViewModel reportViewModel)
        {
            if (!ModelState.IsValid) return FormattedResponse(ModelState);

            await _reportService.Add(_mapper.Map<Report>(reportViewModel));

            return FormattedResponse(reportViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ReportViewModel>> Update(Guid id, ReportViewModel reportViewModel)
        {
            if (id != reportViewModel.Id)
            {
                NotifyError("Id incorreto");
                return FormattedResponse(reportViewModel);
            }

            if (!ModelState.IsValid) return FormattedResponse(ModelState);

            await _reportService.Update(_mapper.Map<Report>(reportViewModel));

            return FormattedResponse(reportViewModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ReportViewModel>> Remove(Guid id)
        {
            var report = await GetReport(id);

            if (report == null) return NotFound();

            await _reportService.Remove(id);

            return FormattedResponse(report);
        }


        private async Task<ReportViewModel> GetReport(Guid id) => _mapper.Map<ReportViewModel>(await _reportService.Show(id));
    }
}
