using GEPV.Domain.DTO;
using GEPV.Domain.Entities;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GEPV.Domain.SQL
{
    public class Consultas
    {
        private Entities.GEPVEntities db = new Entities.GEPVEntities();

        public List<Mensagem> GetMensagens()
        {
            string SQL = @"SELECT MENSAGEM.ID, MENSAGEM.TEXTO, MENSAGEM.DATA_VALIDADE, MENSAGEM.DATA_VALIDADE DataValidade,MENSAGEM.TIPO Tipo FROM MENSAGEM WHERE DATE(MENSAGEM.DATA_VALIDADE) >= DATE(NOW()) ORDER BY MENSAGEM.ID DESC";

            return db.Mensagem.FromSqlRaw(SQL).ToList();
        }

        public List<RegiaoMapa> GetRegioesFeatures()
        {
            string SQl = @"SELECT DESCRICAO, CONCAT('[', GROUP_CONCAT(FEATURE),']') AS FEATURES FROM REGIAO";

            return db.RegiaoMapa.FromSqlRaw(SQl).ToList();
        }

        public List<LatiLongCliente> GetLatiLongClientes()
        {
            string SQl = @"SELECT ID, CONCAT('[', GROUP_CONCAT(LATILONG),']') AS LATILONG FROM CLIENTE";

            return db.LatiLongCliente.FromSqlRaw(SQl).ToList();
        }


        //15 DIAS atrás ou à frente nesse mês atual
        //chega até 15 dias mes que vem
        //checa se é dezembro x janeiro
        public List<FeriadoCliente> GetFeriadosMes()
        {
            string SQl = @"SELECT F.UF, F.TIPO, F.MUNICIPIO, CONCAT_WS(': ', F.DATA, F.NOME) AS NOME FROM 
                    ESTADO E INNER JOIN FERIADO F ON F.UF = E.SIGLA OR F.UF IS NULL
                    WHERE 
	                    ( SUBSTRING_INDEX(F.DATA,'/',-1) = LPAD(MONTH(now()), 2, '0')
                    AND 
	                    (DAY(NOW()) - 15 <= SUBSTRING_INDEX(F.DATA,'/',1) AND DAY(NOW()) + 15 >= SUBSTRING_INDEX(F.DATA,'/',1)) )
                    OR
	                    (
		                    DAY(NOW()) + 15 > 30 AND SUBSTRING_INDEX(F.DATA,'/',-1) = MONTH(NOW()) +1 AND SUBSTRING_INDEX(F.DATA,'/',1) <= 15
	                    )
                    OR
	                    (
		                    MONTH(NOW()) = 12 AND DAY(NOW()) + 15 > 30 AND SUBSTRING_INDEX(F.DATA,'/',-1) = 1
	                    )
                     GROUP BY F.ID";

            return db.FeriadoCliente.FromSqlRaw(SQl).ToList();
        }

        public List<TarefasVendedores> GetVendedores()
        {
            string SQl = @"select
                                NULL as QtdeCliente,
	                            ID IdVendedor,
                                NOME NomeVendedor
                            from VENDEDOR
                            ORDER BY NOME";

            return db.TarefasVendedores.FromSqlRaw(SQl).ToList();
        }

        public List<Cliente> GetAllClientesList()
        {
            try
            {
                return db.Cliente
                .Include(cliente => cliente.Regiao)
                .Include(cliente => cliente.Estado)
                .Include(cliente => cliente.Vendedor)
                .ToList();

            }
            catch (InvalidCastException ex)
            {
                return new List<Cliente>();
            }
        }

        public IEnumerable<Cliente> GetCnpjCliente(int idClienteMatriz)
        {
            return db.Cliente
                     .Where(x => x.Id == idClienteMatriz || x.IdMatriz == idClienteMatriz)
                     .ToList();
        }

        public List<TarefasClientes> GetClientes(int idVendedor)
        {
            string SQL = string.Format(@"
SELECT DISTINCT 
		CLIENTE.Id IdCliente, 
        CLIENTE.CIDADE, 
		ESTADO.SIGLA UfEstado,
		COALESCE(CASE 			 
			 WHEN MIN(FPC.COD_CORCLIENTE) = 0 THEN 'bg-danger'
             WHEN MAX(FPC.CONTATODIA) > 0 THEN 'bg-success'
             WHEN MIN(FPC.COD_CORCLIENTE) = 1 AND MAX(FPC.CONTATODIA) = 0 THEN 'bg-success'
             WHEN MIN(FPC.COD_CORCLIENTE) = 2 AND MAX(FPC.CONTATODIA) = 0 THEN 'bg-info'
             WHEN MIN(FPC.COD_CORCLIENTE) = 3 AND MAX(FPC.CONTATODIA) = 0 THEN 'bg-light' 
             END, 'bg-light')
        CorCliente,	
        MAX(FPC.CONTATODIA),
        MIN(COALESCE(FPC.COD_CORCLIENTE, 3)),
		CLIENTE.RAZAO_SOCIAL Nome,
		REGIAO.DESCRICAO RegiaoDescricao,
		MAX(FPC.UltimoContato) UltimoContato,
		MAX(FPC.UltimaCompra) UltimaCompra,
		MIN(FPC.ProximoContato) ProximoContato,
		CLIENTE.TELEFONE_PRINCIPAL Contato,
		CLIENTE.NOME_COMPRADOR Responsavel,
		CLIENTE.EMAIL_PRINCIPAL Email,
		CLIENTE.ID_VENDEDOR IdVendedor,
		CLIENTE.POTENCIAL, NULL PotencialNome,
        CLIENTE.OBSERVACAO Observacao
FROM CLIENTE
LEFT JOIN VW_FORNECEDOR_POR_CLIENTE FPC ON (FPC.ID_CLIENTE = CLIENTE.ID OR FPC.ID_MATRIZ = CLIENTE.ID)
LEFT JOIN REGIAO ON REGIAO.ID = CLIENTE.ID_REGIAO
LEFT JOIN ESTADO ON ESTADO.ID = CLIENTE.ID_ESTADO
WHERE CLIENTE.SITUACAO = 'A'
AND CLIENTE.ID_MATRIZ IS NULL
AND  0 < (SELECT COUNT(1) FROM VENDEDOR 
WHERE VENDEDOR.ID = {0} AND (
	VENDEDOR.ADMIN = 1 OR 
		(VENDEDOR.ADMIN = 0 AND 0 < (SELECT COUNT(1) FROM VENDEDOR VEND 
										WHERE VEND.ID = CLIENTE.ID_VENDEDOR
                                        AND VEND.ADMIN = 0
        ))
))
GROUP BY CLIENTE.Id,
		 CLIENTE.RAZAO_SOCIAL,
		 REGIAO.DESCRICAO,
		 CLIENTE.TELEFONE_PRINCIPAL,
		 CLIENTE.TELEFONE_CONTATO,
		 CLIENTE.NOME_COMPRADOR,
		 CLIENTE.EMAIL_PRINCIPAL,
         CLIENTE.OBSERVACAO
ORDER BY MIN(COALESCE(FPC.COD_CORCLIENTE, 3)),
         CLIENTE.RAZAO_SOCIAL", idVendedor);

            try
            {
                return db.TarefasClientes.FromSqlRaw(SQL).ToList();
            }
            catch (InvalidCastException ex)
            {
                return new List<TarefasClientes>();
            }
        }

        public List<ExportClientes> GetExportClientes(int vendedorId = 0)
        {
            string SQL = @"SELECT CONCAT_WS(';', C.RAZAO_SOCIAL,C.NOME_FANTASIA,C.CNPJ,C.INSCRICAO_ESTADUAL,C.TELEFONE_PRINCIPAL,
                            C.TELEFONE_CONTATO,C.EMAIL_PRINCIPAL,C.EMAIL_NFE,C.OBSERVACAO,C.LOGRADOURO,
                            C.NUMERO,C.BAIRRO,C.CEP,C.CIDADE,ESTADO.SIGLA,REGIAO.DESCRICAO,C.NOME_COMPRADOR,VDD.NOME)
                            AS LINHA
                            FROM CLIENTE C                          
                            INNER JOIN VENDEDOR VDD ON VDD.ID = C.ID_VENDEDOR AND VDD.ID = " + vendedorId
                          + " INNER JOIN ESTADO ON ESTADO.ID = C.ID_ESTADO"
                          + " INNER JOIN REGIAO ON REGIAO.ID = C.ID_REGIAO";


            return db.ExportClientes.FromSqlRaw(SQL).ToList();
        }

        public List<TarefasFornecedores> GetFornecedoresPorCliente(int? idCliente = 0)
        {
            string SQL = string.Format(@"
SELECT 
FPC.ID_CLIENTE IdCliente,
FPC.CORFORNECEDORCLIENTE,
FPC.IDFORNECEDOR, 
FPC.NOME,
FPC.SIGLA,
FPC.ULTIMOCONTATO,
FPC.ULTIMACOMPRA,
FPC.PROXIMOCONTATO,
FPC.SITUACAO,
FPC.COD_CORCLIENTE, 
CASE WHEN FPC.COD_CORCLIENTE = 2 THEN FPC.PROXIMOCONTATO ELSE NULL END ORD_CONTATOREALIZADO
FROM VW_FORNECEDOR_POR_CLIENTE FPC
WHERE (FPC.ID_CLIENTE = {0} OR FPC.ID_MATRIZ = {0})
UNION ALL
SELECT {0} IDCLIENTE,
	'bg-light' CORFORNECEDORCLIENTE,                                    
	FORNECEDOR.ID IDFORNECEDOR,
	FORNECEDOR.NOME_FANTASIA NOME,
	FORNECEDOR.SIGLA_FORNECEDOR SIGLA,
	NULL ULTIMOCONTATO,
	NULL ULTIMACOMPRA,
	NULL PROXIMOCONTATO,
	'NUNCA REALIZADO CONTATO' SITUACAO,
    3 COD_CORCLIENTE,
    NULL ORD_CONTATOREALIZADO
    FROM FORNECEDOR
    WHERE 0 = (SELECT COUNT(1) FROM CONTATOS
		INNER JOIN CLIENTE ON CLIENTE.ID = CONTATOS.ID_CLIENTE OR CLIENTE.ID_MATRIZ = CONTATOS.ID_CLIENTE
    WHERE CONTATOS.ID_FORNECEDOR = FORNECEDOR.ID AND (CLIENTE.ID = {0} OR CLIENTE.ID_MATRIZ = {0}))
ORDER BY COD_CORCLIENTE, ORD_CONTATOREALIZADO, PROXIMOCONTATO DESC, NOME", idCliente.Value.ToString());

            return db.TarefasFornecedores.FromSqlRaw(SQL).ToList();
        }

        public List<HistoricoDTO> GetHistoricoContatos(int? idCliente, int? idVendedor, int? idFornecedor)
        {
            idCliente = idCliente ?? 0;
            idVendedor = idVendedor ?? 0;
            idFornecedor = idFornecedor ?? 0;

            var SqlSelect = $@"SELECT 
                                CONTATOS.ID IdHistorico,
                                CLIENTE.ID IdCliente,
                                CLIENTE.CNPJ CnpjCliente,
                                CLIENTE.RAZAO_SOCIAL NomeCliente,
                                FORNECEDOR.ID IdFornecedor,
                                FORNECEDOR.NOME_FANTASIA NomeFornecedor,
                                COALESCE(VENDEDOR.ID, 0) IdVendedor,
                                COALESCE(VENDEDOR.NOME, '-') NomeVendedor,
                                DATA_CONTATO DataContato,
                                DATA_COMPRA DataCompra,
                                DATA_AGENDA DataAgenda,    
                                DESCRICAO Negociacao
                            FROM CONTATOS
                            INNER JOIN CLIENTE ON CONTATOS.ID_CLIENTE = CLIENTE.ID
                            LEFT JOIN VENDEDOR ON CONTATOS.ID_VENDEDOR = VENDEDOR.ID
                            INNER JOIN FORNECEDOR ON CONTATOS.ID_FORNECEDOR = FORNECEDOR.ID ";

            var SqlWhere = $@"WHERE 1 = 1 ";

            if (idCliente != 0)
                SqlWhere += $@"AND (CLIENTE.ID = {idCliente} OR CLIENTE.ID_MATRIZ = {idCliente})";

            if (idVendedor != 0)
                SqlWhere += $@"AND CONTATOS.ID_VENDEDOR = {idVendedor} ";

            if (idFornecedor != 0)
                SqlWhere += $@"AND CONTATOS.ID_FORNECEDOR = {idFornecedor} ";

            var SqlOrderBy = $@"ORDER BY DATA_CONTATO DESC";

            var SqlFull = $@"{SqlSelect} {SqlWhere} {SqlOrderBy}";

            return db.HistoricoDTO.FromSqlRaw(SqlFull).ToList();
        }
    }
}
