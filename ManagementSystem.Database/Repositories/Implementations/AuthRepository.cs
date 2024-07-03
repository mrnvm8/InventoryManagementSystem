using ManagementSystem.Application.Entities;
using ManagementSystem.Application.Repositories.Interface;
using ManagementSystem.Database.Data;
using ManagementSystem.Database.Helpers;
using ManagementSystem.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManagementSystem.Database.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly JwtSection _jwtSection;
        public AuthRepository(AppDbContext context, IOptions<JwtSection> jwtOPtions)
        {
            _context = context;
            _jwtSection = jwtOPtions.Value;
        }

        #region Adding User to the system
        public async Task<AppUser?> CreateAsync(AppUser user, CancellationToken token = default)
        {
            //Add new User login record
            var newUser = await AddToDatabase(user, token);

            //Assigning the user a role
            //check if the role admin is assigned to any user
            var checkAdminRole =
                await _context.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(UserRoleConstants.Admin), token);
            if (checkAdminRole is null)
            {
                //if the it is null add one role that is called admin
                var createAdminRole = await AddToDatabase(
                    new SystemRole()
                    {
                        Id = Guid.NewGuid(),
                        Name = UserRoleConstants.Admin,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                    }
                    , token);

                //After creating the role, let asssign it to the current registering new user
                await AddToDatabase(
                    new SystemUserRole
                    {
                        Id = Guid.NewGuid(),
                        RoleId = createAdminRole.Id,
                        UserId = newUser.Id,
                    },
                    token);

                //return the add new User
                return newUser;
            }

            //check if role employee exist
            var checkUserRole =
                await _context.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(UserRoleConstants.User), token);
            //create empty system role
            SystemRole response = new() { Id = Guid.NewGuid(), Name = string.Empty };

            if (checkUserRole is null)
            {
                //add a role called User if it doesn't exist
                response = await AddToDatabase(new SystemRole()
                {
                    Id = Guid.NewGuid(),
                    Name = UserRoleConstants.User,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                }
                , token);

                //Assign the User Role to the current registering User
                await AddToDatabase(new SystemUserRole()
                {
                    Id = Guid.NewGuid(),
                    RoleId = response.Id,
                    UserId = newUser.Id
                }, token);
            }
            else
            {
                //assign the User role
                await AddToDatabase(new SystemUserRole()
                {
                    Id = Guid.NewGuid(),
                    RoleId = checkUserRole.Id,
                    UserId = newUser.Id
                }, token);
            }

            //return created new user
            return newUser;
        }

        #endregion

        #region Generate token and return it as a string
        public string GenerateToken(AppUser user, string role)
        {

            //create sign credentails
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSection.Key!));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //setup claims
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
                new Claim(ClaimTypes.Name, $"{user.Employee!.Name.ToUpper()}," +
                                           $" {user.Employee!.Surname.ToUpper()}" ),
                new Claim(ClaimTypes.Email, user.Employee!.WorkEmail.ToString()),
                new Claim(ClaimTypes.Role, role!),
                new Claim(UserRoleConstants.EmployeeIdentifier, user.EmployeeId.ToString()),
                new Claim(UserRoleConstants.DepartmentIdentifier, user.Employee.DepartmentId.ToString())
            };

            //create security token
            var securityToken = new JwtSecurityToken(
                issuer: _jwtSection.Issuer,
                audience: _jwtSection.Audience,
                expires: DateTime.UtcNow.AddHours(1),
                claims: userClaims,
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
        #endregion

        #region Save to Database
        private async Task<T> AddToDatabase<T>(T model, CancellationToken token = default)
        {
            var result = await _context.AddAsync(model!);
            await _context.SaveChangesAsync(token);
            return (T)result.Entity;
        }
        #endregion

        #region Get the user role and role by Id
        public async Task<SystemUserRole?> UserRole(Guid userId, CancellationToken token = default) =>
            await _context.SystemUserRoles
            .FirstOrDefaultAsync(r => r.UserId.Equals(userId), token);

        public async Task<SystemRole?> SystemRole(Guid roleId, CancellationToken token = default) =>
            await _context.SystemRoles.
            FirstOrDefaultAsync(r => r.Id.Equals(roleId), token);

        #endregion

        #region Get user by employee Id
        public async Task<AppUser?> GetUserByEmployeeId(Guid employeeId, CancellationToken token = default) =>
            await _context.AppUsers
            .Include(e => e.Employee)
            .FirstOrDefaultAsync(u => u.EmployeeId.Equals(employeeId), token);
        #endregion

        #region Get All System Users
        public async Task<IEnumerable<AppUser>> GetAllUsersAsync(CancellationToken token = default) =>
            await _context.AppUsers
            .Include(e => e.Employee)
                .ThenInclude(d => d!.Department)
                    .ThenInclude(o => o!.Offices)
            .ToListAsync(token);
        #endregion

        #region Get user by Id
        public async Task<AppUser?> GetUserById(Guid UserId, CancellationToken token = default) =>
             await _context.AppUsers
            .Include(e => e.Employee)
                .ThenInclude(d => d!.Department)
                    .ThenInclude(o => o!.Offices)
            .FirstOrDefaultAsync(u => u.Id.Equals(UserId), token);
        #endregion

        #region Change user Password
        public async Task<bool> ChangePassword(AppUser user, CancellationToken token = default)
        {
            //attach user entity
            _context.Attach(user);
            //updating user to the database
            _context.Entry(user).State = EntityState.Modified;
            //getting the status if a row was affected from the database 
            var affected = await _context.SaveChangesAsync();
            return affected > 0 ? true : false;
        }
        #endregion

        #region Removing user login record
        public async Task<bool> DeleteUser(AppUser user, CancellationToken token = default)
        {
            //attach user entity
            _context.Attach(user);
            //Delete user to the database
            _context.Entry(user).State = EntityState.Deleted;
            //getting the status if a row was affected from the database 
            var affected = await _context.SaveChangesAsync();
            return affected > 0 ? true : false;
        }
        #endregion

    }
}
