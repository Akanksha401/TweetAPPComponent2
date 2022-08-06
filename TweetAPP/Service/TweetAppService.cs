// <copyright file="TweetAppService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TweetAPP.Service
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TweetAPP.Models;
    using TweetAPP.Repositories;

    /// <summary>
    /// TweetAppService.
    /// </summary>
    public class TweetAppService : ITweetAppService
    {
        private readonly ITweetRepository tweetRepository;
        private ILogger<ITweetAppService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ITweetAppService"/> class.
        /// TweetAppService.
        /// </summary>
        /// <param name="tweetRepository">tweetRepository.</param>
        /// <param name="logger">logger.</param>
        public TweetAppService(ITweetRepository tweetRepository, ILogger<ITweetAppService> logger)
        {
            this.tweetRepository = tweetRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Comments.
        /// </summary>
        /// <param name="comment">comment.</param>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <param name="date">date.</param>
        /// <returns>response.</returns>
        public async Task<int> Comments(string comment, string username, string userName, string tweet)
        {
            try
            {
                this.logger.LogInformation("Service started commenting");
                DateTime date = DateTime.Now;
                var result = await this.tweetRepository.Comments(comment, username, userName, tweet, date);
                this.logger.LogInformation("Service completed commenting");
                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while posting comment");
                throw;
            }
        }

        /// <summary>
        /// ForgotPassword.
        /// </summary>
        /// <param name="emailId">emailId.</param>
        /// <param name="password">password.</param>
        /// <returns>response.</returns>
        public async Task<string> ForgotPassword(string emailId,string contactnumber, string password)
        {
            try
            {
                this.logger.LogInformation("Service started changing password");
                string message = string.Empty;
                if (password != null)
                {
                    password = this.EncryptPassword(password);
                }

                var result = await this.tweetRepository.ForgotPassword(emailId, contactnumber, password);
                if (result)
                {
                    message = "\"Changed Password\"";
                }
                else
                {
                    message = "Failed";
                }
                this.logger.LogInformation("Service finished changing password");
                return message;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while reseting password");
                throw;
            }
        }

        /// <summary>
        /// GetAllTweets.
        /// </summary>
        /// <returns>response.</returns>
        public async Task<List<UserTweets>> GetAllTweets()
        {
            try
            {
                this.logger.LogInformation("Service started getting all tweets");
                var result = await this.tweetRepository.GetAllTweets();
                this.logger.LogInformation("Service finished getting all tweets");
                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while retrieving all tweets");
                throw;
            }
        }

        /// <summary>
        /// GetAllUsers.
        /// </summary>
        /// <returns>response.</returns>
        public async Task<IList<RegisteredUser>> GetAllUsers()
        {
            try
            {
                this.logger.LogInformation("Service started getting all users");
                var result = await this.tweetRepository.GetAllUsers();
                this.logger.LogInformation("Service finished getting all users");
                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while retrieving all registered users");
                throw;
            }
        }

        /// <summary>
        /// GetTweetsByUser.
        /// </summary>
        /// <param name="username">username.</param>
        /// <returns>response.</returns>
        public async Task<List<UserTweets>> GetTweetsByUser(string username)
        {
            try
            {
                this.logger.LogInformation("Service started getting user");
                var result = await this.tweetRepository.GetTweetsByUser(username);
                this.logger.LogInformation("Service finished getting user");
                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while retrieving tweets by user");
                throw;
            }
        }

        /// <summary>
        /// GetUserProfile.
        /// </summary>
        /// <param name="username">username.</param>
        /// <returns>response.</returns>
        public async Task<User> GetUserProfile(string username)
        {
            try
            {
                this.logger.LogInformation("Service started getting userprofile");
                var result = await this.tweetRepository.GetUserProfile(username);
                this.logger.LogInformation("Service finished getting userprofile");
                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while retrieving user");
                throw;
            }
        }

        /// <summary>
        /// Likes.
        /// </summary>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        public async Task<int> Likes(string username, string tweet)
        {
            try
            {
                this.logger.LogInformation("Service started liking tweet");
                var result = await this.tweetRepository.Likes(username, tweet);
                this.logger.LogInformation("Service completed liking tweet");
                return result;

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while getting likes");
                throw;
            }
        }

        /// <summary>
        /// PostTweet.
        /// </summary>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        public async Task<string> PostTweet(Tweet tweet)
        {
            try
            {
                this.logger.LogInformation("Service started posting tweet");
                string message = string.Empty;
                var result = await this.tweetRepository.PostTweet(tweet);
                if (result > 0)
                {
                    message = "\"Posted\"";
                }
                else
                {
                    message = "Error occured";
                }
                this.logger.LogInformation("Service completed posting tweet");
                return message;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while posting tweets");
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
        public async Task<string> UpdatePassword(string emailId, string oldpassword, string newPassword)
        {
            try
            {
                this.logger.LogInformation("Service started updating password");
                string message = string.Empty;
                if (newPassword != null && oldpassword != null)
                {
                    newPassword = this.EncryptPassword(newPassword);
                    oldpassword = this.EncryptPassword(oldpassword);
                }

                var result = await this.tweetRepository.UpdatePassword(emailId, oldpassword, newPassword);
                if (result)
                {
                    message = "\"Updated Successfully\"";
                }
                else
                {
                    message = "Update Failed";
                }
                this.logger.LogInformation("Service completed updating password");
                return message;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while updating password");
                throw;
            }
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="emailId">emailId.</param>
        /// <param name="password">password.</param>
        /// <returns>response.</returns>
        public async Task<User> UserLogin(string emailId, string password)
        {
            try
            {
                this.logger.LogInformation("Service started login");
                if (password != null)
                {
                    password = this.EncryptPassword(password);
                }

                var result = await this.tweetRepository.Login(emailId, password);
                this.logger.LogInformation("Service completed login");
                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while logging in");
                throw;
            }
        }

        /// <summary>
        /// Register.
        /// </summary>
        /// <param name="users">users.</param>
        /// <returns>response.</returns>
        public async Task<string> Register(User users)
        {
            try
            {
                this.logger.LogInformation("Service started Registering");
                if (users != null)
                {
                    string message = string.Empty;
                    var validate = await this.tweetRepository.ValidateEmailId(users.EmailId);
                    var uservalidate = await this.tweetRepository.ValidateName(users.FirstName, users.Username);
                    if (validate == null && uservalidate == null)
                    {
                        users.Password = this.EncryptPassword(users.Password);
                        var result = await this.tweetRepository.Register(users);
                        if (result > 0)
                        {
                            message = "Successfully registerd";
                        }
                        else
                        {
                            message = "Registration failed";
                        }
                    }
                    else
                    {
                        if (validate != null)
                        {
                            message = "EmailId is already used";
                        }
                        else
                        {
                            message = "Username is already taken";
                        }
                    }
                    this.logger.LogInformation("Service completed Registering");
                    return message;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while registering");
                throw;
            }
        }

        public async Task<List<UserComments>> GetComments(string username, string tweet)
        {
            try
            {
                
                var result = await this.tweetRepository.GetComments(username, tweet);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while comments");
                throw;
            }
        }

        public async Task<string> DeleteTweet(string username, string tweet)
        {
            try
            {
                this.logger.LogInformation("Service started deleting tweets");
                string message = string.Empty;
                var result = await this.tweetRepository.DeleteTweet(username, tweet);
                if (result>0)
                {
                    this.logger.LogInformation("Service completed deleting tweets");
                    return message = "\"Deleted\"";
                }
                else
                {
                    this.logger.LogInformation("Service deleting tweets failed");
                    return message = "\"Failed to Delete\"";
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while comments");
                throw;
            }
        }

        private string EncryptPassword(string password)
        {
            string message = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            message = Convert.ToBase64String(encode);
            return message;
        }
    }
}