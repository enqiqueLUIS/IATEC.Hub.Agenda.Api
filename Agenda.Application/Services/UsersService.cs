using System.Net;
using Agenda.Application.Interfaces;
using Agenda.Core.Entities.Core;
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
                        Messages = new[] { new Message { Type = "success", Description = "Los usuarios recuperaron con éxito." } },
                        StatusCode = HttpStatusCode.OK
                    };
                }
                else
                {
                    return new ResponseGetObject
                    {
                        Data = users,
                        Messages = new[] { new Message { Type = "success", Description = "Usuarios accedieron correctamente." } },
                        StatusCode = HttpStatusCode.OK
                    };
                }
            }
            else
            {
                return new ResponseGetObject
                {
                    Data = null,
                    Messages = new[] { new Message { Type = "warning", Description = "No se encontraron usuarios." } },
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }
        catch (Exception)
        {
            return new ResponseGetObject
            {
                Data = null,
                Messages = new[] { new Message { Type = "error", Description = "Error al recuperar los usuarios." } },
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
        try
        {
            var validationResult = await _validator.ValidateAsync(queryFilter);

            if (!validationResult.IsValid)
            {
                return new ResponsePost()
                {
                    Messages = validationResult.Errors.Select(e => new Message { Type = "error", Description = e.ErrorMessage }).ToArray(),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            await _unitOfWork.UsersRepository.InsertAsync(
                new Users
                {
                    Name     = queryFilter.Name,
                    Email    = queryFilter.Email,
                    Password = queryFilter.Password
                });

            return new ResponsePost()
            {
                Messages = new[] { new Message { Type = "success", Description = "Usuario creado exitosamente." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponsePost()
            {
                Messages = new[] { new Message { Type = "error", Description = "Error al crear el usuario." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponsePost> UpdateUsers(UsersQueryFilter queryFilter)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(queryFilter);

            if (!validationResult.IsValid)
            {
                return new ResponsePost()
                {
                    Messages = validationResult.Errors
                        .Select(e => new Message { Type = "error", Description = e.ErrorMessage })
                        .ToArray(),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            if (queryFilter.Id <= 0)
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "El Id del usuario no es válido." } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            _unitOfWork.UsersRepository.UpdateCustom(
                new Users
                {
                    Id       = queryFilter.Id,
                    Name     = queryFilter.Name,
                    Email    = queryFilter.Email,
                    Password = queryFilter.Password
                },
                x => x.Name,
                x => x.Email,
                x => x.Password
            );

            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "success", Description = "Usuario actualizado exitosamente." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "error", Description = "Error al actualizar el usuario." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponsePost> DeleteUsers(UsersQueryFilter queryFilter)
    {
        throw new NotImplementedException();
    }
}