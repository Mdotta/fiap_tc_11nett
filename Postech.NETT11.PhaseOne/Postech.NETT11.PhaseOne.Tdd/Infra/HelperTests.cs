using FluentAssertions;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Tests.Entities;
using Postech.NETT11.PhaseOne.Tests.Enuns;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Infra
{
    public class HelperTests
    {
        public static List<EnumCategories> CreateDefaultCategories()
        {
            return new List<EnumCategories>
            {
                EnumCategories.Action,
                EnumCategories.Adventure,
                EnumCategories.MMO,
                EnumCategories.Indie
            };
        }

        public static GameTests CreateValidGame()
        {
            return new GameTests(
                "Test Game",
                "Game description",
                "Test Developer",
                "Test Publisher",
                99.90m,
                DateTime.UtcNow.AddDays(-1),
                CreateDefaultCategories()
            );
        }

        public static User CreateAdministrator()
        {
            return new User
            {
                UserHandle = "admin123",
                Username = "admin@fiap.com.br",
                PasswordHash = "hash123456",
                Role = UserRole.Admin
            };
        }

        public static User CreateRegularUser()
        {
            return new User
            {
                UserHandle = "usuario123",
                Username = "usuario@fiap.com.br",
                PasswordHash = "hash123456",
                Role = UserRole.Client
            };
        }
    }
}
