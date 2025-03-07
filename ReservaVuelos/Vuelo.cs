using System;

namespace ReservaVuelos
{
    internal class Vuelo
    {
        public string Codigo { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public DateTime FechaSalida { get; set; }
        public int AsientosDisponibles { get; set; }

        public Vuelo(string Codigo, string Origen, string Destino, DateTime FechaSalida, int AsientosDisponibles)
        {
            this.Codigo = Codigo;
            this.Origen = Origen;
            this.Destino = Destino;
            this.FechaSalida = FechaSalida;
            this.AsientosDisponibles = AsientosDisponibles;
        }

        public void RestarAsientosDisponibles(int CantidadARestar)
        {
            AsientosDisponibles -= CantidadARestar;
        }

        public bool ValidarAsientosDisponibles(int CantidadAReservar)
        {
            return AsientosDisponibles >= CantidadAReservar;
        }
    }
}
