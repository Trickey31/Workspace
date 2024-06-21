using FluentAssertions;
using NetArchTest.Rules;
using Workspace.Application;
using Workspace.Contract;

namespace Workspace.Architecture.Tests
{
    public class ArchitectureTests
    {
        private const string DomainNamespace = "Workspace.Domain";
        private const string ApplicationNamespace = "Workspace.Application";
        private const string InfrastructureNamespace = "Workspace.Infrastructure";
        private const string PersistenceNamespace = "Workspace.Persistence";
        private const string ApiNamespace = "Workspace.API";
        private const string PresentationNamespace = "Workspace.Presentation";

        [Fact]
        public void Domain_Should_Not_HaveDependencyOnOtherProject()
        {
            // Arrage
            var assembly = Domain.AssemblyReference.Assembly;

            var otherProjects = new[]
            {
                ApplicationNamespace,
                InfrastructureNamespace,
                PersistenceNamespace,
                ApiNamespace,
                PresentationNamespace
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_HaveDependencyOnOtherProject()
        {
            // Arrage
            var assembly = Application.AssemblyReference.Assembly;

            var otherProjects = new[]
            {
                InfrastructureNamespace,
                PersistenceNamespace,
                PresentationNamespace,
                ApiNamespace
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_Not_HaveDependencyOnOtherProject()
        {
            // Arrage
            var assembly = Infrastructure.AssemblyReference.Assembly;

            var otherProjects = new[]
            {
                ApiNamespace,
                PresentationNamespace
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Presentation_Should_Not_HaveDependencyOnOtherProject()
        {
            // Arrage
            var assembly = Presentation.AssemblyReference.Assembly;

            var otherProjects = new[]
            {
                ApiNamespace,
                PersistenceNamespace,
                DomainNamespace
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Persistence_Should_Not_HaveDependencyOnOtherProject()
        {
            // Arrage
            var assembly = Persistence.AssemblyReference.Assembly;

            var otherProjects = new[]
            {
                ApplicationNamespace,
                InfrastructureNamespace,
                ApiNamespace,
                PresentationNamespace
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        #region =============== Command ===============

        [Fact]
        public void Command_Should_Have_NamingConventionEndingCommand()
        {
            // Arrage
            var assembly = Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(ICommand))
                .Should().HaveNameEndingWith("Command")
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void CommandT_Should_Have_NamingConventionEndingCommand()
        {
            // Arrage
            var assembly = Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(ICommand<>))
                .Should().HaveNameEndingWith("Command")
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void CommandHandlers_Should_Have_NamingConventionEndingCommandHandler()
        {
            // Arrange
            var assembly = Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types.InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(ICommandHandler<>))
                .Should()
                .HaveNameEndingWith("CommandHandler")
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void CommandHandlers_Should_Have_BeSealed()
        {
            // Arrange
            var assembly = Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types.InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(ICommandHandler<>))
                .Should()
                .BeSealed()
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void CommandHandlersT_Should_Have_NamingConventionEndingCommandHandler()
        {
            // Arrange
            var assembly = Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types.InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(ICommandHandler<,>))
                .Should()
                .HaveNameEndingWith("CommandHandler")
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void CommandHandlersT_Should_Have_BeSealed()
        {
            // Arrange
            var assembly = Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types.InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(ICommandHandler<,>))
                .Should()
                .BeSealed()
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        #endregion End Command

        #region =============== Query ===============

        [Fact]
        public void Query_Should_Have_NamingConventionEndingQuery()
        {
            // Arrage
            var assembly = Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(IQuery<>))
                .Should().HaveNameEndingWith("Query")
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void QueryHandlers_Should_Have_NamingConventionEndingQueryHandler()
        {
            // Arrange
            var assembly = Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types.InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(IQueryHandler<,>))
                .Should()
                .HaveNameEndingWith("QueryHandler")
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void QueryHandlers_Should_Have_BeSealed()
        {
            // Arrange
            var assembly = Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types.InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(IQueryHandler<,>))
                .Should()
                .BeSealed()
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        #endregion End Query
    }
}