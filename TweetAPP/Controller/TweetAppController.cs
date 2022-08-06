﻿// <copyright file="TweetAppController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TweetAPP.Controller
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Confluent.Kafka;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using TweetAPP.Models;
    using TweetAPP.Service;

    /// <summary>
    /// TweetAppController.
    /// </summary>
    [Route("api/v1.0/tweets/")]
    [ApiController]
    public class TweetAppController : ControllerBase
    {
        private readonly ITweetAppService tweetAppService;
        private readonly IConfiguration configuration;
        private ILogger<TweetAppController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetAppController"/> class.
        /// TweetAppController.
        /// </summary>
        /// <param name="tweetAppService">tweetAppService.</param>
        /// <param name="logger">logger.</param>
        /// <param name="configuration">configuration.</param>
        public TweetAppController(ITweetAppService tweetAppService, ILogger<TweetAppController> logger, IConfiguration configuration)
        {
            this.tweetAppService = tweetAppService;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Register User.
        /// </summary>
        /// <param name="user">user.</param>
        /// <returns>response.</returns>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                var result = await this.tweetAppService.Register(user);
                using (var producer =
                 new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build())
                {
                    try
                    {
                        Console.WriteLine(producer.ProduceAsync("tweetapp_topic", new Message<Null, string> { Value = user.Username+ " is registerd successfully" })
                            .GetAwaiter()
                            .GetResult());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Oops, something went wrong: {e}");
                    }
                }
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while registering user");
                throw;
            }
        }

        /// <summary>
        /// Login User.
        /// </summary>
        /// <param name="emailId">emailID.</param>
        /// <param name="password">password.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("login/{username},{password}")]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                this.logger.LogInformation("Controller Login Started");
                Token token = null;
                var result = await this.tweetAppService.UserLogin(username, password);
                if (result != null)
                {
                    token = new Token() { UserId = result.UserId, Username = result.Username, Tokens = this.GenerateJwtToken(username), Message = "Success" };
                }
                else
                {
                    token = new Token() { Tokens = null, Message = "UnSuccess" };
                }
                this.logger.LogInformation("Controller Login Finished");
                using (var producer =
                 new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build())
                {
                    try
                    {
                        Console.WriteLine(producer.ProduceAsync("tweetapp_topic", new Message<Null, string> { Value = username + " logged in!" })
                            .GetAwaiter()
                            .GetResult());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Oops, something went wrong: {e}");
                    }
                }
                return this.Ok(token);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while login user");
                throw;
            }
        }

        /// <summary>
        /// Post Tweet.
        /// </summary>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        [HttpPost]
        [Route("tweet")]
        public async Task<IActionResult> Tweet(Tweet tweet)
        {
            try
            {
                this.logger.LogInformation("Controller Tweet started");
                var result = await this.tweetAppService.PostTweet(tweet);
                this.logger.LogInformation("Controller Tweet Posted");
                using (var producer =
                 new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build())
                {
                    try
                    {
                        Console.WriteLine(producer.ProduceAsync("tweetapp_topic", new Message<Null, string> { Value =  tweet.Tweets +" is posted successfully" })
                            .GetAwaiter()
                            .GetResult());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Oops, something went wrong: {e}");
                    }
                }
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while posting user tweet");
                throw;
            }
        }

        /// <summary>
        /// Delete Tweet.
        /// </summary>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        [HttpDelete]
        [Route("tweetdelete/{username},{tweet}")]
        public async Task<IActionResult> DeleteTweet(string username, string tweet)
        {
            try
            {
                this.logger.LogInformation("Controller started tweet delete");
                var result = await this.tweetAppService.DeleteTweet(username, tweet);
                this.logger.LogInformation("Controller finished tweet delete");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while Deleteing user tweet");
                throw;
            }
        }

        /// <summary>
        /// Get All Users.
        /// </summary>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("users/all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                this.logger.LogInformation("Controller started fetching all users");
                var result = await this.tweetAppService.GetAllUsers();
                this.logger.LogInformation("Controller completed fetching all users");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while retrieving users");
                throw;
            }
        }

        /// <summary>
        /// Get Tweets By Users.
        /// </summary>
        /// <param name="username">username.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("user/search/{username}")]
        public async Task<IActionResult> GetTweetsByUser(string username)
        {
            try
            {
                this.logger.LogInformation("Controller started fetching users by userName");
                var result = await this.tweetAppService.GetTweetsByUser(username);
                this.logger.LogInformation("Controller completed fetching users by userName");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while fetching tweets by user");
                throw;
            }
        }

        /// <summary>
        /// Get All Tweets.
        /// </summary>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllTweets()
        {
            try
            {
                this.logger.LogInformation("Controller started fetching all tweets");
                var result = await this.tweetAppService.GetAllTweets();
                this.logger.LogInformation("Controller completed fetching all tweets");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while fetching user tweets");
                throw;
            }
        }

        /// <summary>
        /// Get All Comments.
        /// </summary>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("allcomments/{username},{tweet}")]
        public async Task<IActionResult> GetAllComments(string username, string tweet)
        {
            try
            {
                this.logger.LogInformation("Controller started fetching all tweets");
                var result = await this.tweetAppService.GetComments(username, tweet);
                this.logger.LogInformation("Controller completed fetching all tweets");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while fetching user comments");
                throw;
            }
        }

        /// <summary>
        /// GetUserProfile.
        /// </summary>
        /// <param name="username">username.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("user/{username}")]
        public async Task<IActionResult> GetUserProfile(string username)
        
       {
            try
            {
                this.logger.LogInformation("Controller started fetching users by userName");
                var result = await this.tweetAppService.GetUserProfile(username);
                this.logger.LogInformation("Controller completed fetching users by userName");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while fetching user");
                throw;
            }
        }

        /// <summary>
        /// UpdatePassword.
        /// </summary>
        /// <param name="emailId">emailId.</param>
        /// <param name="oldpassword">oldpassword.</param>
        /// <param name="newPassword">newPassword.</param>
        /// <returns>response.</returns>
        [HttpPut]
        [Route("update/{emailId},{oldpassword},{newpassword}")]
        public async Task<IActionResult> UpdatePassword(string emailId, string oldpassword, string newPassword)
        {
            try
            {
                this.logger.LogInformation("Controller started updating password");
                var result = await this.tweetAppService.UpdatePassword(emailId, oldpassword, newPassword);
                this.logger.LogInformation("Controller completed updating password");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while updating user password");
                throw;
            }
        }

        /// <summary>
        /// ForgotPassword.
        /// </summary>
        /// <param name="emailId">emailId.</param>
        /// <param name="password">password.</param>
        /// <returns>response.</returns>
        [HttpPut]
        [Route("forgot/{emailId},{contactnumber},{password}")]
        public async Task<IActionResult> ForgotPassword(string emailId,string contactnumber, string password)
        {
            try
            {
                this.logger.LogInformation("Controller started changing password");
                var result = await this.tweetAppService.ForgotPassword(emailId, contactnumber, password);
                this.logger.LogInformation("Controller completed changing password");
                using (var producer =
                 new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build())
                {
                    try
                    {
                        Console.WriteLine(producer.ProduceAsync("tweetapp_topic", new Message<Null, string> { Value =  " Password changed successfully" })
                            .GetAwaiter()
                            .GetResult());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Oops, something went wrong: {e}");
                    }
                }
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while reseting user password");
                throw;
            }
        }

        /// <summary>
        /// Comments.
        /// </summary>
        /// <param name="comment">comment.</param>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <param name="date">date.</param>
        /// <returns>response.</returns>
        [HttpPost]
        [Route("reply/{comment},{username},{Name},{tweet}")]
        public async Task<IActionResult> PostComment(string comment, string username, string Name, string tweet)
        {
            try
            {
                this.logger.LogInformation("Controller started commenting");
                var result = await this.tweetAppService.Comments(comment, username,Name, tweet);
                this.logger.LogInformation("Controller completed commenting");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while posting user comment");
                throw;
            }
        }

        /// <summary>
        /// Likes.
        /// </summary>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("likes/{username},{tweet}")]
        public async Task<IActionResult> GetLikes(string username, string tweet)
        {
            try
            {
                this.logger.LogInformation("Controller started like");
                var result = await this.tweetAppService.Likes(username, tweet);
                this.logger.LogInformation("Controller completed like");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while getting user like");
                throw;
            }
        }

        private string GenerateJwtToken(string emailId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, emailId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, emailId),
                new Claim(ClaimTypes.Role, emailId),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JwtKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //recommended is 5 min
            var expires = DateTime.Now.AddDays(Convert.ToDouble(this.configuration["JwtExpireDays"]));
            var token = new JwtSecurityToken(
                this.configuration["JwtIssuer"],
                this.configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
