using Cgr.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cgr.Api.Infrastructure.Data;

/// <summary>
/// Contexto EF Core com toda a modelagem relacional do sistema.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Tabela de pessoas.
    /// </summary>
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();

    /// <summary>
    /// Tabela de categorias.
    /// </summary>
    public DbSet<Categoria> Categorias => Set<Categoria>();

    /// <summary>
    /// Tabela de finalidades.
    /// </summary>
    public DbSet<Finalidade> Finalidades => Set<Finalidade>();

    /// <summary>
    /// Tabela de tipos de transacao.
    /// </summary>
    public DbSet<TipoTransacao> TiposTransacao => Set<TipoTransacao>();

    /// <summary>
    /// Tabela de transacoes.
    /// </summary>
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.ToTable("pessoa", table =>
            {
                table.HasCheckConstraint("CK_pessoa_idade_non_negative", "idade >= 0");
                table.HasCheckConstraint("CK_pessoa_nome_max", "length(nome) <= 200");
            });

            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Nome).HasColumnName("nome").IsRequired();
            entity.Property(p => p.Idade).HasColumnName("idade").IsRequired();
        });

        modelBuilder.Entity<Finalidade>(entity =>
        {
            entity.ToTable("finalidade");
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).HasColumnName("id");
            entity.Property(f => f.Descricao).HasColumnName("descricao").IsRequired();

            entity.HasData(
                new Finalidade { Id = 1, Descricao = "despesa" },
                new Finalidade { Id = 2, Descricao = "receita" },
                new Finalidade { Id = 3, Descricao = "ambas" }
            );
        });

        modelBuilder.Entity<TipoTransacao>(entity =>
        {
            entity.ToTable("tipo_transacao");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.Descricao).HasColumnName("descricao").IsRequired();

            entity.HasData(
                new TipoTransacao { Id = 1, Descricao = "despesa" },
                new TipoTransacao { Id = 2, Descricao = "receita" }
            );
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categoria", table =>
            {
                table.HasCheckConstraint("CK_categoria_descricao_max", "length(descricao) <= 400");
            });

            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.Descricao).HasColumnName("descricao").IsRequired();
            entity.Property(c => c.IdFinalidade).HasColumnName("id_finalidade").IsRequired();

            entity.HasOne(c => c.Finalidade)
                .WithMany(f => f.Categorias)
                .HasForeignKey(c => c.IdFinalidade)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.ToTable("transacao", table =>
            {
                table.HasCheckConstraint("CK_transacao_descricao_max", "length(descricao) <= 400");
                table.HasCheckConstraint("CK_transacao_valor_positive", "valor > 0");
            });

            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.Descricao).HasColumnName("descricao").IsRequired();
            entity.Property(t => t.Valor).HasColumnName("valor").HasPrecision(18, 2).IsRequired();
            entity.Property(t => t.IdTipo).HasColumnName("id_tipo").IsRequired();
            entity.Property(t => t.IdCategoria).HasColumnName("id_categoria").IsRequired();
            entity.Property(t => t.IdPessoa).HasColumnName("id_pessoa").IsRequired();

            entity.HasIndex(t => t.IdPessoa);
            entity.HasIndex(t => t.IdCategoria);

            entity.HasOne(t => t.Pessoa)
                .WithMany(p => p.Transacoes)
                .HasForeignKey(t => t.IdPessoa)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.Categoria)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(t => t.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Tipo)
                .WithMany(tp => tp.Transacoes)
                .HasForeignKey(t => t.IdTipo)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
