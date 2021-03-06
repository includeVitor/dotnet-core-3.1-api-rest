﻿using ApiRestful.Business.Interfaces;
using ApiRestful.Business.Interfaces.Services;
using ApiRestful.Business.Models;
using ApiRestful.Business.Models.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRestful.Business.Services
{
    public class FieldService : BaseService, IFieldService
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IReportRepository _reportRepository;


        public FieldService(INotify notify,
                            IFieldRepository fieldRepository,
                            IFeedbackRepository feedbackRepository, 
                            IReportRepository reportRepository) : base(notify)
        {
            _fieldRepository = fieldRepository;
            _feedbackRepository = feedbackRepository;
            _reportRepository = reportRepository;
        }

        public async Task<IEnumerable<Field>> All() => await _fieldRepository.All();

        public async Task<Field> Show(Guid id) => await _fieldRepository.GetAllInField(id);

        public async Task<bool> Add(Field field)
        {
            if (!ExecuteValidation(new FieldValidation(), field)) return false;

            if (_fieldRepository.Find(f => f.Name == field.Name).Result.Any())
            {
                Notify("Já existe outra área com esse nome");
                return false;
            }

            await _fieldRepository.Add(field);
            return true;
        }

        public async Task<bool> Update(Field field)
        {
            if (!ExecuteValidation(new FieldValidation(), field)) return false;

            if (_fieldRepository.Find(f => f.Name == field.Name && f.Id != field.Id).Result.Any())
            {
                Notify("Já existe outra área com esse nome");
                return false;
            }

            await _fieldRepository.Update(field);
            return true;
        }

        public async Task<bool> Remove(Guid id){

            var feedbacks = await  _feedbackRepository.GetFeedbacksByField(id);

            if(feedbacks != null)
            {
                foreach (var feedback in feedbacks)
                {
                    await _feedbackRepository.Remove(feedback.Id);
                }
            }

            var reports = await _reportRepository.GetReportsByField(id);

            if (reports != null)
            {
                foreach (var report in reports)
                {
                    await _reportRepository.Remove(report.Id);
                }
            }

            await _fieldRepository.Remove(id);  
            return true; 
        }

        public void Dispose()
        {
            _fieldRepository?.Dispose();
        }
    }
}
