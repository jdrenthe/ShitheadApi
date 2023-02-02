using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QRCoder;
using ShitheadApi.Attributes;
using ShitheadApi.Models;
using ShitheadApi.Models.Entities;
using ShitheadApi.Services;
using ShitheadApi.Settings;

namespace ShitheadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly Jwt _jwt;
        private readonly IEmailService _emailSender;
        private readonly ICryptoService _crypto;

        public UserController(DatabaseContext databaseContext,
                              UserManager<User> userManager,
                              IOptionsSnapshot<Jwt> jwtSettings,
                              IEmailService emailSender,
                              ICryptoService crypto)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _jwt = jwtSettings.Value;
            _emailSender = emailSender;
            _crypto = crypto;
        }

        #region User

        [HttpGet]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<User>> GetUser(string uid, string phh)
        {
            var errorMessage = $"Couldn't get user with id: {uid}.";
            DataBundel<User> errorResult = new(null, false, errorMessage);

            try
            {
                Guid id = new(_crypto.Decrypt(uid, "BVV3Qhtsb6iitCCyvbrvFJQnmmJTxNO1"));
                var hash = _crypto.Decrypt(phh, "BVV3Qhtsb6iitCCyvbrvFJQnmmJTxNO1");
                var result = await _databaseContext.User
                                                   .Include(u => u.Friends)
                                                       .ThenInclude(f => f.Friend)
                                                   .FirstOrDefaultAsync(u => u.Id == id && u.PasswordHash == hash);

                if (result == null) return errorResult;

                return new(result, true, null);
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpGet]
        [AuthorizeRoles(AuthorizationRole.ShitheadAdmin)]
        public async Task<DataBundel<List<User>>> GetAllUsers()
        {
            var errorMessage = $"Couldn't get all user.";
            DataBundel<List<User>> errorResult = new(null, false, errorMessage);

            try
            {
                var result = await _databaseContext.User.ToListAsync();

                return result != null ? new(result, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpGet]
        public async Task<DataBundel<User>> GetUserById(string userId)
        {
            var errorMessage = $"Couldn't get user with id: {userId}.";
            DataBundel<User> errorResult = new(null, false, errorMessage);

            try
            {
                var result = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == new Guid(userId));
                if (result == null) return errorResult;

                return new(result, true, null);
            }
            catch
            {
                return errorResult;
            }
        }

        /// <summary>
        /// Sing up
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DataBundel<User>> SingUp(User user)
        {
            var errorMessage = $"Couldn't sing-up user with id: {user.Id}.";
            DataBundel<User> errorResult = new(null, false, errorMessage);

            try
            {
                user.NotSplitUserName = true;
                user.UserName = user.UserName + "@" + user.Id.ToString();

                var userResult = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
                if (userResult != null) return errorResult;

                var result = await _userManager.CreateAsync(user, user.Password);
                if (!result.Succeeded) return errorResult;

                userResult = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
                if (userResult == null) return errorResult;

                //Give user credentials
                userResult.NotSplitUserName = true;

                await _userManager.AddToRoleAsync(userResult, AuthorizationRole.ShitheadUser);
                if (!await SendWelcomeEmail(user)) return errorResult;

                user.NotSplitUserName = false;
                return new(null, true, null);
            }
            catch
            {
                return errorResult;
            }
        }

        /// <summary>
        /// Sing in
        /// </summary>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DataBundel<Credentials>> SingIn(SingIn singIn)
        {
            var errorMessage = $"Couldn't sing-in user with id: {singIn.Email}.";
            DataBundel<Credentials> errorResult = new(null, false, errorMessage);

            try
            {
                if (!string.IsNullOrEmpty(singIn.EmailToken))
                {
                    singIn.Password = _crypto.Base64Decode(singIn.Password);
                    singIn.Email = _crypto.Base64Decode(singIn.Email);
                }

                var userResult = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == singIn.Email);
                if (userResult == null) return errorResult;

                if (!string.IsNullOrEmpty(singIn.EmailToken))
                {
                    var resultEmail = await _userManager.ConfirmEmailAsync(userResult, _crypto.Base64Decode(singIn.EmailToken));
                    if (!resultEmail.Succeeded) return errorResult;
                }

                if (!string.IsNullOrEmpty(singIn.PasswordToken))
                {
                    var resetPassword = await ResetPassword(userResult.Id, singIn.PasswordToken, singIn.Password);
                    if (!resetPassword) return errorResult;
                }

                var passwordResult = await _userManager.CheckPasswordAsync(userResult, singIn.Password);
                if (!passwordResult) return errorResult;

                var credentails = await GetCredentials(userResult);
                return new(credentails, true, null);
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<User>> UpdateUser(User user)
        {
            var errorMessage = $"Couldn't update user with id: {user.Id}.";
            DataBundel<User> errorResult = new(null, false, errorMessage);

            try
            {
                var userResult = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
                if (userResult == null) return errorResult;

                userResult.UserName = user.UserName + "@" + user.Id.ToString();
                userResult.Surname = user.Surname;
                userResult.Email = user.Email;
                userResult.EmailConfirmed = user.EmailConfirmed;
                userResult.PhoneNumber = user.PhoneNumber;
                userResult.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                userResult.Image = user.Image;
                userResult.Blocked = user.Blocked;
                userResult.NotSplitUserName = true;

                var result = await _userManager.UpdateAsync(userResult);
                return result.Succeeded ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpPost]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public async Task<DataBundel<List<UserFriend>>> AddFriends(List<UserFriend> friends)
        {
            var errorMessage = $"Couldn't add friends.";
            DataBundel<List<UserFriend>> errorResult = new(null, false, errorMessage);

            try
            {
                _databaseContext.Friend.AddRange(friends);

                var saveChanges = await _databaseContext.SaveChangesAsync();
                return saveChanges > 0 ? new(null, true, null) : errorResult;
            }
            catch
            {
                return errorResult;
            }
        }

        [HttpGet]
        [AuthorizeRoles(AuthorizationRole.ShitheadUser)]
        public DataBundel<string> GeneratQR(string value)
        {
            var errorMessage = "Couldn't generate qr-code.";
            DataBundel<string> errorResult = new(null, false, errorMessage);

            try
            {
                // For origin view: https://github.com/codebude/QRCoder
                QRCodeGenerator qrGenerator = new();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new(qrCodeData);
                Bitmap bitmap = qrCode.GetGraphic(20);

                string base64 = string.Empty;
                using (MemoryStream memoryStream = new())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    byte[] byteImage = memoryStream.ToArray();
                    base64 = $"data:image/png;base64,{Convert.ToBase64String(byteImage)}";
                }  

                return new(base64, true, null);
            }
            catch
            {
                return errorResult;
            }
        }

        /// <summary>
        /// Resets password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        private async Task<bool> ResetPassword(Guid userId, string token, string newPassword)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.NotSplitUserName = true;

            var result = await _userManager.ResetPasswordAsync(user, _crypto.Base64Decode(token), newPassword);
            return result.Succeeded;
        }

        /// <summary>
        /// Gets credentials for the given user
        /// </summary>
        /// <param name="user"></param>
        private async Task<Credentials> GetCredentials(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new Credentials
            {
                Uid = _crypto.Encrypt(user.Id.ToString(), "BVV3Qhtsb6iitCCyvbrvFJQnmmJTxNO1"),
                Phh = _crypto.Encrypt(user.PasswordHash, "BVV3Qhtsb6iitCCyvbrvFJQnmmJTxNO1"),
                Jwt = GenerateJwt(user, roles)
            };
        }

        /// <summary>
        /// Generates JOSN web token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        private string GenerateJwt(User user, IList<string> roles)
        {
            List<Claim> claims = new()
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roleClaim = roles.Select(r => new Claim(ClaimTypes.Role, r));
            claims.AddRange(roleClaim);

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwt.Secret));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_jwt.ExpirationInDays));

            JwtSecurityToken token = new(issuer: _jwt.Issuer,
                                         audience: _jwt.Issuer,
                                         claims,
                                         expires: expires,
                                         signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion

        #region Email

        [HttpPost]
        public async Task<DataBundel<User>> SendPasswordResetTokenEmail(Email email)
        {
            var errorMessage = "Couldn't create a password token.";
            DataBundel<User> errorResult = new(null, false, errorMessage);

            try
            {
                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email.Adress);
                if (user == null) return errorResult;

                user.NotSplitUserName = true;

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (string.IsNullOrEmpty(token)) return errorResult;

                user.NotSplitUserName = false;

                var base64 = _crypto.Base64Encode(token);
                email.Html = email.Html.Replace("{passwordToken}", $"{base64.Insert(base64.Length / 2, "/")}/{_crypto.Base64Encode(email.Adress)}");
                email.Name = user.UserName;

                return new(null, await SendEmail(email), null);
            }
            catch
            {
                return errorResult;
            }
        }

        /// <summary>
        /// Sends welkome email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> SendWelcomeEmail(User user)
        {
            try
            {
                if (user == null) return false;

                var result = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
                if (result == null) return false;

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(result);
                if (string.IsNullOrEmpty(emailToken)) return false;

                // Email token
                var tokenBase64 = _crypto.Base64Encode(emailToken);
                var passwordBase64 = _crypto.Base64Encode(user.Password);
                var emailBase64 = _crypto.Base64Encode(user.Email);

                user.HtmlEmail.Html = user.HtmlEmail.Html.Replace("{emailToken}", $"{tokenBase64.Insert(tokenBase64.Length / 2, "/")}/{passwordBase64}/{emailBase64}");
                return await SendEmail(user.HtmlEmail);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<bool> SendEmail(Email email)
        {
            try
            {
                if (email == null) return false;

                await _emailSender.SendEmail(email);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
