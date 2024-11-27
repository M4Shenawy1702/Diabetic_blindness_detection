using CheckEyePro.Core.Dtos;
using CheckEyePro.Core.Models;
using Mapster;

namespace CheckEyePro.Core.Mapping
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Map from History entity to HistoryDto
            config.NewConfig<History, HistoryDto>()
                .Map(dest => dest.PaymentStatus, src => PaymentStatus.Paid);
        }
    }
}
