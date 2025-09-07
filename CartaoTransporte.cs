using System;
using System.Linq;




public class CartaoTransporte
{
    public enum StatusCartao        
    {
        Ativo,
        Bloqueado
    }

    private const decimal LIMITE_RECARGA_MAXIMO = 200.0m; 
    private const int HISTORICO_TAMANHO = 5;

    private decimal _saldo;
    private decimal[] _historicoRecargas;
    private StatusCartao _status;

    public string NumeroCartao { get; }         

    public decimal Saldo                    //Propriedade Completa
    {
        get { return _saldo; }
    }

    public StatusCartao Status              //Invariantes status cartao
    {                                       //Propriedade Completa
        get { return _status; }
        private set { _status = value; }
    }
    public CartaoTransporte(string numeroCartao)      //Construtor da Classe   //Invariantes numero do cartao //Auto Property
    {                                                    
        if (string.IsNullOrWhiteSpace(numeroCartao))
        {
            throw new ArgumentException("O número do cartão não pode ser nulo ou vazio.", nameof(numeroCartao));
        }

        NumeroCartao = numeroCartao;
        _saldo = 0.0m;
        _status = StatusCartao.Ativo;
        _historicoRecargas = new decimal[HISTORICO_TAMANHO];
    }

    public void Recarregar(decimal valor)           //Invariantes bloqueio cartao
    {                                               
        if (Status == StatusCartao.Bloqueado)
        {
            throw new InvalidOperationException("Não é possível recarregar um cartão bloqueado.");
        }

        if (valor <= 0 || valor > LIMITE_RECARGA_MAXIMO)
        {
            throw new ArgumentException($"O valor da recarga deve ser maior que zero e não pode exceder R$ " +
                $"{LIMITE_RECARGA_MAXIMO}.", nameof(valor));
        }

        _saldo += valor;
        AdicionarHistoricoRecarga(valor);
    }


    public void PagarTarifa(decimal tarifa)
    {
        if (Status == StatusCartao.Bloqueado)
        {
            throw new InvalidOperationException("Não é possível usar um cartão bloqueado.");
        }

        if (tarifa <= 0)
        {
            throw new ArgumentException("A tarifa deve ser maior que zero.", nameof(tarifa));
        }

        if (_saldo < tarifa)  //Invariantes saldo não pode ser negativo
        {
            throw new InvalidOperationException("Saldo insuficiente para pagar a tarifa.");
        }

        _saldo -= tarifa;
    }

    public void Bloquear()
    {
        Status = StatusCartao.Bloqueado;
    }

    public decimal[] ObterHistoricoRecargas()
    {
        return (decimal[])_historicoRecargas.Clone();
    }

    private void AdicionarHistoricoRecarga(decimal valor)
    {
        for (int i = _historicoRecargas.Length - 1; i > 0; i--)
        {
            _historicoRecargas[i] = _historicoRecargas[i - 1];
        }
        _historicoRecargas[0] = valor;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        

        try
        {
            Console.WriteLine("Cenário de Sucesso: Recarga e Uso Válidos");
            var cartao1 = new CartaoTransporte("001");
            Console.WriteLine($"Cartão {cartao1.NumeroCartao} criado. Saldo inicial: {cartao1.Saldo:C}");

            cartao1.Recarregar(30.0m);
            Console.WriteLine($"Recarregando R$ 30,00. Saldo atual: {cartao1.Saldo:C}");

            cartao1.PagarTarifa(5.0m);
            Console.WriteLine($"Pagando R$ 5,00 de tarifa. Saldo atual: {cartao1.Saldo:C}");

            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
        }

        try
        {
            Console.WriteLine("\nCenário de Falha 1: Saldo Insuficiente");
            var cartao2 = new CartaoTransporte("002");
            cartao2.Recarregar(2.0m);
            Console.WriteLine($"Cartão {cartao2.NumeroCartao} recarregado com R$ 2,00. Saldo: {cartao2.Saldo:C}");

            cartao2.PagarTarifa(5.0m);
            Console.WriteLine("Esta linha não deveria ser executada.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"ERRO (esperado): {ex.Message}");
        }

        try
        {
            Console.WriteLine("\nCenário de Falha 2: Recarga com Valor Negativo");
            var cartao3 = new CartaoTransporte("003");

            cartao3.Recarregar(-10.0m);
            Console.WriteLine("Esta linha não deveria ser executada.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"ERRO (esperado): {ex.Message}");
        }

        try
        {
            Console.WriteLine("\nCenário de Falha 3: Operação em Cartão Bloqueado");
            var cartao4 = new CartaoTransporte("004");
            cartao4.Bloquear();
            Console.WriteLine($"Cartão {cartao4.NumeroCartao} bloqueado.");

            cartao4.Recarregar(10.0m);
            Console.WriteLine("Esta linha não deveria ser executada.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"ERRO (esperado): {ex.Message}");
        }

        Console.WriteLine("\n--- Fim da Demonstração ---");
    }
}
