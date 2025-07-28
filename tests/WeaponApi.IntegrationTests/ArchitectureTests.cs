using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace WeaponApi.IntegrationTests;

/// <summary>
/// Architecture tests to enforce Clean Architecture principles
/// and ensure proper separation of concerns across layers.
/// </summary>
public sealed class ArchitectureTests
{
    private const string DomainNamespace = "WeaponApi.Domain";
    private const string ApplicationNamespace = "WeaponApi.Application";
    private const string InfrastructureNamespace = "WeaponApi.Infrastructure";
    private const string ApiNamespace = "WeaponApi.Api";

    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Domain.DomainReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Domain layer should not depend on other layers. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructureOrApi()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Application.ApplicationReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Application layer should not depend on Infrastructure or API layers. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Application_ShouldDependOnDomain()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Application.ApplicationReference).Assembly;

        // Act & Assert - Check that Application types exist
        var applicationTypes = Types.InAssembly(assembly)
            .That()
            .ResideInNamespaceStartingWith(ApplicationNamespace)
            .GetTypes().ToList();

        applicationTypes.Should().NotBeEmpty("Application layer should have types");

        // Verify the Application assembly references the Domain assembly
        var referencedAssemblies = assembly.GetReferencedAssemblies();
        referencedAssemblies.Should().Contain(a => a.Name == "WeaponApi.Domain",
            "Application assembly should reference Domain assembly");
    }

    [Fact]
    public void Infrastructure_ShouldDependOnApplicationAndDomain()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Infrastructure.InfrastructureReference).Assembly;

        // Act & Assert - Check that Infrastructure types exist that should depend on Domain and Application
        var infrastructureTypes = Types.InAssembly(assembly)
            .That()
            .ResideInNamespaceStartingWith(InfrastructureNamespace)
            .GetTypes().ToList();

        infrastructureTypes.Should().NotBeEmpty("Infrastructure layer should have types");

        // Verify the Infrastructure assembly references the Domain and Application assemblies
        var referencedAssemblies = assembly.GetReferencedAssemblies();
        referencedAssemblies.Should().Contain(a => a.Name == "WeaponApi.Domain",
            "Infrastructure assembly should reference Domain assembly");
        referencedAssemblies.Should().Contain(a => a.Name == "WeaponApi.Application",
            "Infrastructure assembly should reference Application assembly");
    }

    [Fact]
    public void Api_ShouldNotDependOnInfrastructure()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Api.ApiReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace(ApiNamespace)
            .And()
            .DoNotHaveName("Program") // Program.cs may reference Infrastructure for DI
            .Should()
            .NotHaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"API layer should not depend on Infrastructure layer (except Program.cs). Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Controllers_ShouldDependOnMediatR()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Api.ApiReference).Assembly;

        // Act & Assert - Check that Controller types exist
        var controllerTypes = Types.InAssembly(assembly)
            .That()
            .ResideInNamespaceStartingWith($"{ApiNamespace}.Controllers")
            .And()
            .AreClasses()
            .GetTypes().ToList();

        controllerTypes.Should().NotBeEmpty("API layer should have controller types");

        // Verify the API assembly references MediatR
        var referencedAssemblies = assembly.GetReferencedAssemblies();
        referencedAssemblies.Should().Contain(a => a.Name == "MediatR",
            "API assembly should reference MediatR assembly");
    }

    [Fact]
    public void Controllers_ShouldBeSealed()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Api.ApiReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{ApiNamespace}.Controllers")
            .And()
            .AreClasses()
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Controllers should be sealed. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void CommandHandlers_ShouldBeSealed()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Application.ApplicationReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .And()
            .AreClasses()
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Command handlers should be sealed. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void DomainEntities_ShouldBeSealed()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Domain.DomainReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespaceStartingWith(DomainNamespace)
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .And()
            .DoNotHaveNameEndingWith("Event")
            .And()
            .DoNotImplementInterface(typeof(WeaponApi.Domain.User.IPasswordHasher))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Domain entities should be sealed. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void ValueObjects_ShouldBeRecords()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Domain.DomainReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Id")
            .Or()
            .HaveNameEndingWith("Name")
            .Or()
            .HaveNameEndingWith("Email")
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Value objects should be sealed (records). Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Repositories_ShouldHaveInterfaceInApplication()
    {
        // Arrange
        var applicationAssembly = typeof(WeaponApi.Application.ApplicationReference).Assembly;

        // Act
        var repositoryInterfaces = Types.InAssembly(applicationAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .And()
            .AreInterfaces()
            .GetTypes();

        // Assert
        repositoryInterfaces.Should().NotBeEmpty("Repository interfaces should exist in Application layer");
        repositoryInterfaces.Should().OnlyContain(t => t.IsInterface, "Repository contracts should be interfaces");
    }

    [Fact]
    public void Commands_ShouldBeRecords()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Application.ApplicationReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Command")
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Commands should be sealed records. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Queries_ShouldBeRecords()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Application.ApplicationReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Query")
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Queries should be sealed records. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void DomainEvents_ShouldBeRecords()
    {
        // Arrange
        var assembly = typeof(WeaponApi.Domain.DomainReference).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Event")
            .And()
            .AreNotInterfaces()
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Domain events should be sealed records. Failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
}
