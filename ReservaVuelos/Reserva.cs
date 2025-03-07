using System;

namespace ReservaVuelos
{
    internal class Reserva
    {
        public string CodigoReserva { get; set; }
        public Vuelo Vuelo{ get; set; }
        public int AsientosReservados { get; set; }

        public Reserva(string CodigoReserva, Vuelo Vuelo, int AsientosReservados)
        {
            this.CodigoReserva = CodigoReserva;
            this.Vuelo = Vuelo;
            this.AsientosReservados = AsientosReservados;
        }
    }
}
