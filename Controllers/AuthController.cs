using Crud11API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.Helpers;
using Crud11API.ViewModels;
//using System.Web.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Crud11API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PaymentDetailContext _context;

        public AuthController(PaymentDetailContext context)
        {
            _context = context;
        }
       
        [HttpGet]
        /*public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await _context.Users.ToListAsync();
        }
        */
        /*
        public ActionResult<IEnumerable<User>> Get()
        {
            return _context.Users.ToList();
        }
        */
        //revalidate token
        public ActionResult Get()
        {
            //return Ok(_context.Users.ToList());
            return Ok(new { results = _context.Users.ToList(), msg= "Renew" });

        }

        [HttpPost]
        public ActionResult PostLogin(LoginViewModel u)
        {
            Debug.WriteLine("QWWWWWWWWWWWWWWWW");
            Trace.WriteLine(Request.Body);
            Trace.WriteLine(u.Email != null);
            Trace.WriteLine(u.Password != null);
            if (ModelState.IsValid)
            {
                try
                {
                    var passwordHash = Crypto.HashPassword(u.Password);
                    //Debug.WriteLine(passwordHash);

                    //Debug.WriteLine(Crypto.HashPassword(u.Password));

                    
                    var userLog = _context.Users.SingleOrDefault(user => user.Email == u.Email );

                    Debug.WriteLine(userLog);


                    var validate = Crypto.VerifyHashedPassword(userLog.Password, u.Password);

                    Debug.WriteLine(validate);
                    Debug.WriteLine("Validation");

                    //var userLog = _context.Users.SingleOrDefault(user => user.Email == u.Email && user.Password == u.Password );
                    if (!validate)
                    {
                        return NotFound();
                    }

                    if (userLog == null)
                    {
                        //Debug.WriteLine(Crypto.HashPassword(u.Password));

                        return NotFound();
                    }
                    else
                    {
                        return Ok(new { user = userLog });

                    }


                }
                catch (Exception)
                {
                    return ValidationProblem();
                }

            }


            /*

            Trace.WriteLine(algo.Email);
            Trace.WriteLine(algo.Name);
            Trace.WriteLine(algo.Password);
            */




            //using (var reader = new StreamReader(Request.Body))
            //{
            //    var body = reader.ReadToEnd();
            //    Trace.WriteLine(body);

            //    // Do something
            //}
            //var body = new StreamReader(Request.Body);
            //The modelbinder has already read the stream and need to reset the stream index
            //var requestBody = body.ReadToEnd();

            //Trace.WriteLine(body);



            return Ok(new { msg = "Error" });
        }


        //new user
        // POST api/<AuthController>/new
        [Route("new")]

        [HttpPost]
        public ActionResult Post(User user)
        {
            var passwordHash = Crypto.HashPassword(user.Password);
            user.Password = passwordHash;
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(new { user = user , msg = "New user" });
        }

        /*
        // GET: api/<AuthController>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AuthController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AuthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}
