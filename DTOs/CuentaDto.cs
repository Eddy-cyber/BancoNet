using System;
using System.Collections.Generic;

namespace BancoNet.Dtos
{
    public class CuentaDto
    {
        public long No_Cuenta { get; set; }
        public string? Tipo { get; set; }
        public long? Saldo { get; set; }
        public string? Beneficiarios { get; set; }
        public long ClienteId { get; set; }

        public CuentaDto(long no_Cuenta, string? tipo, long? saldo, string? beneficiarios, long clienteId)
        {
            No_Cuenta = no_Cuenta;
            Tipo = tipo;
            Saldo = saldo;
            Beneficiarios = beneficiarios;
            ClienteId = clienteId;
        }

        public CuentaDto() {}
    }
}
