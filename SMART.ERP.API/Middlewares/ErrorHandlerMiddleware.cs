using Microsoft.EntityFrameworkCore;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SMART.ERP.API.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>()
                {
                    Succeeded = false,
                    Message = error.Message,
                };

                switch (error)
                {
                    case ApiException e:
                        // Custom application api error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case ValidationException e:
                        // Custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = e.Errors;
                        break;
                    case InvalidOperationException:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case KeyNotFoundException e:
                        // Not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case DbUpdateException e:
                        SentrySdk.CaptureException(error);
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        LogError newRecord = new();
                        newRecord.Message = e.InnerException.Message;
                        newRecord.CreationDate = DateTime.Now;
                        var pattern = "SMARTERP\\.Ecommerce\\.Application\\\\Features\\\\[A-Za-z]+\\\\Commands\\\\[A-Za-z]+\\\\[A-Za-z]+\\.cs:line [0-9]+";
                        var match = Regex.Match(e.StackTrace, pattern);
                        if (match.Success)
                        {
                            newRecord.StackTrace = match.Value;
                        }
                        if (e.InnerException.Message.Contains("REFERENCE"))
                        {
                            if (e.InnerException.Message.Contains("DELETE"))
                                responseModel.Message = "Ha ocurrido un error de referencias al tratar de eliminar este registro.";
                            else if (e.InnerException.Message.Contains("UPDATE"))
                                responseModel.Message = "Ha ocurrido un error de referencias al tratar de actualizar este registro.";
                            else if (e.InnerException.Message.Contains("CREATE"))
                                responseModel.Message = "Ha ocurrido un error de referencias al tratar de crear este registro.";
                        }
                        else
                        {
                            responseModel.Message = "Ha ocurrido un error de base de datos.";
                        }
                        break;
                    default:
                        // Unhandled error
                        SentrySdk.CaptureException(error);
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);

                await response.WriteAsync(result);
            }
        }
    }
}
