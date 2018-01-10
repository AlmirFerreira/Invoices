using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Instancia Variaveis
            var arquivoClienteLista = new List<Cliente>();
            var clientesComCepValido = new List<Cliente>();
            var faturaZeradaLista = new List<Cliente>();
            var registrosAte6Paginas = new List<Cliente>();
            var registrosAte12Paginas = new List<Cliente>();
            var registrosMais12Paginas = new List<Cliente>();
            #endregion

            #region Le Arquivo Base
            LeArquivoBase(arquivoClienteLista, @"..\..\Arquivos\Baseficticia.txt");
            #endregion

            #region Realiza Validacoes
            var valicacoes = new Validacoes();

            foreach (var item in arquivoClienteLista)
            {
                if (valicacoes.ValidaCepSimplificado(item.CEP))
                {
                    clientesComCepValido.Add(item);

                    ValidaFaturasZeradas(faturaZeradaLista, item);

                    VerificaNumeroPaginasImpares(item);

                    SeparaPorNumeroPaginas(registrosAte6Paginas, registrosAte12Paginas, registrosMais12Paginas, item);

                }

            }
            #endregion

            #region Gera Arquivos CSV
            GeraArquivosCsv(faturaZeradaLista, @"..\..\Arquivos\Zerados.csv");
            GeraArquivosCsv(registrosAte6Paginas, @"..\..\Arquivos\RegistrosAte6Paginas.csv");
            GeraArquivosCsv(registrosAte12Paginas, @"..\..\Arquivos\RegistrosAte12Paginas.csv");
            GeraArquivosCsv(registrosMais12Paginas, @"..\..\Arquivos\RegistrosMais12Paginas.csv"); 
            #endregion

        }

        #region Metodos 
        private static void GeraArquivosCsv(List<Cliente> faturaZeradaLista, string caminhoArquivo)
        {
            using (var file = File.CreateText(caminhoArquivo))
            {
                file.WriteLine("NomeCliente;EnderecoCompleto;ValorFatura;NumeroPaginas");

                foreach (var item in faturaZeradaLista)
                {
                    StringBuilder linha = new StringBuilder();
                    linha.AppendFormat("{0};", item.NomeCliente);
                    linha.AppendFormat("{0} {1} {2} {3};", item.RuaComComplemento, item.CEP, item.Cidade, item.Estado);
                    linha.AppendFormat("{0};", item.ValorFatura);
                    linha.AppendFormat("{0};", item.NumeroPaginas);

                    file.WriteLine(linha.ToString());
                }
            }
        }

        private static void LeArquivoBase(List<Cliente> arquivoClienteLista, string caminhoArquivo)
        {
            string[] linhas = System.IO.File.ReadAllLines(caminhoArquivo);

            for (int i = 1; i < linhas.Length; i++)
            {

                var linhaItens = linhas[i].Split(';').ToList();

                var cliente = new Cliente()
                {
                    NomeCliente = linhaItens[0].ToString(),
                    CEP = linhaItens[1].ToString(),
                    RuaComComplemento = linhaItens[2].ToString(),
                    Bairro = linhaItens[3].ToString(),
                    Cidade = linhaItens[4].ToString(),
                    Estado = linhaItens[5].ToString(),
                    ValorFatura = Convert.ToDouble(linhaItens[6].ToString()),
                    NumeroPaginas = Int32.Parse(linhaItens[7].ToString())
                };

                arquivoClienteLista.Add(cliente);
            }
        }


        private static void SeparaPorNumeroPaginas(List<Cliente> registrosAte6Paginas, List<Cliente> registrosAte12Paginas, List<Cliente> registrosMais12Paginas, Cliente item)
        {
            if (!item.ValorFatura.Equals(0))
            {
                if (item.NumeroPaginas <= 6)
                {
                    registrosAte6Paginas.Add(item);
                }
                else if (item.NumeroPaginas > 6 && item.NumeroPaginas <= 12)
                {
                    registrosAte12Paginas.Add(item);
                }
                else if (item.NumeroPaginas > 12)
                {
                    registrosMais12Paginas.Add(item);
                } 
            }
        }


        private static void ValidaFaturasZeradas(List<Cliente> faturaZeradaLista, Cliente item)
        {
            if (item.ValorFatura.Equals(0))
            {
                faturaZeradaLista.Add(item);
            }
        }

        private static void VerificaNumeroPaginasImpares(Cliente item)
        {
            if (!(item.NumeroPaginas % 2 == 0))
            {
                ++item.NumeroPaginas;
            }
        } 
        #endregion

    }
}
