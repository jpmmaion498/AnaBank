using System.Text.RegularExpressions;

namespace AnaBank.Accounts.Domain.ValueObjects;

public record Cpf
{
    public string Value { get; }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CPF não pode ser nulo ou vazio", nameof(value));

        var cleanCpf = Regex.Replace(value, @"[^\d]", "");
        
        if (!IsValid(cleanCpf))
            throw new ArgumentException("CPF inválido", nameof(value));

        Value = cleanCpf;
    }

    private static bool IsValid(string cpf)
    {
        if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
            return false;

        var digits = cpf.Select(c => int.Parse(c.ToString())).ToArray();

        // Primeiro dígito verificador
        var sum1 = 0;
        for (int i = 0; i < 9; i++)
            sum1 += digits[i] * (10 - i);
        
        var remainder1 = sum1 % 11;
        var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

        if (digits[9] != digit1)
            return false;

        // Segundo dígito verificador
        var sum2 = 0;
        for (int i = 0; i < 10; i++)
            sum2 += digits[i] * (11 - i);
        
        var remainder2 = sum2 % 11;
        var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

        return digits[10] == digit2;
    }

    public override string ToString() => Value;

    public static implicit operator string(Cpf cpf) => cpf.Value;
    public static implicit operator Cpf(string value) => new(value);
}