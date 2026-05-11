using FinBookeAPI.Models.Database.Authentication;

namespace FinBookeAPI.DTO.Authentication.Output;

/// <summary>
/// This class models the transfer object of a user account including
/// all necessary data which must be stored on the client side.
/// </summary>
public class UserDTO(User user)
{
    public string Name { get; set; } = user.Name;

    public string Email { get; set; } = user.Email;

    public string ImagePath { get; set; } = user.ImagePath;

    public SessionDTO Session { get; set; } = new SessionDTO(user);
}
