using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using MAction.SampleOnion.Service.Company;
using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAction.SampleOnion.Service.Category;

namespace MAction.SampleOnion.Infrastructure
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleCompanyController : ControllerBase
    {
        private readonly ICompanyServiceWithExpression _companyServiceWithExpression;
        private readonly ICategoryService _categoryService;

        public SaleCompanyController(ICompanyServiceWithExpression companyServiceWithExpression,
            ICategoryService categoryService)
        {
            _companyServiceWithExpression = companyServiceWithExpression;
            _categoryService = categoryService;
        }

        [HttpPost("GetAllCompany")]
        [AllowAnonymous]
        public async Task<ActionResult<DynamicQueryFilterResult<SaleCompanyOutputModel>>> GetAllCompany(
            [FromBody] FilterAndSortConditions inputs)
        {
            return await _companyServiceWithExpression.GetItemByFilterAsync(inputs);
        }

        [HttpPost]
        public async Task<ActionResult<SaleCompanyOutputModel>> GetAllCompany([FromBody] SaleCompanyInputModel inputs)
        {
            return await _companyServiceWithExpression.InsertAsync(inputs);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCompany([FromBody] SaleCompanyInputModel inputs)
        {
            await _companyServiceWithExpression.UpdateAsync(inputs);

            return Ok();
        }

        [HttpPost("GetAllCategory")]
        [AllowAnonymous]
        public async Task<ActionResult<DynamicQueryFilterResult<CategoryOutputModel>>> GetAllCategory(
            [FromBody] FilterAndSortConditions inputs)
        {
            return await _categoryService.GetItemByFilterAsync(inputs);
        }

        [HttpPost("CreateCategory")]
        public async Task<ActionResult<CategoryOutputModel>> InsertCategory([FromBody] CategoryInputModel inputs)
        {
            return await _categoryService.InsertAsync(inputs);
        }
        
        [HttpPut("UpdateCategory")]
        public async Task<ActionResult> UpdateCategory([FromBody] CategoryInputModel inputs)
        {
            await _categoryService.UpdateAsync(inputs);

            return Ok();
        }
    }
}