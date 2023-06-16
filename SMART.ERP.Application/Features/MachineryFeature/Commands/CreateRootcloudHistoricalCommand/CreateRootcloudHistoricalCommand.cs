using MediatR;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Application.Services.Rootcloud;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateRootcloudHistoricalCommand
{
    public class CreateRootcloudHistoricalCommand : IRequest<Response<string>>
    {
        public class CreateRootcloudHistoricalCommandHandler : IRequestHandler<CreateRootcloudHistoricalCommand, Response<string>>
        {
            private readonly IRootcloudHistoricalService _rootcloudHistoricalService;

            public CreateRootcloudHistoricalCommandHandler(IRootcloudHistoricalService rootcloudHistoricalService)
            {
                _rootcloudHistoricalService = rootcloudHistoricalService;
            }

            public async Task<Response<string>> Handle(CreateRootcloudHistoricalCommand request, CancellationToken cancellationToken)
            {
                await _rootcloudHistoricalService.RootcloudHistoricalJob();

                return new Response<string>("Se actualizaron todos los registros");
            }
        }
    }

}
