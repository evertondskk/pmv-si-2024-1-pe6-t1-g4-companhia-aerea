using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.Domain.Dtos.CustomerAggregate;
using Api.Domain.Interfaces.Services.CustomerAggregate;
using Api.Domain.Interfaces.Services.UserAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize("Bearer")]
    public class CustomersController : ControllerBase
    {

        public ICustomerService _service { get; set; }
        public ILoginService _loginService { get; set; }
        public CustomersController(ICustomerService service, ILoginService loginService)
        {
            _service = service;
            _loginService = loginService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCustomers()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request - Solicitação Inválida
            }
            try
            {
                return Ok(await _service.GetAll());
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetCustomer")]
        public async Task<ActionResult> GetCustomer(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _service.Get(id);
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CustomerDtoCreate customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadToken(jwtToken) as JwtSecurityToken;
                var email = token.Claims.FirstOrDefault(i => i.Type == "name").Value.ToString();

                var baseUser = await _loginService.FindByLogin(email);
                if (baseUser != null)
                {
                    var result = await _service.Post(customer, baseUser.Id);
                    if (result != null)
                    {
                        return Created(new Uri(Url.Link("GetCustomer", new { id = result.Id })), result);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                return BadRequest();

            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] CustomerDtoUpdate customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _service.Put(customer, Guid.Empty);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        //[Authorize("Bearer")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                return Ok(await _service.Delete(id));
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }



    }
}
