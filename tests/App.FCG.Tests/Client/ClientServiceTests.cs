using Repo = FCG.Clients.Data.Repository;
using FCG.Clients.Models;
using ClientEntity = FCG.Clients.Models.Client;
using FCG.Clients.Services;             // TODO
using FCG.Core.Data;                    // IUnitOfWork
using FCG.Shared.Dtos;
using FluentAssertions;
using FluentValidation.Results;
using Moq;

namespace FCG.Tests.Client;

public class ClientService_Tests
{
    [Fact(DisplayName = "ClientService.Insert: should insert a new client and commit")]
    public async Task Insert_Should_Insert_And_Commit()
    {
        // Arrange
        var repo = new Mock<Repo.IClientRepository>();
        var uow = new Mock<IUnitOfWork>();

        repo.SetupGet(r => r.UnitOfWork).Returns(uow.Object);
        repo.Setup(r => r.GetByCpf("98765432100")).ReturnsAsync((ClientEntity?)null);
        uow.Setup(u => u.Commit()).ReturnsAsync(true);

        var service = new ClientService(repo.Object);

        var dto = new ClienteRegistro(Guid.NewGuid(), new UsuarioRegistro
        {
            Nome = "Alice",
            Cpf = "98765432100",
            Email = "alice@mail.com",
            Senha = "Abc!12345",
            SenhaConfirmacao = "Abc!12345"
        });

        // Act
        var result = await service.Insert(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        repo.Verify(r => r.Insert(It.Is<ClientEntity>(c => c.Nome == "Alice" && c.Cpf.Numero == "98765432100")), Times.Once);
        uow.Verify(u => u.Commit(), Times.Once);
    }

    [Fact(DisplayName = "ClientService.Insert: should not insert nor commit when CPF already exists")]
    public async Task Insert_Should_Fail_When_Cpf_Exists()
    {
        var repo = new Mock<Repo.IClientRepository>();
        var uow = new Mock<IUnitOfWork>();

        repo.SetupGet(r => r.UnitOfWork).Returns(uow.Object);
        repo.Setup(r => r.GetByCpf("98765432100"))
            .ReturnsAsync(new ClientEntity(Guid.NewGuid(), "João", "joao@mail.com", "98765432100"));

        var service = new ClientService(repo.Object);

        var dto = new ClienteRegistro(Guid.NewGuid(), new UsuarioRegistro
        {
            Nome = "Alice",
            Cpf = "98765432100",
            Email = "alice@mail.com",
            Senha = "Abc!12345",
            SenhaConfirmacao = "Abc!12345"
        });

        var result = await service.Insert(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Já existe um cliente com este CPF");
        repo.Verify(r => r.Insert(It.IsAny<ClientEntity>()), Times.Never);
        uow.Verify(u => u.Commit(), Times.Never);
    }
}
