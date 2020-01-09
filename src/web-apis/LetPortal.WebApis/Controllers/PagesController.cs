﻿using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Repositories.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.WebApis.Controllers
{
    [Route("api/pages")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IPageRepository _pageRepository;

        private readonly IServiceLogger<PagesController> _logger;

        public PagesController(
            IPageRepository pageRepository,
            IDatabaseServiceProvider databaseServiceProvider,
            IServiceLogger<PagesController> logger)
        {
            _pageRepository = pageRepository;
            _databaseServiceProvider = databaseServiceProvider;
            _logger = logger;
        }

        [HttpGet("shorts")]
        [ProducesResponseType(typeof(List<ShortPageModel>), 200)]
        public async Task<IActionResult> GetAllShortPages()
        {
            var result = await _pageRepository.GetAllShortPagesAsync();
            _logger.Info("All short pages: {@result}", result);
            return Ok(result);
        }

        [HttpGet("all-claims")]
        [ProducesResponseType(typeof(List<ShortPortalClaimModel>), 200)]
        public async Task<IActionResult> GetAllPortalClaims()
        {
            var result = await _pageRepository.GetShortPortalClaimModelsAsync();
            _logger.Info("All portal claims: {@result}", result);
            return Ok(result);
        }

        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(Page), 200)]
        public async Task<IActionResult> GetOneById(string id)
        {
            var result = await _pageRepository.GetOneAsync(id);
            _logger.Info("Found page: {@result}", result);
            return Ok(result);
        }

        [HttpGet("{name}")]
        [ProducesResponseType(typeof(Page), 200)]
        public async Task<IActionResult> GetOne(string name)
        {
            var result = await _pageRepository.GetOneByNameAsync(name);
            _logger.Info("Found page: {@result}", result);
            return Ok(result);
        }

        [HttpGet("render/{name}")]
        [ProducesResponseType(typeof(Page), 200)]
        public async Task<IActionResult> GetOneForRender(string name)
        {
            var result = await _pageRepository.GetOneByNameForRenderAsync(name);
            _logger.Info("Found page: {@result}", result);
            return Ok(result);
        }

        [HttpGet("short-pages")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        public async Task<IActionResult> GetShortPages([FromQuery] string keyWord = null)
        {
            return Ok(await _pageRepository.GetShortPages(keyWord));
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Create([FromBody] Page page)
        {
            if(ModelState.IsValid)
            {
                page.Id = DataUtil.GenerateUniqueId();
                await _pageRepository.AddAsync(page);
                _logger.Info("Created page: {@page}", page);
                return Ok(page.Id);
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Page page)
        {
            if(ModelState.IsValid)
            {
                page.Id = id;
                await _pageRepository.UpdateAsync(id, page);
                _logger.Info("Updated page: {@page}", page);
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _pageRepository.DeleteAsync(id);
            _logger.Info("Deleted page id: {id}", id);
            return Ok();
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _pageRepository.IsExistAsync(a => a.Name == name));
        }

        [HttpPost("{pageId}/submit")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        public async Task<IActionResult> SubmitCommand(string pageId, [FromBody] PageSubmittedButtonModel pageSubmittedButtonModel)
        {
            var page = await _pageRepository.GetOneAsync(pageId);
            if(page != null)
            {
                var button = page.Commands.First(a => a.Name == pageSubmittedButtonModel.ButtonName);
                if(button.ButtonOptions.ActionCommandOptions.ActionType == Portal.Entities.Shared.ActionType.ExecuteDatabase)
                { 
                    var result = 
                        await _databaseServiceProvider
                                .ExecuteDatabase(
                                    button.ButtonOptions.ActionCommandOptions.DatabaseOptions.DatabaseConnectionId, 
                                    button.ButtonOptions.ActionCommandOptions.DatabaseOptions.Query, 
                                    pageSubmittedButtonModel
                                        .Parameters
                                        .Select(a => new ExecuteParamModel { Name = a.Name, RemoveQuotes = a.RemoveQuotes, ReplaceValue = a.ReplaceValue}));

                    return Ok(result);
                }
            }

            return NotFound();
        }

        [HttpPost("{pageId}/fetch-datasource")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        public async Task<IActionResult> GetDatasourceForPage(string pageId, [FromBody] PageRequestDatasourceModel pageRequestDatasourceModel)
        {
            var page = await _pageRepository.GetOneAsync(pageId);
            if(page != null)
            {
                var datasource = page.PageDatasources.First(a => a.Id == pageRequestDatasourceModel.DatasourceId);
                if(datasource.Options.Type == Portal.Entities.Shared.DatasourceControlType.Database)
                {
                    var result = await _databaseServiceProvider.ExecuteDatabase(datasource.Options.DatabaseOptions.DatabaseConnectionId, datasource.Options.DatabaseOptions.Query, pageRequestDatasourceModel.Parameters.Select(a => new ExecuteParamModel { Name = a.Name, RemoveQuotes = a.RemoveQuotes, ReplaceValue = a.ReplaceValue }));

                    return Ok(result);
                }                 
            }

            return NotFound();
        }
    }
}
