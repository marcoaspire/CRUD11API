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
using Crud11API.Repository;
using Microsoft.AspNetCore.Authorization;
//using System.Web.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Crud11API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PaymentDetailContext _context;
        private readonly IJWTManagerRepository _jWTManager;

        public AuthController(PaymentDetailContext context, IJWTManagerRepository jWTManager)
        {
            _context = context;
            this._jWTManager = jWTManager;
        }

        [Authorize]
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
        public ActionResult Get()
        {
            //return Ok(_context.Users.ToList());
            return Ok(new { results = _context.Users.ToList(), msg= "ALl users" });

        }
        //revalidate token
        [HttpGet]
        [Route("renew")]
        public ActionResult RefreshToken()
        {
            Debug.WriteLine("Renew2");

            string token = (Request.Headers["x-token"]);
            if (token == null)
            {
                return Ok(new {  msg = "Did not receive a token, 401 Unautorized" });//401 unautorized

            }
            else
            {
                //validate jwt
                Tokens t = _jWTManager.VerifyToken(token);
                if (t.RefreshToken == true)
                {
                    var userLog = _context.Users.SingleOrDefault(user => user.UserID.ToString() == t.Token);
                    var newToken = _jWTManager.Authenticate(userLog);
                    return Ok(new { ok = true, id = userLog.UserID, name = userLog.Name, token = newToken.Token });
                }

            }
            return Ok(new { msg = "Error2" });


        }

        [HttpPost]
        //Authenticate
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

                    if (userLog == null)
                    {
                        //Debug.WriteLine(Crypto.HashPassword(u.Password));
                        return Ok(new { ok = false,  msg = "Email invalid" });
                    }
                    else
                    {
                        var validate = Crypto.VerifyHashedPassword(userLog.Password, u.Password);

                        Debug.WriteLine(validate);
                        Debug.WriteLine("Validation");

                        //var userLog = _context.Users.SingleOrDefault(user => user.Email == u.Email && user.Password == u.Password );
                        if (!validate)
                        {
                            return Ok(new { ok = false, msg = "Password invalid" });

                        }

                        var token = _jWTManager.Authenticate(userLog);

                        return Ok(new { ok=true,id = userLog.UserID, name= userLog.Name, token= token.Token });

                    }


                }
                catch (Exception)
                {
                    return Ok(new { ok = false, msg = "Invalid credentials" });

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
            try
            {
                var passwordHash = Crypto.HashPassword(user.Password);
                user.Password = passwordHash;
                var token = _jWTManager.Authenticate(user);
                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new { ok = true, id = user.UserID, name = user.Name, token = token.Token });

            }
            catch (Exception)
            {

                throw;
            }

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
