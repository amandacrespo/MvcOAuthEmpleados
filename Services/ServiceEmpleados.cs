using MvcOAuthEmpleados.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MvcOAuthEmpleados.Services
{
    public class ServiceEmpleados
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue Header;
        private IHttpContextAccessor ContextAccesor;

        public ServiceEmpleados(IConfiguration configuration, IHttpContextAccessor httpContext) {
            UrlApi = configuration.GetValue<string>("ApiUrls:ApiEmpleados") ;
            Header = new MediaTypeWithQualityHeaderValue("application/json");
            ContextAccesor = httpContext;
        }

        public async Task<string> GetTokenAsync(string usuario, int password) {
            LoginModel model = new LoginModel
            {
                Apellido = usuario,
                IdEmpleado = password
            };

            using (HttpClient client = new HttpClient()) {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode) {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(data);
                    data = jObject.GetValue("response").ToString();
                    return data;
                }
                else {
                    return "Petición incorrecta: " + response.StatusCode;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                HttpResponseMessage response = await client.GetAsync(request);

                if (response.IsSuccessStatusCode) {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.GetAsync(request);

                if (response.IsSuccessStatusCode) {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else {
                    return default(T);
                }
            }
        }

        public async Task<List<Empleado>> GetEmpleadosAsync() {
            string request = "api/empleados";
            List<Empleado> empleados = await CallApiAsync<List<Empleado>>(request);
            return empleados;
        }

        public async Task<Empleado> FindEmpleadoAsync(int id) {
            string token = this.ContextAccesor.HttpContext.User.FindFirst(x => x.Type == "Token").Value;
            string request = "api/empleados/" + id;
            Empleado empleado = await CallApiAsync<Empleado>(request, token);
            return empleado;
        }

        public async Task<List<Empleado>> GetCompisAsync() {
            string token = this.ContextAccesor.HttpContext.User.FindFirst(x => x.Type == "Token").Value;
            string request = "api/empleados/compis";
            List<Empleado> empleados = await CallApiAsync<List<Empleado>>(request, token);
            return empleados;
        }

        public async Task<Empleado> GetPerfilAsync() {
            string token = this.ContextAccesor.HttpContext.User.FindFirst(x => x.Type == "Token").Value;
            string request = "api/empleados/perfil";
            Empleado empleado = await CallApiAsync<Empleado>(request, token);
            return empleado;
        }

        public async Task<List<string>> GetOficiosAsync() {
            string request = "api/empleados/oficios";
            List<string> oficios = await CallApiAsync<List<string>>(request);
            return oficios;
        }

        public string TransformarOficios(List<string> oficios) {
            string oficiosString = "";
            foreach (string oficio in oficios) {
                oficiosString += "oficios=" + oficio + "&";
            }
            oficiosString = oficiosString.TrimEnd('&');
            return oficiosString;
        }

        public async Task<List<Empleado>> GetEmpleadosOficioAsync(List<string> oficios) {
            string request = "api/empleados/empleadosoficio?";
            request += this.TransformarOficios(oficios);
            List<Empleado> empleados = await CallApiAsync<List<Empleado>>(request);
            return empleados;
        }

        public async Task UpdateEmpleadosOficiosAsync(int incremento, List<string> oficios) {
            string request = "api/empleados/incrementarsalarios/"+incremento+"?";
            request += this.TransformarOficios(oficios);
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response = await client.PutAsync(request, null);
            }
        }
    }
}
