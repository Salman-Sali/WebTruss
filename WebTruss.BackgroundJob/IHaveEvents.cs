using System.ComponentModel.DataAnnotations.Schema;

namespace WebTruss.BackgroundJob
{
    public interface IHaveEvents
    {
        [NotMapped]
        List<BaseEvent> DomainEvents { get; set; }
    }
}
