﻿using ApiRestful.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiRestful.Business.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> GetFieldsInProject(Guid id);
        Task<Project> GetReportModelsInProject(Guid id);
        Task<Project> GetAllInProject(Guid id);
        Task<IEnumerable<Project>> GetAllProjects();

    }
}
