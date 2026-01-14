using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Mobile_APP_INS.Models;

namespace Mobile_APP_INS.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IJSRuntime _jsRuntime;

    // ⚠️ ใส่ Firebase API Key ของคุณที่นี่ หรือดึงมาจาก appsettings.json
    private readonly string _firebaseApiKey = "YOUR_FIREBASE_API_KEY_HERE";

    public AuthService(HttpClient httpClient,
                       AuthenticationStateProvider authStateProvider,
                       IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _jsRuntime = jsRuntime;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        // 1. ยิง Request ไปที่ Firebase REST API
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}";

        var response = await _httpClient.PostAsJsonAsync(url, new
        {
            email = loginRequest.UserName, // Firebase ใช้ email เป็น username
            password = loginRequest.Password,
            returnSecureToken = true
        });

        var result = new AuthResponse();

        // 2. ตรวจสอบผลลัพธ์
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<FirebaseLoginResult>();

            // 3. เก็บ Token ลง LocalStorage (เพื่อให้ User ไม่หลุดเมื่อ Refresh)
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", content.IdToken);

            // 4. แจ้งระบบ Auth ว่า Login สำเร็จแล้ว (ถ้าใช้ CustomProvider)
            ((CustomAuthProvider)_authStateProvider).NotifyUserLogin(content.IdToken);

            result.IsSuccess = true;
        }
        else
        {
            result.IsSuccess = false;
            // อ่าน Error จาก Firebase
            var errorContent = await response.Content.ReadAsStringAsync();
            result.ErrorMessage = "Invalid username or password."; // หรือแกะ errorContent มาแสดง
        }

        return result;
    }

    public async Task LogoutAsync()
    {
        // ลบ Token และแจ้งระบบว่า Logout
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        ((CustomAuthProvider)_authStateProvider).NotifyUserLogout();
    }
}