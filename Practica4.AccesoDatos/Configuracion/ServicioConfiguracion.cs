using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practica4.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica4.AccesoDatos.Configuracion
{
    public class ServicioConfiguracion : IEntityTypeConfiguration<Servicio>
    {
        public void Configure(EntityTypeBuilder<Servicio> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Nombre).IsRequired().HasMaxLength(60);
            builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
            builder.Property(x=>x.CategoriaId).IsRequired();
            builder.Property(x => x.PadreId).IsRequired(false);
            builder.Property(x => x.ImagenUrl).IsRequired(false);

            //Relaciones
            builder.HasOne(x=>x.Categoria).WithMany().HasForeignKey(x=>x.CategoriaId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Padre).WithMany().HasForeignKey(x => x.PadreId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
