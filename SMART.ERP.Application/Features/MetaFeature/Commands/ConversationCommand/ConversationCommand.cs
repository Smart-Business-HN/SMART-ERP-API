using MediatR;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveButtonReply;
using SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveListReply;
using SMART.ERP.Application.DTOs.Meta.MetaTextReply;
using SMART.ERP.Application.DTOs.Meta.Payload;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AssignUserToOpportunityService;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.MetaPostService;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MetaFeature.Commands.ConversationCommand
{
    public class ConversationCommand : IRequest<Response<string>>
    {
        public string Object { get; set; } = null!;
        public List<MetaEntry> Entry { get; set; } = null!;
    }

    public class ConversationCommandHandler : IRequestHandler<ConversationCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;
        private readonly IMetaPostService _metaPostService;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IMailService _mailService;
        private readonly IRepositoryAsync<MetaConversations> _metaRepositoryAsync;
        private readonly IAssignUserToOpportunityService _assignUserToOpportunityService;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchRepositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositryAsync;

        public ConversationCommandHandler(IRepositoryAsync<Category> categoryRepositoryAsync, IMetaPostService metaPostService,
            IRepositoryAsync<Product> productRepositoryAsync, IMailService mailService, IRepositoryAsync<MetaConversations> metaRepositoryAsync,
            IAssignUserToOpportunityService assignUserToOpportunityService, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<BranchOffices> branchRepositoryAsync,
            IRepositoryAsync<Department> departmentRepositryAsync)
        {
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _metaPostService = metaPostService;
            _productRepositoryAsync = productRepositoryAsync;
            _mailService = mailService;
            _metaRepositoryAsync = metaRepositoryAsync;
            _assignUserToOpportunityService = assignUserToOpportunityService;
            _userRepositoryAsync = userRepositoryAsync;
            _branchRepositoryAsync = branchRepositoryAsync;
            _departmentRepositryAsync = departmentRepositryAsync;
        }

        public async Task<Response<string>> Handle(ConversationCommand request, CancellationToken cancellationToken)
        {
            if (request.Object != "whatsapp_business_account")
            {
                throw new ApiException();
            }
            var changes = request.Entry[0].changes;
            var values = changes[0].value;

            if (values.statuses is not null)
            {
                if (values.statuses[0].Conversation is not null)
                {
                    var checkConversation = await _metaRepositoryAsync.GetByIdAsync(values.statuses[0].Recipient_id);
                    if (values.statuses[0].Status == "sent" && checkConversation != null && checkConversation.Expiration != values.statuses[0].Conversation!.Expiration_timestamp)
                    {
                        checkConversation.Expiration = values.statuses[0].Conversation!.Expiration_timestamp;
                        await _metaRepositoryAsync.UpdateAsync(checkConversation);
                        await _metaRepositoryAsync.SaveChangesAsync();
                    }

                }
                return new Response<string>("Ok");
            }

            if (values.messages is not null)
            {
                var messages = values.messages[0];
                var from = messages.From;

                if (messages.Interactive is not null)
                {
                    var checkConversation = await _metaRepositoryAsync.GetByIdAsync(from);
                    if (checkConversation == null)
                    {
                        var newRecord = new MetaConversations();
                        newRecord.Phone = from;
                        int expiration = 0;
                        newRecord.Expiration = expiration;
                        newRecord.ProductId = 0;
                        newRecord.ExpectingInfo = true;

                        await _metaRepositoryAsync.AddAsync(newRecord);
                        await _metaRepositoryAsync.SaveChangesAsync();
                    }
                    else
                    {
                        if (checkConversation.Expiration < messages.Timestamp)
                        {
                            checkConversation.Expiration = 0;
                            checkConversation.ExpectingInfo = false;
                            checkConversation.ProductId = 0;

                            await _metaRepositoryAsync.UpdateAsync(checkConversation);
                            await _metaRepositoryAsync.SaveChangesAsync();

                            bool success = await WelcomeMessage(from);
                            if (success)
                            {
                                return new Response<string>("ACK");
                            }
                            else
                            {
                                return new Response<string>("ACK");
                            }
                        }
                    }

                    if (messages.Interactive.button_reply is not null)
                    {
                        if (messages.Interactive!.button_reply.id!.Contains("Advisor_Product_"))
                        {
                            var splittedString = messages.Interactive.button_reply.id.Split("_");
                            int.TryParse(splittedString[2], out int productId);

                            var textResponse = new MetaResponse();
                            textResponse.to = from;
                            textResponse.text.body = @"Ayuda a nuestro asesor a comunicarse contigo. 😁
Envianos los siguientes datos con saltos de línea entre cada dato en tu siguiente mensaje:
🔵 Nombre Completo
🔵 Correo electronico 

Utiliza mi ejemplo como guía 👀:

Guido Platino Motors
guido@platino.hn";
                            checkConversation!.ProductId = productId;
                            checkConversation.ExpectingInfo = true;

                            await _metaRepositoryAsync.UpdateAsync(checkConversation);
                            await _metaRepositoryAsync.SaveChangesAsync();

                            bool succeeded = await _metaPostService.SendText(textResponse);
                            if (succeeded)
                            {
                                return new Response<string>("ACK");
                            }
                            else
                            {
                                return new Response<string>("ACK");
                            }
                        }
                        else if (messages.Interactive.button_reply.id.Contains("Visit_website_"))
                        {
                            var splittedString = messages.Interactive.button_reply.id.Split("_");
                            var productId = splittedString[2];
                            bool success = await VisitProductWebsiteMessage(productId, from);
                            if (success)
                            {
                                return new Response<string>("ACK");
                            }
                            else
                            {
                                return new Response<string>("ACK");
                            }
                        }
                        else
                        {
                            return new Response<string>("ACK");
                        }
                    }
                    else if (messages.Interactive.list_reply is not null)
                    {
                        var listId = messages.Interactive.list_reply.id;
                        if (listId.Contains("1_1"))
                        {
                            if (listId.Contains("1_1_"))
                            {
                                var splittedString = listId.Split("_");
                                var categoryId = 0;
                                int.TryParse(splittedString[2], out categoryId);
                                if (listId.Contains("1_1_" + categoryId + "_"))
                                {
                                    var splittedProductString = listId.Split("_");
                                    int productId = 0;
                                    int.TryParse(splittedProductString[3], out productId);
                                    bool success = await ProductDetailMessage(productId, from);
                                    if (success)
                                    {
                                        return new Response<string>("ACK");
                                    }
                                    else
                                    {
                                        return new Response<string>("ACK");
                                    }

                                }
                                else
                                {
                                    bool success = await ProductListMessage(categoryId, from);
                                    if (success)
                                    {
                                        return new Response<string>("ACK");
                                    }
                                    else
                                    {
                                        return new Response<string>("ACK");
                                    }
                                }

                            }
                            else
                            {
                                bool success = await CategoryListMessage(from);
                                if (success)
                                {
                                    return new Response<string>("ACK");
                                }
                                else
                                {
                                    return new Response<string>("ACK");
                                }

                            }

                        }
                        else if (listId.Contains("1_2"))
                        {
                            if (listId.Contains("1_2_"))
                            {
                                var splittedString = listId.Split("_");
                                int branchId = 0;
                                int.TryParse(splittedString[2], out branchId);

                                var response = new MetaResponse
                                {
                                    to = from,
                                };
                                response.text.body = "Gracias por comunicarte conmigo, dentro de poco uno de nuestros asesores te contactará 👋";
                                bool success = await _metaPostService.SendText(response);
                                if (success)
                                {
                                    await _metaRepositoryAsync.DeleteAsync(checkConversation!);
                                    await _metaRepositoryAsync.SaveChangesAsync();
                                    return new Response<string>("ACK");
                                }
                                else
                                {
                                    return new Response<string>("ACK");
                                }
                            }
                            else
                            {
                                bool succeeded = await DepartmentListMessage(from);
                                if (succeeded)
                                {
                                    return new Response<string>("ACK");
                                }
                                else
                                {
                                    return new Response<string>("ACK");
                                }
                            }


                        }
                        else if (listId == "1_3")
                        {
                            bool success = await BranchOfficesMessage(from);
                            if (success)
                            {
                                return new Response<string>("Ok");
                            }
                            else
                            {
                                return new Response<string>("Ok");
                            }


                        }
                        else
                        {
                            return new Response<string>("Ok");
                        }
                    }
                    else
                    {
                        return new Response<string>("ACK");
                    }

                }
                else
                {
                    if (messages.Reaction is not null)
                    {
                        return new Response<string>("Ok");
                    }
                    var checkConversation = await _metaRepositoryAsync.GetByIdAsync(from);
                    if (checkConversation == null)
                    {
                        var newRecord = new MetaConversations();
                        newRecord.Phone = from;
                        newRecord.Expiration = 0;
                        newRecord.ExpectingInfo = false;
                        newRecord.ProductId = 0;

                        await _metaRepositoryAsync.AddAsync(newRecord);
                        await _metaRepositoryAsync.SaveChangesAsync();

                        bool success = await WelcomeMessage(from);
                        if (success)
                        {
                            return new Response<string>("ACK");
                        }
                        else
                        {
                            return new Response<string>("ACK");
                        }
                    }
                    else
                    {
                        if (checkConversation.Expiration < messages.Timestamp)
                        {
                            checkConversation.Expiration = 0;
                            checkConversation.ExpectingInfo = false;
                            checkConversation.ProductId = 0;

                            await _metaRepositoryAsync.UpdateAsync(checkConversation);
                            await _metaRepositoryAsync.SaveChangesAsync();

                            bool success = await WelcomeMessage(from);
                            if (success)
                            {
                                return new Response<string>("ACK");
                            }
                            else
                            {
                                return new Response<string>("ACK");
                            }
                        }
                        else
                        {
                            if (checkConversation.ExpectingInfo == true)
                            {
                                var splittedString = messages.Text!.body!.Split('\n');
                                if (splittedString.Length < 2)
                                {
                                    var badMessage = new MetaResponse
                                    {
                                        to = from,
                                    };
                                    badMessage.text.body = "Lo siento, intentalo de nuevo. Verifica el ejemplo que te he enviado, recuerda que debes agregar un salto de linea entre cada dato";

                                    bool success = await _metaPostService.SendText(badMessage);
                                    if (success)
                                    {
                                        return new Response<string>("ACK");
                                    }
                                    else
                                    {
                                        return new Response<string>("ACK");
                                    }
                                }
                                else
                                {
                                    var response = new MetaResponse
                                    {
                                        to = from,
                                    };
                                    response.text.body = "Gracias por comunicarte conmigo, dentro de poco uno de nuestros asesores te contactará 👋";

                                    var selectedProduct = await _productRepositoryAsync.FirstOrDefaultAsync(new ProductIncludesSpecification(checkConversation.ProductId, slug: null));

                                    var newMail = new MailRequestDto();
                                    Guid validUser = await _assignUserToOpportunityService.FindValidUser();
                                    var assignedUser = await _userRepositoryAsync.GetByIdAsync(validUser);
                                    newMail.ToEmail = "josec@smartbusiness.site";
                                    newMail.Subject = "Nueva consulta de Maquinaria";
                                    newMail.Body = $@"¡Hola {assignedUser!.FullName}!,<br><br>

El Lead <b>{splittedString[0]}</b> ha consultado la <b>{selectedProduct!.SubCategory!.Name}</b>: <b>{selectedProduct!.Name}</b>. El Lead se ha contactado por Whatsapp. 
El correo del Lead es {splittedString[1]} y su numero telefónico es <b>{checkConversation.Phone.Insert(7, "-").Insert(3, " ").Insert(0, "+")}</b>";

                                    bool success = await _metaPostService.SendText(response);
                                    if (success)
                                    {
                                        await _metaRepositoryAsync.DeleteAsync(checkConversation);
                                        await _metaRepositoryAsync.SaveChangesAsync();
                                        await _mailService.SendEmailAsync(newMail);
                                        return new Response<string>("ACK");
                                    }
                                    else
                                    {
                                        return new Response<string>("ACK");
                                    }
                                }
                            }
                            else
                            {
                                return new Response<string>("ACK");
                            }
                        }
                    }
                }
            }
            return new Response<string>("Ok");
        }

        private async Task<bool> WelcomeMessage(string to)
        {
            var metaResponse = new MetaInteractive();
            metaResponse.type = "interactive";
            metaResponse.interactive.type = "list";
            metaResponse.interactive.header.text = "Guido ©Platino Motors";
            metaResponse.interactive.body.text = "¡Hola, soy Guido! 😁 \nSoy tu asistente personal, dime ¿En que te puedo ayudar?";
            metaResponse.to = to;
            metaResponse.interactive.action.button = "Opciones";
            var section = new MetaInteractiveSection();
            section.title = "";

            var interactiveRow1 = new MetaInteractiveRow
            {
                id = "1_1",
                title = "Consultar maquinas 🚜",
                description = "Obten una lista de nuestras maquinas destacadas"
            };

            var interactiveRow2 = new MetaInteractiveRow
            {
                id = "1_2",
                title = "Contacta un asesor 👨‍💻",
                description = "Obten ayuda personalizada"
            };

            var interactiveRow3 = new MetaInteractiveRow
            {
                id = "1_3",
                title = "Consultar sucursales 🏢",
                description = "Encuentra sucursales cerca de ti"
            };

            var interactiveRow4 = new MetaInteractiveRow
            {
                id = "1_4",
                title = "Reporta un problema ⚠",
                description = "Reportanos un problema y te ayudaremos a solucionarlo"
            };

            section.rows.Add(interactiveRow1);
            section.rows.Add(interactiveRow2);
            section.rows.Add(interactiveRow3);
            section.rows.Add(interactiveRow4);

            metaResponse.interactive.action.sections.Add(section);

            bool succeeded = await _metaPostService.SendInteractiveList(metaResponse);
            if (succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> CategoryListMessage(string to)
        {
            var categories = await _categoryRepositoryAsync.ListAsync();

            var categoriesResponse = new MetaInteractive();
            categoriesResponse.type = "interactive";
            categoriesResponse.interactive.type = "list";
            categoriesResponse.interactive.header.text = "Guido ©Platino Motors";
            categoriesResponse.interactive.body.text = "Tenemos un amplio surtido en maquinaria, ayudame a identificar el tipo de maquinaria que deseas consultar 🤔";
            categoriesResponse.to = to;
            categoriesResponse.interactive.action.button = "Categorías";
            var categoriesSection = new MetaInteractiveSection();
            categoriesSection.title = "";

            foreach (var category in categories)
            {
                if (category.Name.Length <= 24)
                {
                    var interactiveRow = new MetaInteractiveRow
                    {
                        id = "1_1_" + category.Id,
                        title = category.Name,
                    };

                    categoriesSection.rows.Add(interactiveRow);
                }
            }

            categoriesResponse.interactive.action.sections.Add(categoriesSection);

            bool succeeded = await _metaPostService.SendInteractiveList(categoriesResponse);
            if (succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> ProductListMessage(int categoryId, string to)
        {
            var productList = await _productRepositoryAsync.ListAsync(new FilterProductCategorySpecification(categoryId));

            if (productList.Count == 0)
            {
                var noFoundResponse = new MetaResponse();
                noFoundResponse.to = to;
                noFoundResponse.text.body = @"¡Uh, oh! No encuentro productos disponibles para esta categoría 😳 \n
                                ¿Deseas ponerte en contacto con un asesor? Te podemos dar más información sobre los productos de esta categoría.";
                bool succeeded2 = await _metaPostService.SendText(noFoundResponse);
                if (succeeded2)
                {
                    return true;
                }
                else
                {

                }
                return false;
            }

            var productResponse = new MetaInteractive();
            productResponse.type = "interactive";
            productResponse.interactive.type = "list";
            productResponse.interactive.header.text = "Guido ©Platino Motors";
            productResponse.interactive.body.text = @"He encontrado estos productos en nuestra base de datos 🤩, elige el producto que deseas consultar. Puedes elegir otro producto nuevamente de esta lista si deseas consultarlo.";
            productResponse.to = to;
            productResponse.interactive.action.button = "Productos";
            var productSection = new MetaInteractiveSection();
            productSection.title = "";

            foreach (var product in productList)
            {
                if (product.Name.Length <= 24)
                {
                    var interactiveRow = new MetaInteractiveRow
                    {
                        id = "1_1_" + categoryId + "_" + product.Id,
                        title = product.Name,
                        description = product.SubCategory!.Name
                    };
                    productSection.rows.Add(interactiveRow);
                }
            }

            productResponse.interactive.action.sections.Add(productSection);

            bool succeeded = await _metaPostService.SendInteractiveList(productResponse);
            if (succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> ProductDetailMessage(int productId, string to)
        {

            var product = await _productRepositoryAsync.FirstOrDefaultAsync(new ProductIncludesSpecification(productId, slug: null));
            var buttonReply = new MetaInteractiveButtonRoot();
            buttonReply.to = to;
            buttonReply.interactive.type = "button";
            buttonReply.interactive.body.text = $@"Tu producto solicitado 🫡
*Producto*: {product!.Name}
*Subcategoria*: {product.SubCategory!.Name}";
            for (int i = 0; i < product.ProductDataSheets!.Count; i++)
            {
                if (i > 2)
                {
                    break;
                }
                buttonReply.interactive.body.text += $"\n*{product.ProductDataSheets[i].DataSheet!.Name}*: {product.ProductDataSheets[i].Title}";
            }
            if (product.ProductImages!.Count > 0)
            {
                buttonReply.interactive.header.type = "image";
                buttonReply.interactive.header.text = "";
                buttonReply.interactive.header.image = new MetaInteractiveHeaderImage();
                buttonReply.interactive.header.image.link = product.ProductImages[0].Url;
            }
            else
            {
                buttonReply.interactive.header.type = "text";
                buttonReply.interactive.header.text = "Imagen no disponible";
            }

            var button1 = new MetaInteractiveButton();
            button1.reply.id = "Advisor_Product_" + productId;
            button1.reply.title = "¡Lo Quiero!";

            var button3 = new MetaInteractiveButton();
            button3.reply.id = "Visit_website_" + productId;
            button3.reply.title = "Sitio web";

            buttonReply.interactive.action.buttons.Add(button1);
            buttonReply.interactive.action.buttons.Add(button3);

            bool succeeded = await _metaPostService.SendInteractiveButton(buttonReply);
            if (succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> VisitProductWebsiteMessage(string productId, string to)
        {
            var textResponse = new MetaResponse();
            textResponse.to = to;
            textResponse.text.preview_url = true;
            textResponse.text.body = @"Puedes ver más información sobre esta maquina en nuestro sitio web 😎: 
https://motors.platino.hn/#/shop/products/" + productId;

            bool succeeded = await _metaPostService.SendText(textResponse);
            if (succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> BranchOfficesMessage(string to)
        {
            var branchOffices = await _branchRepositoryAsync.ListAsync();
            var branchResponse = new MetaResponse();
            branchResponse.to = to;
            if (branchOffices.Count == 0)
            {
                branchResponse.text.body = @"¡Uh oh! No he logrado encontrar sucursales disponibles por ahora. 😵
Porfavor, intenta de nuevo más tarde.";
                bool succeeded = await _metaPostService.SendText(branchResponse);
                if (succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                branchResponse.text.body = @"Tenemos las siguientes sucursales disponibles 🏢:

";
                foreach (var branch in branchOffices)
                {
                    branchResponse.text.body += $@"Sucursal 🏢: {branch.Name} 
Teléfono 📞: +504 {branch.PhoneNumber} 
Dirección 📍: {branch.Address} 
Mapa 🗺: https://www.google.com/maps/search/?api=1&query={branch.Lat}%2C{branch.Lng}{(branch.MapsId != null ? $"&query_place_id={branch.MapsId}" : "")}

";
                }
                bool succeeded = await _metaPostService.SendText(branchResponse);
                if (succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        private async Task<bool> DepartmentListMessage(string to)
        {
            var departments = await _departmentRepositryAsync.ListAsync();

            if (departments.Count == 0)
            {
                var noFoundResponse = new MetaResponse();
                noFoundResponse.to = to;
                noFoundResponse.text.body = @"¡Uh, oh! No encuentro departamentos disponibles. 😳 \n
                                Intentalo de nuevo más tarde.";
                bool succeeded2 = await _metaPostService.SendText(noFoundResponse);
                if (succeeded2)
                {
                    return true;
                }
                else
                {

                }
                return false;
            }

            var departmentResponse = new MetaInteractive();
            departmentResponse.type = "interactive";
            departmentResponse.interactive.type = "list";
            departmentResponse.interactive.header.text = "Guido ©Platino Motors";
            departmentResponse.interactive.body.text = @"Nuestros asesores están dispuestos a ayudarte. \n
Elige tu departamento. 🤔";
            departmentResponse.to = to;
            departmentResponse.interactive.action.button = "Departamentos";
            var branchSection = new MetaInteractiveSection();
            branchSection.title = "";

            foreach (var department in departments)
            {
                var interactiveRow = new MetaInteractiveRow
                {
                    id = "1_2_" + department.Id,
                    title = department.Name
                };
                branchSection.rows.Add(interactiveRow);
            }

            departmentResponse.interactive.action.sections.Add(branchSection);

            bool succeeded = await _metaPostService.SendInteractiveList(departmentResponse);
            if (succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
