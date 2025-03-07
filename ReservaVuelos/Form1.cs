using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace ReservaVuelos
{
    public partial class Form1 : Form
    {
        List<Vuelo> Vuelos;
        List<Reserva> Reservas;
        Vuelo VueloSeleccionado = null;

        public Form1()
        {
            InitializeComponent();
            Vuelos = new List<Vuelo>();
            Reservas = new List<Reserva>();

            InicializarDataGrids();
            DeshabilitarCamposReserva();
        }

        private void btnAgregarVuelo_Click(object sender, EventArgs e)
        {
            string Codigo = txtCodigoVuelo.Text;
            string Origen = txtOrigen.Text;
            string Destino = txtDestino.Text;
            DateTime Fecha = dtpFechaSalida.Value;
            int Asientos;

            if (Codigo.Trim().Length == 0) { MessageBox.Show("El código es obligatorio."); return; }
            if (Vuelos.Any(vuelo => vuelo.Codigo == Codigo)) { MessageBox.Show($"El código {Codigo} ya existe."); return; }
            if (Origen.Trim().Length == 0) { MessageBox.Show("El origen es obligatorio."); return; }
            if (Destino.Trim().Length == 0) { MessageBox.Show("El destino es obligatorio."); return; }
            if (!dtpFechaSalida.Checked) { MessageBox.Show("Por favor, seleccione una fecha."); return; }
            if (!int.TryParse(txtAsientosDisponibles.Text, out Asientos)) { MessageBox.Show("Los asientos son invalidos, ingrese un número."); return; }
            if (Asientos <= 0) { MessageBox.Show("Los asientos deben ser mayor a 0."); return; }

            Vuelos.Add(new Vuelo(Codigo, Origen, Destino, Fecha, Asientos));
            ActualizarDataGridVuelos(Vuelos, dtgVuelos);
        }

        private void btnReservarVuelo_Click(object sender, EventArgs e)
        {
            string CodigoReserva = txtCodigoReserva.Text;
            int Asientos;

            if (CodigoReserva.Trim().Length == 0) { MessageBox.Show("El código es obligatorio."); return; }
            if (Reservas.Any(reserva => reserva.CodigoReserva == CodigoReserva)) { MessageBox.Show($"La reserva {CodigoReserva} ya existe."); return; }
            if (!int.TryParse(txtCantidadReservas.Text, out Asientos)) { MessageBox.Show("Los asientos son invalidos, ingrese un número."); return; }
            if (Asientos <= 0) { MessageBox.Show("Los asientos deben ser mayor a 0."); return; }

            if (VueloSeleccionado != null && VueloSeleccionado.ValidarAsientosDisponibles(Asientos))
            {
                VueloSeleccionado.RestarAsientosDisponibles(Asientos);
                Reservas.Add(new Reserva(CodigoReserva, VueloSeleccionado, Asientos));

                ActualizarDataGridVuelos(Vuelos, dtgVuelos);
                ActualizarDataGridReservas(Reservas, dtgReservas);
                DeshabilitarCamposReserva();
                dtgVuelos.ClearSelection();
            }
            else MessageBox.Show("La cantidad de reservas supera a los asientos disponibles");
        }

        private void dtgVuelos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow filaSeleccionada = dtgVuelos.Rows[e.RowIndex];
                string dato = filaSeleccionada.Cells[0].Value?.ToString();
                if (dato.Trim() != "")
                {
                    habilitarCamposReserva();
                    VueloSeleccionado = Vuelos.Find(vuelo => vuelo.Codigo == dato);
                    lblVueloSeleccionado.Text = $"Vuelo Seleccionado: {VueloSeleccionado.Codigo} [origen: {VueloSeleccionado.Origen}, destino: {VueloSeleccionado.Destino}]";
                }
                else DeshabilitarCamposReserva();
            }
        }

        private void ActualizarDataGridVuelos(List<Vuelo> Vuelos, DataGridView MyDataGridView)
        {
            MyDataGridView.Rows.Clear();

            foreach (var Vuelo in Vuelos)
            {
                MyDataGridView.Rows.Add(Vuelo.Codigo, Vuelo.Origen, Vuelo.Destino, Vuelo.FechaSalida.ToString(), Vuelo.AsientosDisponibles);
            }
        }

        private void ActualizarDataGridReservas(List<Reserva> Reservas, DataGridView MyDataGridView)
        {
            MyDataGridView.Rows.Clear();

            foreach (var Reserva in Reservas)
            {
                MyDataGridView.Rows.Add(
                    Reserva.CodigoReserva,
                    Reserva.Vuelo.Codigo,
                    Reserva.Vuelo.Origen,
                    Reserva.Vuelo.Destino,
                    Reserva.Vuelo.FechaSalida.ToString(),
                    Reserva.AsientosReservados
                );
            }
        }

        private void InicializarDataGrids()
        {
            dtgVuelos.Rows.Clear();

            dtgVuelos.Columns.Clear();
            dtgVuelos.Columns.Add("Código", "Código");
            dtgVuelos.Columns.Add("Origen", "Origen");
            dtgVuelos.Columns.Add("Destino", "Destino");
            dtgVuelos.Columns.Add("Fecha Salida", "Fecha Salida");
            dtgVuelos.Columns.Add("Asientos disponibles", "Asientos disponibles");
            dtgVuelos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dtgReservas.Rows.Clear();

            dtgReservas.Columns.Clear();
            dtgReservas.Columns.Add("Código Reserva", "Código Reserva");
            dtgReservas.Columns.Add("Código", "Código");
            dtgReservas.Columns.Add("Origen", "Origen");
            dtgReservas.Columns.Add("Destino", "Destino");
            dtgReservas.Columns.Add("Fecha Salida", "Fecha Salida");
            dtgReservas.Columns.Add("Asientos", "Asientos");
            dtgReservas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void DeshabilitarCamposReserva()
        { 
            txtCodigoReserva.Text = "";
            txtCantidadReservas.Text = "";
            lblVueloSeleccionado.Text = "";

            txtCodigoReserva.Enabled = false;
            txtCantidadReservas.Enabled = false;
            btnReservarVuelo.Enabled = false;
            VueloSeleccionado = null;
        }

        private void habilitarCamposReserva()
        {
            txtCodigoReserva.Enabled = true;
            txtCantidadReservas.Enabled = true;
        }

        private void txtCantidadReservas_TextChanged(object sender, EventArgs e)
        {
            btnReservarVuelo.Enabled = txtCantidadReservas.Text.Trim().Length > 0;
        }
    }
}
