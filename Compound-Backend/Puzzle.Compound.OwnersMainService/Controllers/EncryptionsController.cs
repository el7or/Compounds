using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Puzzle.Compound.Security;

namespace Puzzle.Compound.OwnersMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public EncryptionsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpGet("{text}")]
        public ActionResult Get(string text)
        {

            var encryptionKey = configuration.GetSection("Security:EncryptionKey").Value;
            string encText = Encryption.EncryptData(text, encryptionKey);
            //Console.WriteLine(encText);
            return Ok(new
            {
                Encrypted = encText
            });
        }
    }
}
