using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UserApis.Entities;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public List<UserEntity> users = new List<UserEntity>
        {
            new UserEntity
            {
                userId = 101,
                userName = "Satish"
            },
            new UserEntity
            {
                userId = 102,
                userName = "Gayatri"
            },
            new UserEntity
            {
                userId = 103,
                userName = "Sagar"
            },
            new UserEntity
            {
                userId = 104,
                userName = "Rutik"
            },
            new UserEntity
            {
                userId = 105,
                userName = "Tejashri"
            }

        };
        

        // GET: api/user

        //THis is method is to get all user in json, xml and text format

        [HttpGet("allUsers/{responseFormat}")]
        public ActionResult<IEnumerable<UserEntity>> Get(string responseFormat)
        {
            if(responseFormat == null)
            {
                return BadRequest(responseFormat);
            } else if(responseFormat == "text")
            {   StringBuilder sb = new StringBuilder();

                foreach (var userEntity in users)
                { 
                    sb.Append($" {userEntity.userId}, {userEntity.userName} ");
                    
                }

                return Content(sb.ToString(), "text/plain");

            } else if (responseFormat == "xml")
            {
                var serializer = new XmlSerializer(typeof(UserEntity));
                var xmlString = "";

                using (var stringWriter = new System.IO.StringWriter())
                {
                   foreach(var userEntity in users)
                   {
                        serializer.Serialize(stringWriter, userEntity);
                        xmlString = stringWriter.ToString();
                    }
                }

                return Content(xmlString, "application/xml");
            }
            return Ok(users);
        }


        [HttpGet("singleUser/{id}/{responseFormat}")]
        public ActionResult<UserEntity> Get(int id,string responseFormat)
        {
            var user = users.Find(u => u.userId == id);
            if (user == null)
            {
                return NotFound();
            } else if(responseFormat == "text")
            {
                var textResult = new ContentResult
                {
                    Content = $"UserId: {user.userId}, UserName: {user.userName}",
                    ContentType = "text/plain",
                    StatusCode = 200
                };

                return textResult;
            }
            else if(responseFormat == "xml")
            {
                UserEntity userEntity = new UserEntity();
                userEntity.userId = user.userId;
                userEntity.userName = user.userName;

                var xmlSerializer = new XmlSerializer(typeof(UserEntity));
                var xmlResult = new ContentResult
                {
                    Content = SerializeObjectToXml(userEntity, xmlSerializer),
                    ContentType = "application/xml",
                    StatusCode = 200
                };

                return xmlResult;
            }

            return Ok(user);
        }

        // GET: api/user/{id}
        //this method is to get single user in json,xml and text format

        [HttpGet("{id}")]
        public ActionResult<UserEntity> Get(int id)
        {
            var user = users.Find(u => u.userId == id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/user
        //This method is for json packet format to text and xml response

        [HttpPost("json/{responseFormat}")]
        public ActionResult<UserEntity> Post([FromBody] UserEntity newUser,string responseFormat)
        {
            var contentType = Request.ContentType?.ToLower();

            if(contentType == "application/json" && responseFormat == "text")
            {
                if (newUser == null)
                {
                    Console.WriteLine("Entity can't be null here");
                }
                else
                {
                    //newUser.userId = newUser.userId;
                    users.Add(newUser);

                    // Converting the response to "text/plain" format
                    var textResult = new ContentResult
                    {
                        Content = $"UserId: {newUser.userId}, UserName: {newUser.userName}",
                        ContentType = "text/plain",
                        StatusCode = 201
                    };

                    return textResult;
                }
            } else if(contentType == "application/json" && responseFormat == "xml")
            {
                //newUser.userId = Guid.NewGuid();
                users.Add(newUser);

                //converting the response to xml format
                var xmlSerializer = new XmlSerializer(typeof(UserEntity));
                var xmlResult = new ContentResult
                {
                    Content = SerializeObjectToXml(newUser, xmlSerializer),
                    ContentType = "application/xml",
                    StatusCode = 201
                };

                return xmlResult;
            }
            
            //newUser.userId = Guid.NewGuid();
            users.Add(newUser);
            return CreatedAtAction(nameof(Get), new { id = newUser.userId }, newUser);
        }

        //************************************************************************************************

        [HttpPost("text/{responseFormat}")]
        public ActionResult TextToJsonAndXml([FromBody] string textData, string responseFormat)
        {

            UserEntity userEntity = new UserEntity();

            if(textData == null)
            {
                return BadRequest(responseFormat);
            }

            Regex userIdRegex = new Regex(@"userId = (\d+)");
            Regex userNameRegex = new Regex(@"userName = (\w+)");

            
            Match userIdMatch = userIdRegex.Match(textData);
            Match userNameMatch = userNameRegex.Match(textData);

            if (userIdMatch.Success && userNameMatch.Success)
            {
                string userId = userIdMatch.Groups[1].Value;
                userEntity.userId = Convert.ToInt32(userId);
                string userName = userNameMatch.Groups[1].Value;
                userEntity.userName = userName;

            }

            if (responseFormat.ToLower() == "json")
            {
                return new JsonResult(userEntity);
            }
            else if (responseFormat.ToLower() == "xml")
            {
                var serializer = new XmlSerializer(typeof(UserEntity));
                var xmlString = "";

                using (var stringWriter = new System.IO.StringWriter())
                {
                    serializer.Serialize(stringWriter, userEntity);
                    xmlString = stringWriter.ToString();
                }

                return Content(xmlString, "application/xml");
            }

            var plainText = $"UserId : {userEntity.userId} , UserName : {userEntity.userName}";
            return Content(plainText, "text/plain");
        }

        //************************************************************************************************


        //This method is for xml packet format to json and text response
        [HttpPost("xml/{responseFormat}")]
        public IActionResult XmlToJsonAndText([FromBody] UserEntity newUser, string responseFormat)
        {
            var contentType = Request.ContentType?.ToLower();
            if (contentType == "application/xml" && responseFormat == "json")
            {
                
            UserEntity userEntity = new UserEntity();

                //var jsonResponse = JsonConvert.SerializeObject(new { ResponseMessage = $"{newUser.userId = Guid.NewGuid()}{newUser.userName}" });

                var jsonResponse = JsonConvert.SerializeObject(new
                {
                    userId = newUser.userId,
                    userName = newUser.userName
                });

                var jsonResult = new ContentResult
                {
                    Content = JsonConvert.SerializeObject(jsonResponse),
                    ContentType = "application/json",
                    StatusCode = 201
                };

                return jsonResult;

            } else if(contentType == "application/xml" && responseFormat == "text")
            {

                //newUser.userId = Guid.NewGuid();
                users.Add(newUser);

                // Converting the response to "text/plain" format
                var textResult = new ContentResult
                {
                    Content = $"UserId: {newUser.userId}, UserName: {newUser.userName}",
                    ContentType = "text/plain",
                    StatusCode = 201
                };

                return textResult;
            }

            //for xml to xml 

            //newUser.userId = Guid.NewGuid();
            users.Add(newUser);

            //converting the response to xml format
            var xmlSerializer = new XmlSerializer(typeof(UserEntity));
            var xmlResult = new ContentResult
            {
                Content = SerializeObjectToXml(newUser, xmlSerializer),
                ContentType = "application/xml",
                StatusCode = 201
            };

            return xmlResult;

        }

        //************************************************************************************************

        [HttpPatch("update/{id}/xml/{responseFormat}")]
        public ActionResult UpdateDataXmlPacket([FromBody] UserEntity userEntity,int id,string responseFormat)
        {
            var data = Get(id);

            if (data == null)
            {
                return NotFound();
            }

            data.Value.userId = userEntity.userId;
            data.Value.userName = userEntity.userName;

            if (responseFormat == "json")
            {

                var jsonResult = JsonConvert.SerializeObject(data);
                return Content(jsonResult, "application/json");

            }
            else if (responseFormat == "text")
            {
                var textResult = new ContentResult
                {
                    Content = $"UserId: {data.Value.userId}, UserName: {data.Value.userName}",
                    ContentType = "text/plain",
                    StatusCode = 200
                };

                return textResult;

            }
            else if (responseFormat == "xml")
            {
                UserEntity newUser = new UserEntity();
                newUser.userId = data.Value.userId;
                newUser.userName = data.Value.userName;

                var xmlSerializer = new XmlSerializer(typeof(UserEntity));
                var xmlResult = new ContentResult
                {
                    Content = SerializeObjectToXml(newUser, xmlSerializer),
                    ContentType = "application/xml",
                    StatusCode = 200
                };

                return xmlResult;
            }

            return BadRequest("Invalid response format specified");
        }

        //************************************************************************************************

        [HttpPatch("update/{id}/json/{responseFormat}")]
        public ActionResult UpdateDataJsonPacket([FromBody] UserEntity newUser,int id,string responseFormat)
        {
            var data = Get(id);

            if (data == null)
            {
                return NotFound(); 
            }
            

            data.Value.userName = newUser.userName;
           
            if (responseFormat == "json")
            {
                
                var jsonResult = JsonConvert.SerializeObject(data);
                return Content(jsonResult, "application/json");

            } else if (responseFormat == "text")
            {
                var textResult = new ContentResult
                {
                    Content = $"UserId: {data.Value.userId}, UserName: {data.Value.userName}",
                    ContentType = "text/plain",
                    StatusCode = 200
                };

                return textResult;

            } else if (responseFormat == "xml")
            {
                UserEntity userEntity = new UserEntity();
                userEntity.userId = data.Value.userId;
                userEntity.userName = data.Value.userName;
                
                var xmlSerializer = new XmlSerializer(typeof(UserEntity));
                var xmlResult = new ContentResult
                {
                    Content = SerializeObjectToXml(userEntity, xmlSerializer),
                    ContentType = "application/xml",
                    StatusCode = 200
                };

                return xmlResult;
            }

            return BadRequest("Invalid response format specified");
        }

        //************************************************************************************************

        [HttpPatch("update/{id}/text/{responseFormat}")]
        public ActionResult UpdateDataTextPacket([FromBody] string textData, string responseFormat)
        {

            UserEntity userEntity = new UserEntity();

            if (textData == null)
            {
                return BadRequest(responseFormat);
            }

            Regex userIdRegex = new Regex(@"userId = (\d+)");
            Regex userNameRegex = new Regex(@"userName = (\w+)");


            Match userIdMatch = userIdRegex.Match(textData);
            Match userNameMatch = userNameRegex.Match(textData);

            if (userIdMatch.Success && userNameMatch.Success)
            {
                string userId = userIdMatch.Groups[1].Value;
                userEntity.userId = Convert.ToInt32(userId);
                string userName = userNameMatch.Groups[1].Value;
                userEntity.userName = userName;

            }

            if (responseFormat.ToLower() == "json")
            {
                return new JsonResult(userEntity);
            }
            else if (responseFormat.ToLower() == "xml")
            {
                var serializer = new XmlSerializer(typeof(UserEntity));
                var xmlString = "";

                using (var stringWriter = new System.IO.StringWriter())
                {
                    serializer.Serialize(stringWriter, userEntity);
                    xmlString = stringWriter.ToString();
                }

                return Content(xmlString, "application/xml");
            }

            var plainText = $"UserId : {userEntity.userId} , UserName : {userEntity.userName}";
            return Content(plainText, "text/plain");
        }

        //************************************************************************************************


        //This method is for to convert json object to xml
        public static string SerializeObjectToXml(object obj , XmlSerializer xmlSerializer)
        {
            using (var writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        //**************************************** Gayatri ****************************************************
        //***************************************** Code ******************************************************



        // PUT api/<StudentsController>/5
        [HttpPut("{id}")]
        [HttpPut("json")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult UpdateJson([FromBody] UserEntity studentEntity)
        {
            var existingStudent = users.FirstOrDefault(s => s.userId == studentEntity.userId);
            if (existingStudent != null)
            {
                existingStudent.userName = studentEntity.userName;
                return Ok(new { Id = existingStudent.userId, Name = existingStudent.userName });
            }
            return NotFound(new { Message = "Not found" });
        }

        [HttpPut("json-xml")]
        [Consumes("application/json")]
        [Produces("application/xml")]

        public IActionResult UpdateJsonToXml([FromBody] UserEntity studentEntity)
        {
            var existingStudent = users.FirstOrDefault(s => s.userId == studentEntity.userId);
            if (existingStudent != null)
            {
                existingStudent.userName = studentEntity.userName;

                UserEntity newStudent = new UserEntity();
                newStudent.userId = studentEntity.userId;
                newStudent.userName = existingStudent.userName;

                var xmlSerializer = new XmlSerializer(typeof(UserEntity));
                var xmlResult = new ContentResult
                {
                    Content = SerializeObjectToXml(newStudent, xmlSerializer),
                    ContentType = "application/xml",
                    StatusCode = 200
                };

                return xmlResult;
            }

            return NotFound(new { Message = "Not Found" });
        }


        [HttpPut("json-text")]
        [Consumes("application/json")]
        [Produces("text/plain")]
        public ContentResult UpdateJsonToText([FromBody] UserEntity studentEntity)
        {
            var existingStudent = users.FirstOrDefault(s => s.userId == studentEntity.userId);

            if (existingStudent != null)
            {
                existingStudent.userName = studentEntity.userName;
                return Content($"Id: {existingStudent.userId} Name: {existingStudent.userName}", "text/plain");
            }

            return Content("Not found", "text/plain");
        }


        [HttpPut("xml-xml/{id}")]
        [Consumes("application/xml")]
        [Produces("application/xml")]
        public ActionResult UpdateXmlToXml([FromBody] UserEntity studentXml, int id)
        {
            if (ModelState.IsValid)
            {
                var existingStudent = users.FirstOrDefault(s => s.userId == studentXml.userId);

                if (existingStudent != null)
                {
                    existingStudent.userName = studentXml.userName;

                    UserEntity newStudent = new UserEntity();
                    newStudent.userId = studentXml.userId;
                    newStudent.userName = existingStudent.userName;

                    var xmlSerializer = new XmlSerializer(typeof(UserEntity));
                    var xmlResult = new ContentResult
                    {
                        Content = SerializeObjectToXml(newStudent, xmlSerializer),
                        ContentType = "application/xml",
                        StatusCode = 200
                    };

                    return xmlResult;
                }
            }

            return NotFound(new { Message = "Not found" });
        }

        [HttpPut("xml-json/{id}")]
        [Consumes("application/xml")]
        [Produces("application/json")]
        public ActionResult UpdateXmlToJson([FromBody] UserEntity studentEntity, int id)
        {
            if (ModelState.IsValid)
            {

                var existingStudent = users.FirstOrDefault(s => s.userId == studentEntity.userId);

                UserEntity stuEntity = new UserEntity();
                stuEntity.userId = studentEntity.userId;
                stuEntity.userName = studentEntity.userName;

                var jsonResponse = JsonConvert.SerializeObject(stuEntity);
                return Content(jsonResponse, "application/json");
            }

            return NotFound(new { Message = "Not found" });
        }

        [HttpPut("xml-plain/{id}")]
        [Consumes("application/xml")]
        [Produces("text/plain")]
        public ActionResult UpdateXmlToPlainText([FromBody] UserEntity studentEntity, int id)
        {
            if (ModelState.IsValid)
            {
                var existingStudent = users.FirstOrDefault(s => s.userId == id);

                if (existingStudent != null)
                {
                    existingStudent.userName = studentEntity.userName;

                    var plainTextResponse = $"UserId : {existingStudent.userId}, UserName : {existingStudent.userName}";

                    return Content(plainTextResponse, "text/plain");
                }

                return NotFound(new { Message = "Not found" });
            }

            return BadRequest(ModelState);
        }


        [HttpPut("text-xml/{id}")]
        [Consumes("text/plain")]
        [Produces("application/xml")]
        public ActionResult UpdateTextToXml([FromBody] string studentData, int id)
        {

            UserEntity stuEntity = new UserEntity();

            Regex idRegex = new Regex(@"Id = (\d+)");
            Regex nameRegex = new Regex(@"Name = (\w+)");

            Match IdMatch = idRegex.Match(studentData);
            Match NameMatch = nameRegex.Match(studentData);

            if (IdMatch.Success && NameMatch.Success)
            {
                string studentId = IdMatch.Groups[1].Value;
                stuEntity.userId = Convert.ToInt32(studentId);
                string userName = NameMatch.Groups[1].Value;
                stuEntity.userName = userName;

            }

            var xmlSerializer = new XmlSerializer(typeof(UserEntity));
            var xmlResult = new ContentResult
            {
                Content = SerializeObjectToXml(stuEntity, xmlSerializer),
                ContentType = "application/xml",
                StatusCode = 200
            };

            return xmlResult;
        }

        [HttpPut("text-json/{id}")]
        [Consumes("text/plain")]
        [Produces("application/json")]
        public IActionResult UpdateTextToJson([FromBody] string studentInfo, int id)
        {
            UserEntity stuEntity = new UserEntity();

            Regex idRegex = new Regex(@"Id = (\d+)");
            Regex nameRegex = new Regex(@"Name = (\w+)");

            Match IdMatch = idRegex.Match(studentInfo);
            Match NameMatch = nameRegex.Match(studentInfo);

            if (IdMatch.Success && NameMatch.Success)
            {
                string studentId = IdMatch.Groups[1].Value;
                stuEntity.userId = Convert.ToInt32(studentId);
                string userName = NameMatch.Groups[1].Value;
                stuEntity.userName = userName;

            }
            var jsonResponse = JsonConvert.SerializeObject(stuEntity);
            return Content(jsonResponse, "application/json");
        }

        [HttpPut("text-text/{id}")]
        [Consumes("text/plain")]
        [Produces("text/plain")]
        public IActionResult UpdateTextToText([FromBody] string stuInfo, int id)
        {
            UserEntity stuEntity = new UserEntity();

            Regex idRegex = new Regex(@"Id = (\d+)");
            Regex nameRegex = new Regex(@"Name = (\w+)");

            Match IdMatch = idRegex.Match(stuInfo);
            Match NameMatch = nameRegex.Match(stuInfo);

            if (IdMatch.Success && NameMatch.Success)
            {
                string studentId = IdMatch.Groups[1].Value;
                stuEntity.userId = Convert.ToInt32(studentId);
                string userName = NameMatch.Groups[1].Value;
                stuEntity.userName = userName;

            }
            var plainTextResponse = $"UserId : {stuEntity.userId}, UserName : {stuEntity.userName}";

            return Content(plainTextResponse, "text/plain");
        }






        [HttpDelete("{id}")]
        public void DeleteStudent(int id)
        {
            users.RemoveAll(s => s.userId == id);
        }

    }
}
