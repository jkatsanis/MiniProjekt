using Wishlist.Core.Services;
using Wishlist.Persistence.Repositories;
using Wishlist.Persistence.Util;
using NSubstitute;

namespace Wishlist.Test;

public sealed class RocketServiceTests
{
    private readonly IUnitOfWork _mockUnit;
    private readonly RocketService _sut;

    public RocketServiceTests()
    {
        _mockUnit = Substitute.For<IUnitOfWork>();
        _sut = new RocketService(_mockUnit);
    }
    
    [Theory]
    [InlineData(1, true)]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public async Task GetRocketByIdAsync_CallsRepoCorrectly(int id, bool tracking)
    {
        var repoMock = Substitute.For<IRocketRepository>();
        _mockUnit.RocketRepository.Returns(repoMock);
        
        await _sut.GetRocketByIdAsync(id, tracking);
        
        await repoMock.Received(1).GetRocketByIdAsync(id, tracking);
    }
}
