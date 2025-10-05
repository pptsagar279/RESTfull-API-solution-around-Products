using ProductAPI.Application.DTOs;

namespace ProductAPI.Application.Interfaces;

/// <summary>
/// Item service interface
/// </summary>
public interface IItemService
{
    Task<PagedResponseDto<ItemDto>> GetItemsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<ItemDto?> GetItemByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemDto>> GetItemsByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<ItemDto> CreateItemAsync(CreateItemDto createItemDto, CancellationToken cancellationToken = default);
    Task<ItemDto?> UpdateItemAsync(int id, UpdateItemDto updateItemDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteItemAsync(int id, CancellationToken cancellationToken = default);
}
