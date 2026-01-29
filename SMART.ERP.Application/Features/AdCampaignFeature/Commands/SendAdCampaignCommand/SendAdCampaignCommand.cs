using MediatR;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AdCampaignFeature.Commands.SendAdCampaignCommand
{
    public class SendAdCampaignCommand : IRequest<Response<string>>
    {
        public string? HeadingId { get; set; }
        public string? CustomerTypeId { get; set; }
        public string Message { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public List<Guid>? SelectedCustomers { get; set; }
        public List<IFormFile>? File { get; set; }
        public bool All { get; set; }
    }

    public class SendAdCampaignCommandHandler : IRequestHandler<SendAdCampaignCommand, Response<string>>
    {
        private readonly IMailService _mailService;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<Heading> _headingRepositoryAsync;
        private readonly IRepositoryAsync<CustomerType> _customerTypeRepositoryAsync;

        public SendAdCampaignCommandHandler(IMailService mailService, IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<Heading> headingRepositoryAsync, IRepositoryAsync<CustomerType> customerTypeRepositoryAsync)
        {
            _mailService = mailService;
            _customerRepositoryAsync = customerRepositoryAsync;
            _headingRepositoryAsync = headingRepositoryAsync;
            _customerTypeRepositoryAsync = customerTypeRepositoryAsync;
        }

        public async Task<Response<string>> Handle(SendAdCampaignCommand request, CancellationToken cancellationToken)
        {
            if (request.HeadingId == null && request.CustomerTypeId == null)
            {
                if (request.All)
                {
                    var customers = await _customerRepositoryAsync.ListAsync();
                   
                    foreach (var customer in customers)
                    {
                        if (customer.Email is null)
                        {
                            throw new ApiException("Un customer o mas no contienen correos asignados.");
                        }
                    }

                    foreach (var customer in customers)
                    {
                        MailRequestDto mailRequest = new MailRequestDto();
                        mailRequest.Attachment = request.File;
                        mailRequest.ToEmail = customer.Email!;
                        mailRequest.Body = request.Message;
                        mailRequest.Subject = request.Subject;
                        try
                        {
                            await _mailService.SendEmailAsync(mailRequest);
                        }
                        catch (Exception e)
                        {
                            throw new ApiException(e.Message);
                        }
                    }
                    return new Response<string>("Campaña enviada con éxito");
                }
                else
                {
                    var selectedCustomers = new List<Customer>();
                    for (int i = 0; i < request.SelectedCustomers!.Count; i++)
                    {
                        var checkIfExist = await _customerRepositoryAsync.GetByIdAsync(request.SelectedCustomers[i]);
                        if (checkIfExist == null)
                        {
                            throw new KeyNotFoundException($"No se encontro el customere con id {request.SelectedCustomers[i]}");
                        }
                        selectedCustomers.Add(checkIfExist);
                    }
                    foreach (var customer in selectedCustomers)
                    {
                        if (customer.Email is null)
                        {
                            throw new ApiException("Un customere o mas no contienen correos asignados.");
                        }
                    }
                    foreach (var customer in selectedCustomers)
                    {
                        MailRequestDto mailRequest = new MailRequestDto();
                        mailRequest.Attachment = request.File;
                        mailRequest.ToEmail = customer.Email!;
                        mailRequest.Body = request.Message;
                        mailRequest.Subject = request.Subject;
                        try
                        {
                            await _mailService.SendEmailAsync(mailRequest);
                        }
                        catch (Exception e)
                        {
                            throw new ApiException(e.Message);
                        }
                    }
                    return new Response<string>("Campaña enviada con éxito");
                }

            }
            else
            {
                if (request.All)
                {
                    var selectedCustomers = new List<Customer>();
                    if (request.CustomerTypeId != null && request.HeadingId != null)
                    {
                        var checkIfHeadingExist = await _headingRepositoryAsync.GetByIdAsync(int.Parse(request.HeadingId));
                        if (checkIfHeadingExist == null)
                        {
                            throw new KeyNotFoundException($"No existe el rubro con id {request.HeadingId}");
                        }
                        var checkIfcustomerTypeExist = await _customerTypeRepositoryAsync.GetByIdAsync(int.Parse(request.CustomerTypeId));
                        if (checkIfcustomerTypeExist == null)
                        {
                            throw new KeyNotFoundException($"No existe el tipo de customere con id {request.CustomerTypeId}");
                        }
                        //selectedCustomers = await _customerRepositoryAsync.ListAsync(new FilterCustomerAdCampaignSpecification(int.Parse(request.CustomerTypeId), int.Parse(request.HeadingId)));
                        foreach (var customer in selectedCustomers)
                        {
                            if (customer.Email is null)
                            {
                                throw new ApiException("Un customere o mas no contienen correos asignados.");
                            }
                        }
                        foreach (var customer in selectedCustomers)
                        {
                            MailRequestDto mailRequest = new MailRequestDto();
                            mailRequest.Attachment = request.File;
                            mailRequest.ToEmail = customer.Email!;
                            mailRequest.Body = request.Message;
                            mailRequest.Subject = request.Subject;
                            try
                            {
                                await _mailService.SendEmailAsync(mailRequest);
                            }
                            catch (Exception e)
                            {
                                throw new ApiException(e.Message);
                            }
                        }
                        return new Response<string>("Campaña enviada con éxito");
                    }
                    else
                    {
                        if (request.HeadingId != null)
                        {
                            //selectedCustomers = await _customerRepositoryAsync.ListAsync(new FiltercustomerAdCampaignSpecification(null, int.Parse(request.HeadingId)));
                        }
                        else
                        {
                            //selectedCustomers = await _customerRepositoryAsync.ListAsync(new FiltercustomerAdCampaignSpecification(int.Parse(request.CustomerTypeId!), null));
                        }
                        foreach (var customer in selectedCustomers)
                        {
                            if (customer.Email is null)
                            {
                                throw new ApiException("Un customere o mas no contienen correos asignados.");
                            }
                        }
                        foreach (var customer in selectedCustomers)
                        {
                            MailRequestDto mailRequest = new MailRequestDto();
                            mailRequest.Attachment = request.File;
                            mailRequest.ToEmail = customer.Email!;
                            mailRequest.Body = request.Message;
                            mailRequest.Subject = request.Subject;
                            try
                            {
                                await _mailService.SendEmailAsync(mailRequest);
                            }
                            catch (Exception e)
                            {
                                throw new ApiException(e.Message);
                            }
                        }
                        return new Response<string>("Campaña enviada con éxito");
                    }

                }
                else
                {
                    var selectedCustomers = new List<Customer>();
                    for (int i = 0; i < request.SelectedCustomers!.Count; i++)
                    {
                        var checkIfExist = await _customerRepositoryAsync.GetByIdAsync(request.SelectedCustomers[i]);
                        if (checkIfExist == null)
                        {
                            throw new KeyNotFoundException($"No se encontro el customere con id {request.SelectedCustomers[i]}");
                        }
                        selectedCustomers.Add(checkIfExist);
                    }
                    foreach (var customer in selectedCustomers)
                    {
                        if (customer.Email is null)
                        {
                            throw new ApiException("Un customere o mas no contienen correos asignados.");
                        }
                    }
                    foreach (var customer in selectedCustomers)
                    {
                        MailRequestDto mailRequest = new MailRequestDto();
                        mailRequest.Attachment = request.File;
                        mailRequest.ToEmail = customer.Email!;
                        mailRequest.Body = request.Message;
                        mailRequest.Subject = request.Subject;
                        try
                        {
                            await _mailService.SendEmailAsync(mailRequest);
                        }
                        catch (Exception e)
                        {
                            throw new ApiException(e.Message);
                        }
                    }
                    return new Response<string>("Campaña enviada con éxito");

                }
            }
        }
    }
}
