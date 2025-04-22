using Asf.Messaging.MessageBroker;
using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Skyworx.Common.Command;
using Skyworx.Common.Constants;
using Skyworx.Common.Dto;
using Skyworx.Common.Exception;
using Skyworx.Repository.DataContext;
using Skyworx.Repository.Entity;
using Skyworx.Service.Impl;

namespace Skyworx.Tests;

public class KreditServiceTests
{
    private readonly DataContext _dataContext;
    private readonly LocalContext _localContext;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IBus> _busMock;
    private readonly KreditService _service;

    public KreditServiceTests()
    {
        var dataContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"DataContext_{Guid.NewGuid()}")
            .Options;

        var localContextOptions = new DbContextOptionsBuilder<LocalContext>()
            .UseInMemoryDatabase(databaseName: $"LocalContext_{Guid.NewGuid()}")
            .Options;

        _dataContext = new DataContext(dataContextOptions);
        _localContext = new LocalContext(localContextOptions);
        _mapperMock = new Mock<IMapper>();
        _busMock = new Mock<IBus>();

        _service = new KreditService(_dataContext, _localContext, _mapperMock.Object, _busMock.Object);
    }

    [Fact(DisplayName = "CreateAsync - Berhasil Menyimpan Kredit dan Publish ke Broker")]
    public async Task CreateAsync_ShouldReturnSuccessMessage_WhenInputIsValid()
    {
        var command = new CreatePengajuanKreditCommand
        {
            Plafon = 100_000_000,
            Bunga = 10,
            Tenor = 12
        };

        var kredit = new Repository.Entity.PengajuanKredit
        {
            Id = Guid.NewGuid(),
            Plafon = command.Plafon,
            Bunga = command.Bunga,
            Tenor = command.Tenor,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.MaxValue,
            Angsuran = 8796000
        };

        _mapperMock.Setup(m => m.Map<Repository.Entity.PengajuanKredit>(command)).Returns(kredit);

        var result = await _service.CreateAsync(command);

        Assert.NotNull(result);
        Assert.Equal(ResponseConstant.SubmitSuccess, result.Message);
        Assert.NotNull(await _localContext.PengajuanKredits.FirstOrDefaultAsync(k => k.Id == kredit.Id));
        _busMock.Verify(b => b.Publish(It.IsAny<KreditPengajuanCreatedEvent>(), default), Times.Once);
    }

    [Fact(DisplayName = "CreateAsync - Gagal Jika Plafon Tidak Valid")]
    public async Task CreateAsync_ShouldThrowBadRequest_WhenPlafonIsZero()
    {
        var command = new CreatePengajuanKreditCommand
        {
            Plafon = 0,
            Bunga = 10,
            Tenor = 12
        };

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateAsync(command));
        Assert.Equal(ResponseConstant.InvalidInput, exception.Message);
    }

    [Fact(DisplayName = "CreateAsync - Gagal Jika Bunga Melebihi 100")]
    public async Task CreateAsync_ShouldThrowBadRequest_WhenBungaIsTooHigh()
    {
        var command = new CreatePengajuanKreditCommand
        {
            Plafon = 100_000_000,
            Bunga = 150,
            Tenor = 12
        };

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateAsync(command));
        Assert.Equal(ResponseConstant.InvalidInput, exception.Message);
    }

    [Fact(DisplayName = "UpdateAsync - Berhasil Update Data Kredit")]
    public async Task UpdateAsync_ShouldUpdateEntity_WhenIdExists()
    {
        var existing = new Repository.Entity.PengajuanKredit
        {
            Id = Guid.NewGuid(),
            Plafon = 50_000_000,
            Bunga = 8,
            Tenor = 6,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dataContext.PengajuanKredits.AddAsync(existing);
        await _dataContext.SaveChangesAsync();

        var updateCommand = new CreatePengajuanKreditCommand
        {
            Plafon = 70_000_000,
            Bunga = 9,
            Tenor = 10
        };

        _mapperMock.Setup(m => m.Map(updateCommand, It.IsAny<Repository.Entity.PengajuanKredit>()))
            .Callback<CreatePengajuanKreditCommand, Repository.Entity.PengajuanKredit>((cmd, entity) =>
            {
                entity.Plafon = cmd.Plafon;
                entity.Bunga = cmd.Bunga;
                entity.Tenor = cmd.Tenor;
            });

        var result = await _service.UpdateAsync(existing.Id, updateCommand);

        Assert.NotNull(result);
        Assert.Equal(ResponseConstant.UpdateSuccess, result.Message);

        var updated = await _dataContext.PengajuanKredits.FindAsync(existing.Id);
        Assert.Equal(70_000_000, updated.Plafon);
        Assert.Equal(9, updated.Bunga);
        Assert.Equal(10, updated.Tenor);
    }

    [Fact(DisplayName = "UpdateAsync - Gagal Update Jika ID Tidak Ditemukan")]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        var invalidId = Guid.NewGuid();
        var command = new CreatePengajuanKreditCommand
        {
            Plafon = 60_000_000,
            Bunga = 7,
            Tenor = 24
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(invalidId, command));
        Assert.Equal(ResponseConstant.MesNotFound, exception.Message);
    }

    [Fact(DisplayName = "DeleteAsync - Berhasil Menghapus Data Kredit")]
    public async Task DeleteAsync_ShouldRemoveEntity_WhenIdExists()
    {
        var kredit = new Repository.Entity.PengajuanKredit
        {
            Id = Guid.NewGuid(),
            Plafon = 80_000_000,
            Bunga = 9,
            Tenor = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dataContext.PengajuanKredits.AddAsync(kredit);
        await _dataContext.SaveChangesAsync();

        var result = await _service.DeleteAsync(kredit.Id);

        Assert.NotNull(result);
        Assert.Equal(ResponseConstant.DeleteSuccess, result.Message);
        Assert.Null(await _dataContext.PengajuanKredits.FindAsync(kredit.Id));
    }

    [Fact(DisplayName = "DeleteAsync - Gagal Jika ID Tidak Ditemukan")]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenIdNotExists()
    {
        var id = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(id));
        Assert.Equal(ResponseConstant.MesNotFound, exception.Message);
    }

    [Fact(DisplayName = "HitungAngsuranAsync - Sukses Hitung Angsuran Valid")]
    public async Task HitungAngsuranAsync_ShouldReturnCorrectValue_WhenValidInput()
    {
        var command = new CalculateAngsuranCommand
        {
            Plafon = 100_000_000,
            Bunga = 12,
            Tenor = 12
        };

        var bungaBulanan = command.Bunga / 12 / 100;
        var expectedAngsuran = Math.Round(
            (command.Plafon * bungaBulanan) /
            (decimal)(1 - Math.Pow(1 + (double)bungaBulanan, -command.Tenor)), 2);

        var result = await _service.HitungAngsuranAsync(command);

        Assert.NotNull(result);
        Assert.Equal("Perhitungan angsuran berhasil", result.Message);
        Assert.Single(result.Data);
        Assert.Equal(expectedAngsuran, result.Data.First().Angsuran);
    }

    [Fact(DisplayName = "HitungAngsuranAsync - Gagal Jika Plafon Nol")]
    public async Task HitungAngsuranAsync_ShouldThrowBadRequest_WhenPlafonIsZero()
    {
        var command = new CalculateAngsuranCommand
        {
            Plafon = 0,
            Bunga = 12,
            Tenor = 12
        };

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.HitungAngsuranAsync(command));
        Assert.Equal(ResponseConstant.InvalidInput, exception.Message);
    }

    [Fact(DisplayName = "HitungAngsuranAsync - Gagal Jika Bunga Melebihi 100")]
    public async Task HitungAngsuranAsync_ShouldThrowBadRequest_WhenBungaTooHigh()
    {
        var command = new CalculateAngsuranCommand
        {
            Plafon = 100_000_000,
            Bunga = 150,
            Tenor = 12
        };

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.HitungAngsuranAsync(command));
        Assert.Equal(ResponseConstant.InvalidInput, exception.Message);
    }

    
    [Fact(DisplayName = "GetAllAsync - Berhasil Mengembalikan Data Kredit")]
    public async Task GetAllAsync_ShouldReturnList_WhenDataExists()
    {
        // Arrange
        var kredit = new Repository.Entity.PengajuanKredit
        {
            Id = Guid.NewGuid(),
            Plafon = 100_000_000,
            Bunga = 10,
            Tenor = 12,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Angsuran = 8796000
        };

        _dataContext.PengajuanKredits.Add(kredit);
        await _dataContext.SaveChangesAsync();

        var kreditDto = new PengajuanKreditDto
        {
            Id = kredit.Id,
            Plafon = kredit.Plafon,
            Bunga = kredit.Bunga,
            Tenor = kredit.Tenor,
            Angsuran = kredit.Angsuran
        };

        // Use lambda so it's unambiguous
        _mapperMock
            .Setup(m => m.Map<List<PengajuanKreditDto>>(It.Is<List<Repository.Entity.PengajuanKredit>>(list => list.Any(k => k.Id == kredit.Id))))
            .Returns(new List<PengajuanKreditDto> { kreditDto });

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ResponseConstant.GetDataSuccess, result.Message);
        Assert.Single(result.Data);
        Assert.Equal(kreditDto.Id, result.Data.First().Id);
    }



}
