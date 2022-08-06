using System;
using Xunit;
using Moq;
using TweetAPP.Repositories;
using TweetAPP.Service;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TweetAPP.Models;
using System.Collections.Generic;

namespace TestTweetApp
{
    public class TweetAppServiceTest
    {
        private readonly Mock<ILogger<TweetAppService>> _mockLogger;
        private readonly  Mock<ITweetRepository> _mocktweetRepository;
        private readonly TweetAppService _service;

        public TweetAppServiceTest()
        {
            _mockLogger = new Mock<ILogger<TweetAppService>>();
            _mocktweetRepository = new Mock<ITweetRepository>();
            _service = new TweetAppService(_mocktweetRepository.Object, _mockLogger.Object);
        }
       
        [Fact]
        public async Task CommentsTest()
        {
            string username = "akanksha";
            string userName = "bijin";
            string comment = "Hello World";
            string tweet = "Hello Akanksha";
            DateTime time = DateTime.Now;
           _mocktweetRepository.Setup(x => x.Comments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(2);
            var result = await _service.Comments(comment, username, userName, tweet);
            Assert.Equal(2, result);
            _mocktweetRepository.Verify(x => x.Comments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once());
        }

        [Fact]
        public async Task GetAllUsersTest()
        {
            RegisteredUser user = new RegisteredUser
            {
                Username = "akanksha",
                FirstName = "Akanksha",
                LastName = "Sahu",
                ImageName = "image.jpg"
            };
            List<RegisteredUser> Users = new List<RegisteredUser>();
            Users.Add(user);
            _mocktweetRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(Users);
            var result = await this._service.GetAllUsers();
            Assert.NotNull(result);
            Assert.Equal(Users[0].FirstName, result[0].FirstName);
            Assert.Equal(Users[0].LastName, result[0].LastName);
            Assert.Equal(Users[0].Username, result[0].Username);
            Assert.Equal(Users[0].ImageName, result[0].ImageName);
            _mocktweetRepository.Verify(x => x.GetAllUsers(), Times.Once);
        }

        [Theory]
        [InlineData("akanksha")]
        [InlineData("bijin")]
        [InlineData("Navya")]
        public async Task GetTweetsByUserTest(string username)
        {
            UserTweets user = new UserTweets
            {
                UserName = username,
                Tweets = "My Name is Akanksha",
                FirstName = "Akanksha",
                LastName = "Sahu",
                Imagename = "image.jpg",
                TweetDate = System.DateTime.Now,
                Likes = 2,
            };
            List<UserTweets> users = new List<UserTweets>();
            users.Add(user);
            string UserName = username;
            _mocktweetRepository.Setup(x => x.GetTweetsByUser(It.IsAny<string>())).ReturnsAsync(users);
            var result = await this._service.GetTweetsByUser(UserName);
            Assert.NotNull(result);
            Assert.Equal(users[0].FirstName, result[0].FirstName);
            Assert.Equal(users[0].LastName, result[0].LastName);
            Assert.Equal(users[0].UserName, result[0].UserName);
            Assert.Equal(users[0].Imagename, result[0].Imagename);
            Assert.Equal(users[0].Likes, result[0].Likes);
            _mocktweetRepository.Verify(x => x.GetTweetsByUser(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData("akanksha")]
        [InlineData("bijin")]
        [InlineData("Navya")]
        public async Task GetUserProfileTest(string username)
        {
            User user = new User
            {
                UserId = 3,
                Username = username,
                FirstName = "Akanksha",
                LastName = "Sahu",
                EmailId = "akanksha@gmail.com",
                ContactNumber = "7566618557",
                Password = "Akanksha@12",
                Tweet = new List<Tweet>(),
                ImageName = "image.jpg"
            };
           string UserName = username;
            _mocktweetRepository.Setup(x => x.GetUserProfile(It.IsAny<string>())).ReturnsAsync(user);
            var result = await this._service.GetUserProfile(UserName);
            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
            _mocktweetRepository.Verify(x => x.GetUserProfile(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData("akanksha" , "Hello World")]
        [InlineData("bijin", "Hello World")]
        public async Task LikesTest(string username, string tweet)
        {
            string userName = username;
            string tweets = tweet;
            _mocktweetRepository.Setup(x => x.Likes(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(2);
            var result = await this._service.Likes(userName, tweets);
            Assert.Equal(2, result);
            _mocktweetRepository.Verify(x => x.Likes(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task PostTweet()
        {
            Tweet tweet = new Tweet
            {
                Id = 1,
                UserId = 3,
                Username = "akanksha",
                FirstName = "Akanksha",
                LastName = "Sahu",
                Tweets = "Heloo World",
                TweetDate = System.DateTime.Now
            };
            _mocktweetRepository.Setup(x => x.PostTweet(It.IsAny<Tweet>())).ReturnsAsync(2);
            var result = await this._service.PostTweet(tweet);
            Assert.Equal("\"Posted\"", result);
            _mocktweetRepository.Verify(x => x.PostTweet(It.IsAny<Tweet>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePassword()
        {
            string emailId = "akanksha@gmail.com";
            string oldpass = "Akanksha@12";
            string newpass = "Akanksha@123";
            _mocktweetRepository.Setup(x => x.UpdatePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._service.UpdatePassword(emailId, oldpass, newpass);
            Assert.NotNull(result);
            Assert.Equal("\"Updated Successfully\"", result);
            _mocktweetRepository.Verify(x => x.UpdatePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Once);
        }

        [Fact]
        public async Task UserLoginTest()
        {
            string userName = "akanksha";
            string password = "Akanksha@123";
            User user = new User
            {
                UserId = 3,
                Username = "akanksha",
                FirstName = "Akanksha",
                LastName = " Sahu",
                Tweet = new List<Tweet>(),
                Password = "Akanksha@123",
                ContactNumber = "7566618557"

            };
            _mocktweetRepository.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);
            var result = await this._service.UserLogin(userName, password);
            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Password, result.Password);
            _mocktweetRepository.Verify(x => x.Login(It.IsAny<string>(), It.IsAny<string>()),Times.Once);
        }

        [Theory]
        [InlineData(true , "\"Changed Password\"")]
        [InlineData(false, "Failed")]
        public async Task ForgotPasswordTest(bool password , string status)
        {
            string emaildId = "akanksha@gmail.com";
            string conNumber = "7566618557";
            string pass = "Akanksha@123";
            _mocktweetRepository.Setup(x => x.ForgotPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(password);
            var result = await this._service.ForgotPassword(emaildId, conNumber, pass);
            Assert.NotNull(result);
            Assert.Equal(status, result);
            _mocktweetRepository.Verify(x => x.ForgotPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Once);
        }


        [Theory]
        [InlineData("Successfully registerd" , 2)]
        [InlineData("Registration failed", -1)]
        [InlineData("Registration failed", 0)]
        public async Task RegisterTest(string status, int code)
        {
            User user = new User
            {
                UserId = 3,
                Username = "akanksha",
                FirstName = "Akanksha",
                LastName = " Sahu",
                Tweet = new List<Tweet>(),
                Password = "Akanksha@123",
                ContactNumber = "7566618557"
            };
            _mocktweetRepository.Setup(x => x.Register(It.IsAny<User>())).ReturnsAsync(code);
            var result = await this._service.Register(user);
            Assert.NotNull(result);
            Assert.Equal(status, result);
            _mocktweetRepository.Verify(x => x.Register(It.IsAny<User>()),Times.Once);
        }

        [Theory]
        [InlineData("\"Deleted\"", 1)]
        [InlineData("\"Failed to Delete\"", 0)]
        [InlineData("\"Failed to Delete\"", -1)]
        public async Task DeleteTweetTest(string status , int code)
        {
            string userName = "akanksha";
            string tweet = "Hello Wolrd";
            _mocktweetRepository.Setup(x =>x.DeleteTweet(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(code);
            var result = await this._service.DeleteTweet(userName, tweet);
            Assert.NotNull(result);
            Assert.Equal(status, result);
            _mocktweetRepository.Verify(x => x.DeleteTweet(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

    }
}
