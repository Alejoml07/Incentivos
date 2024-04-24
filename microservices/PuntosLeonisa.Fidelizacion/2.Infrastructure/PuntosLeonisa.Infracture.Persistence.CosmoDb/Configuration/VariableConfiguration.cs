using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class VariableConfiguration : IEntityTypeConfiguration<Variable>
    {
        public void Configure(EntityTypeBuilder<Variable> builder)
        {
            builder.ToContainer("Variable")
                .HasPartitionKey(e => e.Id);
        }
    }
}
