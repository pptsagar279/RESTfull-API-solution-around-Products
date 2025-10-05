using AutoMapper;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using ProductAPI.Domain.Entities;
using ProductAPI.Domain.Exceptions;

namespace ProductAPI.Application.Services;

/// <summary>
/// Item service implementation
/// </summary>
public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAppLogger<ItemService> _logger;

    public ItemService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger<ItemService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResponseDto<ItemDto>> GetItemsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var totalRecords = await _unitOfWork.Items.CountAsync(cancellationToken: cancellationToken);
        var items = await _unitOfWork.Items.GetPagedAsync(page, pageSize, cancellationToken);
        var itemDtos = _mapper.Map<IEnumerable<ItemDto>>(items);

        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        return new PagedResponseDto<ItemDto>
        {
            Data = itemDtos,
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<ItemDto?> GetItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id, cancellationToken);
        return item == null ? null : _mapper.Map<ItemDto>(item);
    }


    public async Task<IEnumerable<ItemDto>> GetItemsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Items.GetItemsByProductIdAsync(productId, cancellationToken);
        return _mapper.Map<IEnumerable<ItemDto>>(items);
    }

    public async Task<ItemDto> CreateItemAsync(CreateItemDto createItemDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new item for product ID: {ProductId}", createItemDto.ProductId);
        
        // Verify that the product exists
        var productExists = await _unitOfWork.Products.AnyAsync(p => p.ProductId == createItemDto.ProductId, cancellationToken);
        if (!productExists)
        {
            _logger.LogWarning("Product not found with ID: {ProductId} while creating item", createItemDto.ProductId);
            throw new ProductNotFoundException(createItemDto.ProductId);
        }

        var item = _mapper.Map<Item>(createItemDto);
        await _unitOfWork.Items.AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Item created successfully with ID: {ItemId}", item.ItemId);

        return _mapper.Map<ItemDto>(item);
    }

    public async Task<ItemDto?> UpdateItemAsync(int id, UpdateItemDto updateItemDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating item with ID: {ItemId}", id);
        
        var existingItem = await _unitOfWork.Items.GetByIdAsync(id, cancellationToken);
        if (existingItem == null)
        {
            _logger.LogWarning("Item not found with ID: {ItemId}", id);
            throw new ItemNotFoundException(id);
        }

        _mapper.Map(updateItemDto, existingItem);
        _unitOfWork.Items.Update(existingItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Item updated successfully with ID: {ItemId}", id);

        return _mapper.Map<ItemDto>(existingItem);
    }

    public async Task<bool> DeleteItemAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting item with ID: {ItemId}", id);
        
        var item = await _unitOfWork.Items.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            _logger.LogWarning("Item not found with ID: {ItemId} for deletion", id);
            return false;
        }

        _unitOfWork.Items.Remove(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Item deleted successfully with ID: {ItemId}", id);
        return true;
    }
}
