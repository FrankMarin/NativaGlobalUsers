using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NativaGlobalUsers.Models;
using NativaGlobalUsers.Repository;
using System;
using System.Net;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NativaGlobalUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IRepository<User> userRepository;
        protected UserResponse userResponse;

        public UserController(ILogger<UserController> logger, IRepository<User> userRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.userResponse = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResponse>> GetAll()
        {
            try
            {
                userResponse.StatusCode = HttpStatusCode.OK;
                userResponse.Result = await this.userRepository.GetAll();
            }
            catch (Exception exception)
            {
                userResponse.IsSuccess = false;
                userResponse.ErrorMessages = new List<string> { exception.ToString()};
            }

            return Ok(userResponse);
        }

        [HttpGet("{id}", Name ="GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> GetUser(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest(GetUserRequiredResponse());
                }

                var user = await this.userRepository.Get(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(GetUserNotFoundResponse());
                }

                userResponse.StatusCode = HttpStatusCode.OK;
                userResponse.Result = user;

                return Ok(userResponse);
            }
            catch (Exception exception)
            {
                userResponse.IsSuccess = false;
                userResponse.ErrorMessages = new List<string> { exception.ToString() };
            }

            return userResponse;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> CreateUser([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!IsValidUserModel(user))
                {
                    return GetBadRequestResponse(
                        new List<string>() {
                        { "Incomplete model or password do not fit the minimal requirements" },
                        { "minimum of 8 characters, alphanumeric, upper and lower case, and at least one special character" }
                    });
                }

                if (await userRepository.Get(u => u.Name.ToLower() == user.Name.ToLower()) != null)
                {
                    return GetBadRequestResponse("Duplicated, User with same name already exist in the database");
                }

                User model = new()
                {
                    Name = user.Name,
                    Password = user.Password
                };

                await userRepository.Create(model);

                userResponse.StatusCode = HttpStatusCode.OK;
                userResponse.Result = model;

                return CreatedAtRoute("GetUser", new { id = model.Id }, userResponse);
            }
            catch (Exception exception)
            {
                userResponse.IsSuccess = false;
                userResponse.ErrorMessages = new List<string> { exception.ToString() };
            }

            return userResponse;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<UserResponse> Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    return GetUserRequiredResponse();
                }

                var user = await userRepository.Get(u => u.Id == id);

                if (user == null)
                {
                    return GetUserNotFoundResponse();
                }

                await this.userRepository.Remove(user);

                userResponse.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception exception)
            {
                userResponse.IsSuccess = false;
                userResponse.ErrorMessages = new List<string> { exception.ToString() };
            }

            return userResponse;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<UserResponse> Update(int id, [FromBody] User updateUser)
        {
            try
            {
                if (updateUser == null || id != updateUser.Id)
                {
                    return GetUserRequiredResponse();
                }

                var user = await userRepository.Get(u => u.Id == id);

                if (user == null)
                {
                    return GetUserNotFoundResponse();
                }

                if (!IsValidUserModel(updateUser))
                {
                    return GetBadRequestResponse(
                        new List<string>() {
                        { "Incomplete model or password do not fit the minimal requirements" },
                        { "minimum of 8 characters, alphanumeric, upper and lower case, and at least one special character" }
                    });
                }

                await this.userRepository.UpdateUser(updateUser);

                userResponse.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception exception)
            {
                userResponse.IsSuccess = false;
                userResponse.ErrorMessages = new List<string> { exception.ToString() };
            }

            return userResponse;
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProperty(int id, JsonPatchDocument<User> patchDocument)
        {
            if (patchDocument == null || id == 0)
            {
                return BadRequest(GetBadRequestResponse("Wrong information provided"));
            }

            var user = await this.userRepository.Get(u => u.Id == id, tracked: false);

            if (user == null)
            {
                return NotFound(GetUserNotFoundResponse());
            }

            patchDocument.ApplyTo(user, ModelState);

            if (!ModelState.IsValid)
            {
                if (patchDocument == null || id == 0)
                {
                    return BadRequest(GetBadRequestResponse("Model User can be patched with information provided."));
                }
            }

            await this.userRepository.UpdateUser(user);

            return NoContent();
        }


        private UserResponse GetBadRequestResponse(string message)
        {
            return new UserResponse()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMessages = new List<string> { message }
            };
        }

        private UserResponse GetBadRequestResponse(List<string> messages)
        {
            return new UserResponse()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMessages = messages
            };
        }

        private UserResponse GetUserRequiredResponse()
        {
            return new UserResponse()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMessages = new List<string> { "User Id is required!" }
            };
        }

        private UserResponse GetUserNotFoundResponse()
        {
            return new UserResponse()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.NotFound,
                ErrorMessages = new List<string> { "User Id Not found!" }
            };
        }

        private static bool IsValidUserModel(User user)
        {
            bool nameValidation = !string.IsNullOrEmpty(user.Name);
            bool passwordValidation = IsPasswordValid(user.Password);

            return nameValidation && passwordValidation;
        }

        private static bool IsPasswordValid(string password)
        {
            var hasMinimumLength = new Regex(@".{8,}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasDigit = new Regex(@"[0-9]+");
            var hasSpecialChar = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            return hasMinimumLength.IsMatch(password)
                   && hasLowerChar.IsMatch(password)
                   && hasUpperChar.IsMatch(password)
                   && hasDigit.IsMatch(password)
                   && hasSpecialChar.IsMatch(password);
        }
    }
}
