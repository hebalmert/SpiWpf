using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiWpf.Data;
using SpiWpf.Entities.DTOs;
using SpiWpf.Entities.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Claims;
using System.Windows;

namespace SpiWpf.Wpf.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private string? _username;

        public string? Username
        {
            get => _username;
            set { SetProperty(ref _username!, value); }
        }

        private SecureString? _password;

        public SecureString? Password
        {
            get => _password;
            set { SetProperty(ref _password!, value); }
        }

        private bool? _IsViewVisible = true;

        public bool? IsViewVisible
        {
            get => _IsViewVisible;
            set { SetProperty(ref _IsViewVisible!, value); }
        }

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set { SetProperty(ref _errorMessage!, value); }
        }

        private static int t;

        public LoginViewModel()
        {
            t = 0;
        }

        [RelayCommand]
        public async Task btnLogin()
        {
            if (string.IsNullOrEmpty(Username) || Password == null)
            {
                ErrorMessage = "Usuario y Clave son Obligatorios";
                return;
            }
            var usuario = Username;
            string clave = ConvertToUnsecureString(Password!);
            LoginCls modelo = new()
            {
                Username = usuario!,
                Password = clave
            };
            var responseHttp = await Repository.Post<LoginCls, TokenDTO>("/api/accounts/Login", modelo);
            if (responseHttp.Error)
            {
                var msgerror = await responseHttp.GetErrorMessageAsync();
                t += 1;
                if (t >= 3)
                {
                    ErrorMessage = "Ha llegado al numero maximo de intentos";
                    Application.Current.Shutdown();
                }
                else
                {
                    ErrorMessage = "Usuario o Clave Incorrecto";
                    return;
                }
            }

            TokenDTO _token = responseHttp.Response;
            //lectura de los claims que vienen en el token
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(_token.Token) as JwtSecurityToken;
            IEnumerable<Claim> claims = jsonToken!.Claims;
            Preferences.Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            Preferences.UserType = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            Preferences.CorpName = claims.FirstOrDefault(x => x.Type == "CorpName")?.Value;
            Preferences.LogoCorp = claims.FirstOrDefault(x => x.Type == "LogoCorp")?.Value;
            Preferences.FirstName = claims.FirstOrDefault(x => x.Type == "FirstName")?.Value;
            Preferences.LastName = claims.FirstOrDefault(x => x.Type == "LastName")?.Value;
            Preferences.Photo = claims.FirstOrDefault(x => x.Type == "Photo")?.Value;
            Preferences.Token = _token.Token;
            Preferences.DateToken = _token.Expiration.ToString("yyyy-MM-dd HH:mm:ss");
            IsViewVisible = false;

            return;

        }


        public static string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                // Convierte el SecureString a un BSTR (Binary String)
                unmanagedString = Marshal.SecureStringToBSTR(secureString);
                // Convierte el BSTR a un string .NET normal
                return Marshal.PtrToStringBSTR(unmanagedString);
            }
            finally
            {
                // Asegúrate de liberar la memoria no administrada.
                Marshal.ZeroFreeBSTR(unmanagedString);
            }
        }
    }
}
