using System.ComponentModel.DataAnnotations;
using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Data;
using CineMeoTic.UserService.API.Models;

namespace CineMeoTic.UserService.API.CQRS.Commands.CreateUser;

public class CreateUserHandler(IUserDbContext dbContext)
    : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        //create User entity from command object
        //save to database
        //return result 

        var User = CreateNewUser(command.UserModel);

        dbContext.Users.Add(User);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateUserResponse(User.Id);
    }

    private User CreateNewUser(CreateUserRequest user)
    {
        var userName = user.Username;
        var password = user.Password;

        var newUser = new User(){

                // Id: Guid.NewGuid(),
                Email = userName,
                PasswordHash = password
        };
        return newUser;
    }

}