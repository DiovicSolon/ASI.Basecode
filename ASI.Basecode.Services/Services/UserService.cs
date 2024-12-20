﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public LoginResult AuthenticateUser(string username, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.UserName == username &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public void AddUser(UserViewModel model)
        {
            var user = new User();
            if (!_repository.UserExists(model.UserName))
            {
                _mapper.Map(model, user);
                user.Password = PasswordManager.EncryptPassword(model.Password);
                user.CreatedTime = DateTime.Now;
                user.UpdatedTime = DateTime.Now;
                user.CreatedBy = System.Environment.UserName;
                user.UpdatedBy = System.Environment.UserName;

                _repository.AddUser(user);
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }

        public Task GetUserByUserNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public User GetUserByEmail(string email)
        {
            // Fetch the user by email from the repository
            return _repository.GetUsers().FirstOrDefault(user => user.Email == email);
        }

        public void UpdateUser(User user)
        {
            // Update user details in the repository
            var existingUser = _repository.GetUsers().FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Password = PasswordManager.EncryptPassword(user.Password);
                existingUser.UpdatedTime = DateTime.Now;
                existingUser.UpdatedBy = System.Environment.UserName;

                _repository.UpdateUser(existingUser);
            }
            else
            {
                throw new InvalidDataException("User not found.");
            }
        }

        public Task<User> AddUserAsync(UserViewModel model)
        {
            throw new NotImplementedException();
        }

        public object GetUserByUsername(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
