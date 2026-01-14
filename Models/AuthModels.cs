namespace Mobile_APP_INS.Models;

// ขาเข้า (จากหน้า Login)
public class LoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// ขาออก (แจ้งผลลัพธ์กลับไปที่หน้า Login)
public class AuthResponse
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

// ตัวรับ JSON จาก Firebase (Internal use)
public class FirebaseLoginResult
{
    public string IdToken { get; set; }
    public string Email { get; set; }
    public string RefreshToken { get; set; }
    public string ExpiresIn { get; set; }
    public string LocalId { get; set; }
}