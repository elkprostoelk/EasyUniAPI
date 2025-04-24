using EasyUniAPI.Common.Dto;
using EasyUniAPI.DataAccess;
using EasyUniAPI.DataAccess.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyUniAPI.Core.Validators
{
    public class GrantUserRolesDtoValidator : AbstractValidator<GrantUserRolesDto>
    {
        private readonly IRepository<User, string> _userRepository;
        private readonly IRepository<Role, int> _roleRepository;

        public GrantUserRolesDtoValidator(
            IRepository<User, string> userRepository,
            IRepository<Role, int> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;

            RuleFor(dto => dto.UserId)
                .NotEmpty()
                .MustAsync(UserExistsAsync)
                .WithMessage("User does not exist.");

            RuleFor(dto => dto.RoleIds)
                .NotEmpty()
                .MustAsync(AllRolesExistAsync)
                .WithMessage("You have specified not existing role(s). Please review the list.");
        }

        private async Task<bool> UserExistsAsync(
            string userId,
            CancellationToken cancellationToken) => await _userRepository.DbSet
                .AnyAsync(u => u.Id == userId, cancellationToken);

        private async Task<bool> AllRolesExistAsync(
            List<int> roleIds,
            CancellationToken cancellationToken) => await _roleRepository.DbSet
                .Where(r => roleIds.Contains(r.Id))
                .CountAsync(cancellationToken) == roleIds.Count;
    }
}
