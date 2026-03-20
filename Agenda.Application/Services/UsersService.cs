using System.Net;
using Agenda.Core.Entities.Core.CustomEntities.ResponseApi.Details;
using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.Interfaces;
using Agenda.Core.QueryFilters;
using FluentValidation;

namespace Agenda.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PaginationOptions _paginationOptions;
    private readonly IUsersRepository _usersRepository;
    private readonly IValidator<UsersQueryFilter> _validator;

    public UsersService
    (
        IUnitOfWork unitOfWork,
        PaginationOptions paginationOptions,
        IUsersRepository usersRepository,
        IValidator<UsersQueryFilter> validator
    )
    {
        _unitOfWork = unitOfWork;
        _paginationOptions = paginationOptions;
        _usersRepository = usersRepository;
        _validator = validator;
    }

    public async Task<object> GetAllUsers(PaginationQueryFilter queryFilter)
    {
        try
        {
            queryFilter.PageNumber = queryFilter.PageNumber == 0 ? _paginationOptions.InitialPageNumber : queryFilter.PageNumber;
            queryFilter.PageSize = queryFilter.PageSize == 0 ? _paginationOptions.InitialPageSize : queryFilter.PageSize;

            var allUsers = await _usersRepository.GetAllUsers();
            var filtered = allUsers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryFilter.SearchCriteria))
            {
                var criteria = queryFilter.SearchCriteria.ToLower();
                filtered = filtered.Where(u =>
                    (u.Name != null && u.Name.ToLower().Contains(criteria)) ||
                    (u.Email != null && u.Email.ToLower().Contains(criteria))
                );
            }

            var totalRecords = filtered.Count();
            var users = filtered
                .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
                .Take(queryFilter.PageSize)
                .ToList();

            if (users.Any())
            {
                bool needsPagination = totalRecords > queryFilter.PageSize;

                if (needsPagination)
                {
                    return new ResponseGetObject
                    {
                        Data = new
                        {
                            Items = users,
                            CurrentPage = queryFilter.PageNumber,
                            PageSize = queryFilter.PageSize,
                            TotalRecords = totalRecords,
                            TotalPages = (int)Math.Ceiling((double)totalRecords / queryFilter.PageSize)
                        },
                        Messages = new[] { new Message { Type = "success", Description = "Users retrieved successfully." } },
                        StatusCode = HttpStatusCode.OK
                    };
                }
                else
                {
                    return new ResponseGetObject
                    {
                        Data = users,
                        Messages = new[] { new Message { Type = "success", Description = "Users retrieved successfully." } },
                        StatusCode = HttpStatusCode.OK
                    };
                }
            }
            else
            {
                return new ResponseGetObject
                {
                    Data = null,
                    Messages = new[] { new Message { Type = "warning", Description = "No users found." } },
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }
        catch (Exception)
        {
            return new ResponseGetObject
            {
                Data = null,
                Messages = new[] { new Message { Type = "error", Description = "Error retrieving users." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseGetObject> GetUserById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponsePost> InsertUsers(UsersQueryFilter queryFilter)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponsePost> UpdateUsers(UsersQueryFilter queryFilter)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponsePost> DeleteUsers(UsersQueryFilter queryFilter)
    {
        throw new NotImplementedException();
    }
}