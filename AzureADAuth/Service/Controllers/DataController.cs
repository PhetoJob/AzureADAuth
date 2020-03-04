using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "ReaderRole")]
        public string Get()
        {
            return "Some random data coming from the service: " + Guid.NewGuid();
        }
    }
}