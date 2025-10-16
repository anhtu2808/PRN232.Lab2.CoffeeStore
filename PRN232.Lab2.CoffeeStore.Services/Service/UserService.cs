using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Enums;
using PRN232.Lab2.CoffeeStore.Models.Exception;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.User;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PageResponse<UserResponse>> GetUsers(UserFilterRequest filter)
        {
            var pagedUsers = await _unitOfWork.Users.GetPagedUsersAsync(filter);
            var userResponses = _mapper.Map<List<UserResponse>>(pagedUsers.Items);

            return new PageResponse<UserResponse>(
                userResponses,
                pagedUsers.TotalCount,
                pagedUsers.Page,
                pagedUsers.PageSize
            );
        }

        public async Task<UserResponse?> GetUserById(Guid id)
        {
            var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(
                u => u.UserId == id,
                includeProperties: "RefreshTokens"
            );

            if (user is null)
            {
                throw new AppException(ErrorCode.UserNotFound);
            }

            return _mapper.Map<UserResponse>(user);
        }

        public async Task<UserResponse?> CreateUser(UserRequest request,Role role)
        {
            // Check if username already exists
            var existingUserByUsername =
                await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUserByUsername != null)
            {
                throw new AppException(ErrorCode.UsernameAlreadyExists);
            }

            // Check if email already exists
            var existingUserByEmail = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUserByEmail != null)
            {
                throw new AppException(ErrorCode.EmailAlreadyExists);
            }

            var userEntity = _mapper.Map<User>(request);
            userEntity.UserId = Guid.NewGuid();
            userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            userEntity.CreatedDate = DateTime.UtcNow;
            userEntity.Role = role.ToString();

            await _unitOfWork.Users.AddAsync(userEntity);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserResponse>(userEntity);
        }

        public async Task<UserResponse?> UpdateUser(Guid id, UserRequest request)
        {
            var existingUser = await _unitOfWork.Users.GetByIdAsync(id);
            if (existingUser is null)
            {
                throw new AppException(ErrorCode.UserNotFound);
            }

            // Check if username is being changed and if it already exists
            if (existingUser.Username != request.Username)
            {
                var userWithSameUsername =
                    await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Username == request.Username);
                if (userWithSameUsername != null)
                {
                    throw new AppException(ErrorCode.UsernameAlreadyExists);
                }
            }

            // Check if email is being changed and if it already exists
            if (existingUser.Email != request.Email)
            {
                var userWithSameEmail = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Email == request.Email);
                if (userWithSameEmail != null)
                {
                    throw new AppException(ErrorCode.EmailAlreadyExists);
                }
            }

            // Map request to existing user (excluding UserId and CreatedDate)
            existingUser.Username = request.Username;
            existingUser.Email = request.Email;

            // Only update password if provided
            if (!string.IsNullOrEmpty(request.Password))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _unitOfWork.Users.UpdateAsync(existingUser);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserResponse>(existingUser);
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var userToDelete = await _unitOfWork.Users.GetByIdAsync(id);
            if (userToDelete is null)
            {
                throw new AppException(ErrorCode.UserNotFound);
            }
            
            await _unitOfWork.Users.DeleteAsync(userToDelete);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}