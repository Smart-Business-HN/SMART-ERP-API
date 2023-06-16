using MediatR;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Mail;

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
        private readonly IRepositoryHNAsync<Client> _clientRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryHNAsync<ClientHeading> _headingRepositoryAsync;
        private readonly IRepositoryHNAsync<ClientType> _clientTypeRepositoryAsync;

        public SendAdCampaignCommandHandler(IMailService mailService, IRepositoryHNAsync<Client> clientRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryHNAsync<ClientHeading> headingRepositoryAsync, IRepositoryHNAsync<ClientType> clientTypeRepositoryAsync)
        {
            _mailService = mailService;
            _clientRepositoryAsync = clientRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _headingRepositoryAsync = headingRepositoryAsync;
            _clientTypeRepositoryAsync = clientTypeRepositoryAsync;
        }

        public async Task<Response<string>> Handle(SendAdCampaignCommand request, CancellationToken cancellationToken)
        {
            if (request.HeadingId == null && request.CustomerTypeId == null)
            {
                if (request.All)
                {
                    var hnCustomers = await _clientRepositoryAsync.ListAsync();
                    var customers = await _customerRepositoryAsync.ListAsync();
                    var selectedCustomers = new List<Client>();

                    selectedCustomers = hnCustomers.FindAll(x => customers.Exists(y => y.MasterId == x.Id));
                    foreach (var customer in selectedCustomers)
                    {
                        if (customer.Email is null)
                        {
                            throw new ApiException("Un cliente o mas no contienen correos asignados.");
                        }
                    }

                    foreach (var client in selectedCustomers)
                    {
                        MailRequestDto mailRequest = new MailRequestDto();
                        mailRequest.Attachment = request.File;
                        mailRequest.ToEmail = client.Email!;
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
                    var hnCustomers = await _clientRepositoryAsync.ListAsync();
                    var customers = await _customerRepositoryAsync.ListAsync();
                    var selectedCustomers = new List<Client>();
                    for (int i = 0; i < request.SelectedCustomers!.Count; i++)
                    {
                        var checkIfExist = await _clientRepositoryAsync.GetByIdAsync(request.SelectedCustomers[i]);
                        if (checkIfExist == null)
                        {
                            throw new KeyNotFoundException($"No se encontro el cliente con id {request.SelectedCustomers[i]}");
                        }
                        selectedCustomers.Add(checkIfExist);
                    }
                    foreach (var customer in selectedCustomers)
                    {
                        if (customer.Email is null)
                        {
                            throw new ApiException("Un cliente o mas no contienen correos asignados.");
                        }
                    }
                    foreach (var client in selectedCustomers)
                    {
                        MailRequestDto mailRequest = new MailRequestDto();
                        mailRequest.Attachment = request.File;
                        mailRequest.ToEmail = client.Email!;
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
                    var selectedCustomers = new List<Client>();
                    if (request.CustomerTypeId != null && request.HeadingId != null)
                    {
                        var checkIfHeadingExist = await _headingRepositoryAsync.GetByIdAsync(int.Parse(request.HeadingId));
                        if (checkIfHeadingExist == null)
                        {
                            throw new KeyNotFoundException($"No existe el rubro con id {request.HeadingId}");
                        }
                        var checkIfClientTypeExist = await _clientTypeRepositoryAsync.GetByIdAsync(int.Parse(request.CustomerTypeId));
                        if (checkIfClientTypeExist == null)
                        {
                            throw new KeyNotFoundException($"No existe el tipo de cliente con id {request.CustomerTypeId}");
                        }
                        selectedCustomers = await _clientRepositoryAsync.ListAsync(new FilterClientAdCampaignSpecification(int.Parse(request.CustomerTypeId), int.Parse(request.HeadingId)));
                        foreach (var customer in selectedCustomers)
                        {
                            if (customer.Email is null)
                            {
                                throw new ApiException("Un cliente o mas no contienen correos asignados.");
                            }
                        }
                        foreach (var client in selectedCustomers)
                        {
                            MailRequestDto mailRequest = new MailRequestDto();
                            mailRequest.Attachment = request.File;
                            mailRequest.ToEmail = client.Email!;
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
                            selectedCustomers = await _clientRepositoryAsync.ListAsync(new FilterClientAdCampaignSpecification(null, int.Parse(request.HeadingId)));
                        }
                        else
                        {
                            selectedCustomers = await _clientRepositoryAsync.ListAsync(new FilterClientAdCampaignSpecification(int.Parse(request.CustomerTypeId!), null));
                        }
                        foreach (var customer in selectedCustomers)
                        {
                            if (customer.Email is null)
                            {
                                throw new ApiException("Un cliente o mas no contienen correos asignados.");
                            }
                        }
                        foreach (var client in selectedCustomers)
                        {
                            MailRequestDto mailRequest = new MailRequestDto();
                            mailRequest.Attachment = request.File;
                            mailRequest.ToEmail = client.Email!;
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
                    var selectedCustomers = new List<Client>();
                    for (int i = 0; i < request.SelectedCustomers!.Count; i++)
                    {
                        var checkIfExist = await _clientRepositoryAsync.GetByIdAsync(request.SelectedCustomers[i]);
                        if (checkIfExist == null)
                        {
                            throw new KeyNotFoundException($"No se encontro el cliente con id {request.SelectedCustomers[i]}");
                        }
                        selectedCustomers.Add(checkIfExist);
                    }
                    foreach (var customer in selectedCustomers)
                    {
                        if (customer.Email is null)
                        {
                            throw new ApiException("Un cliente o mas no contienen correos asignados.");
                        }
                    }
                    foreach (var client in selectedCustomers)
                    {
                        MailRequestDto mailRequest = new MailRequestDto();
                        mailRequest.Attachment = request.File;
                        mailRequest.ToEmail = client.Email!;
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
