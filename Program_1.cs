using System;

namespace Agenda
{
    class Program
    {
        static Contatos contatos = new Contatos();

        static void Main(string[] args)
        {
            int opcao;

            do
            {
                MostrarMenu();
                opcao = LerInteiro("Opção: ");
                Console.WriteLine();

                switch (opcao)
                {
                    case 0: Console.WriteLine("Encerrando..."); break;
                    case 1: AdicionarContato(); break;
                    case 2: PesquisarContato(); break;
                    case 3: AlterarContato(); break;
                    case 4: RemoverContato(); break;
                    case 5: ListarContatos(); break;
                    default: Console.WriteLine("Opção inválida!"); break;
                }

                if (opcao != 0)
                {
                    Console.WriteLine("\nPressione ENTER para continuar...");
                    Console.ReadLine();
                }
            } while (opcao != 0);
        }

        static void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("===== AGENDA DE CONTATOS =====");
            Console.WriteLine("0. Sair");
            Console.WriteLine("1. Adicionar contato");
            Console.WriteLine("2. Pesquisar contato");
            Console.WriteLine("3. Alterar contato");
            Console.WriteLine("4. Remover contato");
            Console.WriteLine("5. Listar contatos");
            Console.WriteLine("==============================");
        }

        // ---------- Opções do menu ----------

        static void AdicionarContato()
        {
            Console.WriteLine("--- Adicionar contato ---");

            string email = LerTexto("E-mail: ");
            if (contatos.pesquisar(new Contato(email, "", null)) != null)
            {
                Console.WriteLine("Já existe um contato com esse e-mail.");
                return;
            }

            string nome = LerTexto("Nome: ");
            Data dtNasc = LerData("Data de nascimento (dd/mm/aaaa): ");

            Contato c = new Contato(email, nome, dtNasc);
            LerTelefones(c);

            Console.WriteLine(contatos.adicionar(c)
                ? "Contato adicionado com sucesso!"
                : "Não foi possível adicionar o contato.");
        }

        static void PesquisarContato()
        {
            Console.WriteLine("--- Pesquisar contato ---");

            string email = LerTexto("E-mail do contato: ");
            Contato encontrado = contatos.pesquisar(new Contato(email, "", null));

            if (encontrado == null)
            {
                Console.WriteLine("Contato não encontrado.");
                return;
            }

            MostrarDetalhes(encontrado);
        }

        static void AlterarContato()
        {
            Console.WriteLine("--- Alterar contato ---");

            string email = LerTexto("E-mail do contato a alterar: ");
            Contato atual = contatos.pesquisar(new Contato(email, "", null));

            if (atual == null)
            {
                Console.WriteLine("Contato não encontrado.");
                return;
            }

            Console.WriteLine("Deixe em branco para manter o valor atual.\n");

            Console.Write($"Nome [{atual.Nome}]: ");
            string nome = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nome)) nome = atual.Nome;

            Data dtNasc = atual.DtNasc;
            while (true)
            {
                Console.Write($"Data de nascimento [{atual.DtNasc}]: ");
                string txt = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(txt)) break;
                if (Data.TryParse(txt, out Data nova)) { dtNasc = nova; break; }
                Console.WriteLine("Data inválida. Use o formato dd/mm/aaaa.");
            }

            Contato alterado = new Contato(atual.Email, nome, dtNasc);

            Console.Write("Deseja substituir os telefones? (s/n): ");
            if (Console.ReadLine().Trim().ToLower() == "s")
            {
                LerTelefones(alterado);
            }
            else
            {
                foreach (Telefone t in atual.Telefones)
                    alterado.adicionarTelefone(t);
            }

            Console.WriteLine(contatos.alterar(alterado)
                ? "Contato alterado com sucesso!"
                : "Não foi possível alterar o contato.");
        }

        static void RemoverContato()
        {
            Console.WriteLine("--- Remover contato ---");

            string email = LerTexto("E-mail do contato a remover: ");
            Contato c = new Contato(email, "", null);

            Console.WriteLine(contatos.remover(c)
                ? "Contato removido com sucesso!"
                : "Contato não encontrado.");
        }

        static void ListarContatos()
        {
            Console.WriteLine("--- Lista de contatos ---");

            if (contatos.Agenda.Count == 0)
            {
                Console.WriteLine("A agenda está vazia.");
                return;
            }

            foreach (Contato c in contatos.Agenda)
                Console.WriteLine(c);
        }

        // ---------- Auxiliares ----------

        static void MostrarDetalhes(Contato c)
        {
            Console.WriteLine($"Nome: {c.Nome}");
            Console.WriteLine($"E-mail: {c.Email}");
            Console.WriteLine($"Nascimento: {c.DtNasc} ({c.getIdade()} anos)");

            if (c.Telefones.Count == 0)
            {
                Console.WriteLine("Telefones: nenhum");
                return;
            }

            Console.WriteLine("Telefones:");
            foreach (Telefone t in c.Telefones)
                Console.WriteLine($"  - {t}");
        }

        static void LerTelefones(Contato c)
        {
            while (true)
            {
                Console.Write("Adicionar telefone? (s/n): ");
                if (Console.ReadLine().Trim().ToLower() != "s") break;

                string tipo = LerTexto("  Tipo (ex.: celular, residencial): ");
                string numero = LerTexto("  Número: ");
                Console.Write("  É o principal? (s/n): ");
                bool principal = Console.ReadLine().Trim().ToLower() == "s";

                c.adicionarTelefone(new Telefone(tipo, numero, principal));
            }
        }

        static string LerTexto(string mensagem)
        {
            string texto;
            do
            {
                Console.Write(mensagem);
                texto = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(texto));

            return texto.Trim();
        }

        static int LerInteiro(string mensagem)
        {
            Console.Write(mensagem);
            return int.TryParse(Console.ReadLine(), out int valor) ? valor : -1;
        }

        static Data LerData(string mensagem)
        {
            while (true)
            {
                Console.Write(mensagem);
                if (Data.TryParse(Console.ReadLine(), out Data data))
                    return data;

                Console.WriteLine("Data inválida. Use o formato dd/mm/aaaa.");
            }
        }
    }
}
