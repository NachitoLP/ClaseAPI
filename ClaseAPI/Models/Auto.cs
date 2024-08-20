using System;

namespace ClaseAPI.Models
{
    public class Auto
    {
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Año { get; set; }
        public Motor Motor { get; set; }
        public Transmision Transmision { get; set; }
        public string VelocidadMaxima { get; set; }
        public Confort Confort { get; set; }

    }

    public class Motor
    {
        public int Cilindros { get; set; }
        public double Cilindrada { get; set; }
        public string Potencia { get; set; }
        public int ValvulasPorCilindro { get; set; }
    }

    public class Transmision
    {
        public string Traccion { get; set; }
        public string CajaDeCambios { get; set; }
    }

    public class Confort
    {
        public bool AireAcondicionado { get; set; }
        public bool Calefaccion { get; set; }
        public bool LevantaVidrios { get; set; }
        public string Direccion { get; set; }
    }
}
