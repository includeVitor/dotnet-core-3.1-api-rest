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
    public class FeedbackController : MainController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IMapper _mapper;


        public FeedbackController(INotify notify,
                                  IFeedbackService feedbackService, 
                                  IMapper mapper,
                                  IUser user) : base(notify, user)
        {
            _feedbackService = feedbackService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<FeedbackViewModel>> All() => _mapper.Map<IEnumerable<FeedbackViewModel>>(await _feedbackService.All());

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FeedbackViewModel>> Show(Guid id)
        {
            var feedback = await GetFeedback(id);

            if (feedback == null) return NotFound();

            return FormattedResponse(feedback);
        }

        [HttpPost]
        public async Task<ActionResult<FeedbackViewModel>> Add(FeedbackViewModel feedbackViewModel)
        {
            if (!ModelState.IsValid) return FormattedResponse(ModelState);

            await _feedbackService.Add(_mapper.Map<Feedback>(feedbackViewModel));

            return FormattedResponse(feedbackViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FeedbackViewModel>> Update(Guid id, FeedbackViewModel feedbackViewModel)
        {
            if (id != feedbackViewModel.Id)
            {
                NotifyError("Id incorreto");
                return FormattedResponse(feedbackViewModel);
            }

            if (!ModelState.IsValid) return FormattedResponse(feedbackViewModel);
            
            await _feedbackService.Update(_mapper.Map<Feedback>(feedbackViewModel));

            return FormattedResponse(feedbackViewModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FeedbackViewModel>> Remove(Guid id)
        {
            var feedback = await GetFeedback(id);

            if (feedback == null) return FormattedResponse(feedback);

            await _feedbackService.Remove(id);

            return FormattedResponse(feedback);
        }


        private async Task<FeedbackViewModel> GetFeedback(Guid id) => _mapper.Map<FeedbackViewModel>(await _feedbackService.Show(id));


    }
}
