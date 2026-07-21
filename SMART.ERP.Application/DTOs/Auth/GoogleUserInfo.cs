namespace SMART.ERP.Application.DTOs.Auth;

/// <summary>
/// Datos extraidos de un id_token de Google ya validado.
/// </summary>
public class GoogleUserInfo
{
    public string Email { get; set; } = null!;

    /// <summary>
    /// True cuando Google confirma que el usuario es dueño del correo.
    /// Es la condicion que habilita crear o empatar una cuenta.
    /// </summary>
    public bool EmailVerified { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    /// <summary>Claim "sub": identificador estable de la cuenta de Google.</summary>
    public string SubjectId { get; set; } = null!;

    public string? PictureUrl { get; set; }
}
