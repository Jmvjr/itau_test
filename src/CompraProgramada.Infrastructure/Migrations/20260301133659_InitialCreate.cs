using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompraProgramada.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CestasRecomendadas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ativa = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    DataDesativacao = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CestasRecomendadas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CPF = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValorMensal = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    DataAdesao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cotacoes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DataPregao = table.Column<DateTime>(type: "DATE", nullable: false),
                    Ticker = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecoAbertura = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false),
                    PrecoFechamento = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false),
                    PrecoMaximo = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false),
                    PrecoMinimo = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotacoes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventosIR",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    Tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValorBase = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    ValorIR = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    PublicadoKafka = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DataEvento = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosIR", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrdensCompra",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContaMasterId = table.Column<long>(type: "bigint", nullable: false),
                    Ticker = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false),
                    TipoMercado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataExecucao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensCompra", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rebalanceamentos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    Tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TickerVendido = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TickerComprado = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValorVenda = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    DataRebalanceamento = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rebalanceamentos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CestaRecomendacaoItens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CestaId = table.Column<long>(type: "bigint", nullable: false),
                    Ticker = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Percentual = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CestaRecomendacaoItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CestaRecomendacaoItens_CestasRecomendadas_CestaId",
                        column: x => x.CestaId,
                        principalTable: "CestasRecomendadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContasGraficas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<long>(type: "bigint", nullable: true),
                    NumeroConta = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContasGraficas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContasGraficas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Custodias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContaGraficaId = table.Column<long>(type: "bigint", nullable: false),
                    Ticker = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoMedio = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false),
                    DataUltimaAtualizacao = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    ContaGraficaId1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Custodias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Custodias_ContasGraficas_ContaGraficaId",
                        column: x => x.ContaGraficaId,
                        principalTable: "ContasGraficas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Custodias_ContasGraficas_ContaGraficaId1",
                        column: x => x.ContaGraficaId1,
                        principalTable: "ContasGraficas",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Distribuicoes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrdemCompraId = table.Column<long>(type: "bigint", nullable: false),
                    CustodiaFilhoteId = table.Column<long>(type: "bigint", nullable: false),
                    Ticker = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantidade = table.Column<int>(type: "INT", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false),
                    DataDistribuicao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distribuicoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distribuicoes_Custodias_CustodiaFilhoteId",
                        column: x => x.CustodiaFilhoteId,
                        principalTable: "Custodias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Distribuicoes_OrdensCompra_OrdemCompraId",
                        column: x => x.OrdemCompraId,
                        principalTable: "OrdensCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CestaRecomendacaoItens_CestaId",
                table: "CestaRecomendacaoItens",
                column: "CestaId");

            migrationBuilder.CreateIndex(
                name: "UX_Cliente_CPF",
                table: "Clientes",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContasGraficas_ClienteId",
                table: "ContasGraficas",
                column: "ClienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ContaGrafica_Numero",
                table: "ContasGraficas",
                column: "NumeroConta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Cotacao_TickerData",
                table: "Cotacoes",
                columns: new[] { "Ticker", "DataPregao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Custodias_ContaGraficaId1",
                table: "Custodias",
                column: "ContaGraficaId1");

            migrationBuilder.CreateIndex(
                name: "UX_Custodia_ContaTicket",
                table: "Custodias",
                columns: new[] { "ContaGraficaId", "Ticker" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Distribuicao_OrdemData",
                table: "Distribuicoes",
                columns: new[] { "OrdemCompraId", "DataDistribuicao" });

            migrationBuilder.CreateIndex(
                name: "IX_Distribuicoes_CustodiaFilhoteId",
                table: "Distribuicoes",
                column: "CustodiaFilhoteId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoIR_ClienteData",
                table: "EventosIR",
                columns: new[] { "ClienteId", "DataEvento" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdemCompra_ContaMasterData",
                table: "OrdensCompra",
                columns: new[] { "ContaMasterId", "DataExecucao" });

            migrationBuilder.CreateIndex(
                name: "IX_Rebalanceamento_ClienteData",
                table: "Rebalanceamentos",
                columns: new[] { "ClienteId", "DataRebalanceamento" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CestaRecomendacaoItens");

            migrationBuilder.DropTable(
                name: "Cotacoes");

            migrationBuilder.DropTable(
                name: "Distribuicoes");

            migrationBuilder.DropTable(
                name: "EventosIR");

            migrationBuilder.DropTable(
                name: "Rebalanceamentos");

            migrationBuilder.DropTable(
                name: "CestasRecomendadas");

            migrationBuilder.DropTable(
                name: "Custodias");

            migrationBuilder.DropTable(
                name: "OrdensCompra");

            migrationBuilder.DropTable(
                name: "ContasGraficas");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
