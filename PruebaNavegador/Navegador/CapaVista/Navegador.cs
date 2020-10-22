﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaControladorNavegador;
using CapaVistaSeguridad;
using CapaVistaReporteador;
using CapaVistaReporteador.Reporteador_Navegador;
using CapaVista.Usuario_Avanzado;
using CapaVista.Usuario_Normal;

namespace CapaVistaNavegador
{
    public partial class Navegador : UserControl
    {
        clsVistaBitacora Bitacora = new clsVistaBitacora();
        //codigo de guardar
        public List<Control> control = new List<Control>();
        public List<string> Modificar = new List<string>();
        public int OpGuardar;
        public int aplicacion;
        private bool reporteador = false;
        //codigo de guardar
        //codigo de actualizar
        public DataGridView DatosActualizar = new DataGridView();
        //codigo de actualizar
        public string campoEstado;
        public string tbl;
        public string cmp;
        public TextBox txt;
        public string ruta = "";
        public string ayudaRuta = "";
        public string Usuario;
        string[] word;
        bool Señal2 = false;
        public Form formulario;
        public Form MDIformulario;
        private frmReporteadorNavegador reporte;
        private frmUsuarioNormal UsuarioNormal;
        private frmUsuarioAvanzado UsuarioAvanzado;
        //Para Permisos
        public Navegador()
        {
            InitializeComponent();
            control.Reverse();
        }
        clsControlador cn = new clsControlador();
       
        public void cargar()
        {

            string Mensaje = cn.VerficarTabla(tbl);

            if (!Mensaje.Equals("bien"))
            {
                MessageBox.Show(Mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnInicio.Enabled = false;
                btnInsertar.Enabled = false;
                btnModificar.Enabled = false;
                btnEliminar.Enabled = false;
                btnRefrescar.Enabled = false;
                btnCancelar.Enabled = false;
                btnConsultar.Enabled = false;
                btnImprimir.Enabled = false;
                btnAtras.Enabled = false;
                btnFinal.Enabled = false;
                btnAdelante.Enabled = false;
                btnAyuda.Enabled = false;
                btnSalir.Enabled = false;
                btnGuardar.Enabled = false;
                DatosActualizar.Enabled = false;
                bloquear();
            }
            else
            {
                ObtenerPermisos();
                PermisosBotones();
                VerificarCampos();
                if(Señal2)
                {
                    DatosActualizar.CurrentCell = DatosActualizar.Rows[0].Cells[0];

                    int i = -1;
                    control.Reverse();
                    TextBox text = (TextBox)control.First();
                    text.Enabled = false;
                    foreach (var item in control)
                    {
                        i++;
                        item.Enabled = false;
                        // MessageBox.Show("Item: " + item.Name);
                        string datico = DatosActualizar.Rows[0].Cells[i].Value.ToString();

                        if (item is DateTimePicker)
                        {
                            DateTimePicker ll = (DateTimePicker)item;
                            ll.Value = Convert.ToDateTime(datico);

                        }
                        else
                        {
                            item.Text = datico;

                        }
                        //MessageBox.Show("data: " + datico);
                    }
                }
               
            }



        }
        private void btnInsertar_Click(object sender, EventArgs e)
        {
            OpGuardar = 1;
            Desbloquear();
            TextBox text = (TextBox)control.First();
            insertar(tbl, text.Tag.ToString(), text);
            btnModificar.Enabled = false;
            btnInsertar.Enabled = false;
            btnEliminar.Enabled = false;
            btnRefrescar.Enabled = false;
            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

        }

        public void insertar(string tabla, string campo, TextBox txt)
        {
            Desbloquear();
            string tbl = tabla;
            string cmp1 = campo;
            TextBox txt1 = txt;
            int codigo = cn.codigoMax(tbl, cmp1);
            txt1.Text = codigo.ToString();
            txt1.Enabled = false;
        }
        private void Desbloquear()
        {
            TextBox text = (TextBox)control.First();
            foreach (var item in control)
            {
                if (OpGuardar==1)
                {
                    item.Text = "";
                }
                item.Enabled = true;
            }
           
            text.Enabled = false;
        }

        private void bloquear()
        {
            foreach (var item in control)
            {
                item.Enabled = false;
            }
        }
        private void btnAyuda_Click(object sender, EventArgs e)
        {
            if(!ayudaRuta.Equals("")  && !ruta.Equals(""))
            {
                Bitacora.insert("Formulario de ayuda", aplicacion);
                Help.ShowHelp(this, ayudaRuta, ruta);
            }
            else
            {
                MessageBox.Show("Debe ingresar las rutas necesarias para abrir los archivos de ayuda", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (OpGuardar == 1)
            {
                insertar();
                actualizarData();
                MessageBox.Show("El Dato se Guardo Correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (OpGuardar == 0)
            {
                    modificar();
                    actualizarData();
                    MessageBox.Show("El Dato se Modifico Correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);                    
            }
            TextBox text = (TextBox)control.First();
            text.Enabled = false;
            PermisosBotones();
            bloquear();
            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
            ///////////////////
            DatosActualizar.CurrentCell = DatosActualizar.Rows[0].Cells[0];

            int i = -1;
            foreach (var item in control)
            {
                i++;
                // MessageBox.Show("Item: " + item.Name);
                string datico = DatosActualizar.Rows[0].Cells[i].Value.ToString();

                if (item is DateTimePicker)
                {
                    DateTimePicker ll = (DateTimePicker)item;
                    ll.Value = Convert.ToDateTime(datico);
                }
                else
                {
                    item.Text = datico;
                }
                //MessageBox.Show("data: " + datico);
            }
        }

        private void insertar()
        {

            List<string> lista = new List<string>();
            //  control.Reverse();
            foreach (var items in control)
            {
                if (items is TextBox)
                {
                    lista.Add(items.Text);
                }
                else if (items is ComboBox)
                {
                    lista.Add(items.Text);
                }
                else if (items is DateTimePicker)
                {
                    DateTimePicker fecha = (DateTimePicker)items;
                    lista.Add(fecha.Value.ToString("yyyy-MM-dd hh:mm:ss"));
                }
            }
            cn.Datos(tbl, lista, aplicacion);
        }

        private void modificar()
        {
            List<string> campos = new List<string>();
            List<string> Datos = new List<string>();
            //   control.Reverse();
            /*  foreach (var items in Modificar)
           {
               campos.Add(items);
           }*/
            foreach (var items in control)
            {
                campos.Add(items.Tag.ToString());
                if (items is TextBox)
                {
                    Datos.Add(items.Text);
                }
                else if (items is ComboBox)
                {
                    Datos.Add(items.Text);
                }
                else if (items is DateTimePicker)
                {
                    DateTimePicker fecha = (DateTimePicker)items;
                    Datos.Add(fecha.Value.ToString("yyyy-MM-dd hh:mm:ss"));
                }
            }
            cn.Datos2(tbl, campos, Datos, aplicacion);
        }

        public void actualizarData()
        {
           
    
                    DataTable dt = cn.enviar(tbl, campoEstado);
                    DatosActualizar.DataSource = dt;
            
           
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            Bitacora.insert("Refrescar Datos", aplicacion);
            actualizarData();
            DatosActualizar.CurrentCell = DatosActualizar.Rows[0].Cells[0];

            int i = -1;
            foreach (var item in control)
            {
                i++;
                // MessageBox.Show("Item: " + item.Name);
                string datico = DatosActualizar.Rows[0].Cells[i].Value.ToString();

                if (item is DateTimePicker)
                {
                    DateTimePicker ll = (DateTimePicker)item;
                    ll.Value = Convert.ToDateTime(datico);
                }
                else
                {
                    item.Text = datico;
                }
                //MessageBox.Show("data: " + datico);
            }
            TextBox text = (TextBox)control.First();
            text.Enabled = false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Esta Seguro que desea salir de la aplicacion?", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.OK)
            {
                Bitacora.insert("Salir de la aplicacion", aplicacion);
                formulario.Dispose();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            bloquear();
            Bitacora.insert("Cancelar", aplicacion);
            foreach (var item in control)
            {
                if (item is TextBox)
                {

                    item.Text = "";
                }
                else if (item is ComboBox)
                {
                    item.Text = "";
                }
                else if (item is DateTimePicker)
                {
                    DateTimePicker SeleccionadorFecha = (DateTimePicker)item;
                    SeleccionadorFecha.Value = DateTime.Now;
                }
            }
            DatosActualizar.CurrentCell = DatosActualizar.Rows[0].Cells[0];

            int i = -1;
            foreach (var item in control)
            {
                i++;
                // MessageBox.Show("Item: " + item.Name);
                string datico = DatosActualizar.Rows[0].Cells[i].Value.ToString();

                if (item is DateTimePicker)
                {
                    DateTimePicker ll = (DateTimePicker)item;
                    ll.Value = Convert.ToDateTime(datico);
                }
                else
                {
                    item.Text = datico;
                }
                //MessageBox.Show("data: " + datico);
            }
            PermisosBotones();
            TextBox text = (TextBox)control.First();
            text.Enabled = false;
            /*  btnInsertar.Enabled = true;
              btnModificar.Enabled = true;
              btnEliminar.Enabled = true;*/
            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
            btnRefrescar.Enabled = true;

        }


        public void ParametrosEliminar(string tabla, string campo, string campoid, TextBox textBox)
        {

            if (cn.Eliminar(tabla, campo, campoid, textBox.Text, aplicacion))
            {
                MessageBox.Show("Registro Eliminado","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("El registro no pudo eliminarse", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //DialogResult dialogResult = (MessageBox.Show("¿Esta Seguro de eliminar este resgistro?", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning));
            DialogResult dialogResult = MessageBox.Show("¿Esta Seguro de eliminar este registro?", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.OK)
            {
                Bitacora.insert("Eliminacion de Datos", aplicacion);
                TextBox text = (TextBox)control.First();
                ParametrosEliminar(tbl, campoEstado, control.First().Tag.ToString(), text);

                DatosActualizar.CurrentCell = DatosActualizar.Rows[0].Cells[0];

                int i = -1;
                foreach (var item in control)
                {
                    i++;
                    // MessageBox.Show("Item: " + item.Name);
                    string datico = DatosActualizar.Rows[0].Cells[i].Value.ToString();

                    if (item is DateTimePicker)
                    {
                        DateTimePicker ll = (DateTimePicker)item;
                        ll.Value = Convert.ToDateTime(datico);
                    }
                    else
                    {
                        item.Text = datico;
                    }
                    //MessageBox.Show("data: " + datico);
                }

            }

            actualizarData();
            bloquear();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            
            OpGuardar = 0;
            Desbloquear();
            btnModificar.Enabled = false;
            btnInsertar.Enabled = false;
            btnRefrescar.Enabled = false;
            btnCancelar.Enabled = true;
            btnGuardar.Enabled = true;
            btnEliminar.Enabled = false;

        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            try
            {
                DatosActualizar.CurrentCell = DatosActualizar.Rows[0].Cells[0];
                int i = -1;
                foreach (var item in control)
                {
                    i++;
                    // MessageBox.Show("Item: " + item.Name);
                    string datico = DatosActualizar.Rows[0].Cells[i].Value.ToString();

                    if (item is DateTimePicker)
                    {
                        DateTimePicker ll = (DateTimePicker)item;
                        ll.Value = Convert.ToDateTime(datico);
                    }
                    else
                    {
                        item.Text = datico;
                    }
                    //MessageBox.Show("data: " + datico);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Esta en el primer elemento de la tabla", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            int row = DatosActualizar.CurrentCell.RowIndex;

            try
            {
                DatosActualizar.CurrentCell = DatosActualizar.Rows[row - 1].Cells[0];
                int i = -1;
                foreach (var item in control)
                {
                    i++;
                    // MessageBox.Show("Item: " + item.Name);
                    string datico = DatosActualizar.Rows[row - 1].Cells[i].Value.ToString();

                    if (item is DateTimePicker)
                    {
                        DateTimePicker ll = (DateTimePicker)item;
                        ll.Value = Convert.ToDateTime(datico);
                    }
                    else
                    {
                        item.Text = datico;
                    }
                    //MessageBox.Show("data: " + datico);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ya se encuentra en el primer elemento de la tabla", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAdelante_Click(object sender, EventArgs e)
        {
            int row = DatosActualizar.CurrentCell.RowIndex;
            try
            {
                DatosActualizar.CurrentCell = DatosActualizar.Rows[row + 1].Cells[0];
                int i = -1;
                foreach (var item in control)
                {
                    i++;
                    // MessageBox.Show("Item: " + item.Name);
                    string datico = DatosActualizar.Rows[row + 1].Cells[i].Value.ToString();

                    if (item is DateTimePicker)
                    {
                        DateTimePicker ll = (DateTimePicker)item;
                        ll.Value = Convert.ToDateTime(datico);
                    }
                    else
                    {
                        item.Text = datico;
                    }
                    //MessageBox.Show("data: " + datico);
                }
            }
            catch (Exception ex)
            {
                DialogResult dialogResult = MessageBox.Show("Ya se encuentra en el ultimo elenemento ¿Desea regresar al primer elemento?", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.OK)
                {
                    actualizarData();
                    DatosActualizar.CurrentCell = DatosActualizar.Rows[0].Cells[0];

                    int i = -1;
                    foreach (var item in control)
                    {
                        i++;
                        // MessageBox.Show("Item: " + item.Name);
                        string datico = DatosActualizar.Rows[0].Cells[i].Value.ToString();

                        if (item is DateTimePicker)
                        {
                            DateTimePicker ll = (DateTimePicker)item;
                            ll.Value = Convert.ToDateTime(datico);
                        }
                        else
                        {
                            item.Text = datico;
                        }
                        //MessageBox.Show("data: " + datico);
                    }
                    TextBox text = (TextBox)control.First();
                    text.Enabled = false;
                }
                   
                
            }
        }
        private void ObtenerPermisos()
        {
            clsFuncionesSeguridad seguridad = new clsFuncionesSeguridad();
           string permisos = seguridad.Permisos(aplicacion.ToString(), Usuario);
            word = permisos.Split(',');
          
        }
        public void PermisosBotones()
        {
            //Para Permisos

            if (word[0] == "0")
            {
                btnInsertar.Enabled = false;
            }
            else
            {
                btnInsertar.Enabled = true;
            }
            ////////////////////////////////
            if (word[1]=="0")
            {
                btnModificar.Enabled = false;
            }
            else
            {
                btnModificar.Enabled = true;
            }
            //////////////////////////////////
            if (word[2]=="0")
            {
                btnEliminar.Enabled = false;
            }
            else
            {
                btnEliminar.Enabled = true;
            }
            ////////////////////////////////
            if (word[3]=="0")
            {
                btnConsultar.Enabled = false;
            }
            else
            {
                btnConsultar.Enabled = true;
            }
            ///////////////////////////////
            if (word[4]=="0")
            {
                btnImprimir.Enabled = false;
            }
            else
            {
                btnImprimir.Enabled = true;
            }
        }
        private void btnFinal_Click(object sender, EventArgs e)
        {
            int nRowIndex = DatosActualizar.Rows.Count - 1;
            try
            {
                DatosActualizar.CurrentCell = DatosActualizar.Rows[nRowIndex].Cells[0];
                int i = -1;
                foreach (var item in control)
                {
                    i++;
                    // MessageBox.Show("Item: " + item.Name);
                    string datico = DatosActualizar.Rows[nRowIndex].Cells[i].Value.ToString();

                    if (item is DateTimePicker)
                    {
                        DateTimePicker ll = (DateTimePicker)item;
                        ll.Value = Convert.ToDateTime(datico);
                    }
                    else
                    {
                        item.Text = datico;
                    }
                    //MessageBox.Show("data: " + datico);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Usted ha llegado al fondo de la tabla y no se encuentran mas datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void VerificarCampos()
        {
            List<string> CamposTabla = cn.VerficarCampo(tbl);
            
            foreach (var item in control)
            {
                bool Señal = false;
                string Elemento = item.Tag.ToString();
                foreach (var items in CamposTabla)
                {

                    if (items.Equals(Elemento))
                    {
                        Señal = true;
                    }
                   
                }

           
                if (!Señal)
                {
                    MessageBox.Show("El campo: "+Elemento+" No existe en la tabla "+tbl, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnInicio.Enabled = false;
                    btnInsertar.Enabled = false;
                    btnModificar.Enabled = false;
                    btnEliminar.Enabled = false;
                    btnRefrescar.Enabled = false;
                    btnCancelar.Enabled = false;
                    btnConsultar.Enabled = false;
                    btnImprimir.Enabled = false;
                    btnAtras.Enabled = false;
                    btnFinal.Enabled = false;
                    btnAdelante.Enabled = false;
                    btnAyuda.Enabled = false;
                    btnSalir.Enabled = false;
                    btnGuardar.Enabled = false;
                    DatosActualizar.Enabled = false;
                    bloquear();
                }
            }
            Señal2 = false;
            foreach(var items in CamposTabla)
            {
                if (items.Equals(campoEstado))
                {
                    Señal2 = true;
                }
            }
            if(!Señal2)
            {
                MessageBox.Show("El campo: " + campoEstado + " No existe en la tabla " + tbl, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnInicio.Enabled = false;
                btnInsertar.Enabled = false;
                btnModificar.Enabled = false;
                btnEliminar.Enabled = false;
                btnRefrescar.Enabled = false;
                btnCancelar.Enabled = false;
                btnConsultar.Enabled = false;
                btnImprimir.Enabled = false;
                btnAtras.Enabled = false;
                btnFinal.Enabled = false;
                btnAdelante.Enabled = false;
                btnAyuda.Enabled = false;
                btnSalir.Enabled = false;
                btnGuardar.Enabled = false;
                DatosActualizar.Enabled = false;
                bloquear();
            }
            
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {           
            try
            {
                if (reporte == null)
                {
                    reporte = new frmReporteadorNavegador(aplicacion);
                    reporte.MdiParent = MDIformulario;
                    reporte.FormClosed += (o, args) => reporte = null;
                }
                reporte.Show();
                reporte.BringToFront();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Verifique que si tiene instalado el software necesario para utilizar esta aplicacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
          
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            clsVistaBitacora bit = new clsVistaBitacora();
            clsFuncionesSeguridad seguridad = new clsFuncionesSeguridad();
            if (seguridad.PermisosAcceso("201", Usuario) == 1)
            {
                if(UsuarioAvanzado == null)
                {
                    UsuarioAvanzado = new frmUsuarioAvanzado();
                    UsuarioAvanzado.MdiParent = MDIformulario;
                    UsuarioAvanzado.FormClosed += (o, args) => UsuarioAvanzado = null;
                }
                bit.user(Usuario);
                UsuarioAvanzado.Show();
                UsuarioAvanzado.BringToFront();
            }
            else
            {
                if (UsuarioNormal == null)
                {
                    UsuarioNormal = new frmUsuarioNormal(tbl);
                    UsuarioNormal.MdiParent = MDIformulario;
                    UsuarioNormal.FormClosed += (o, args) => UsuarioNormal = null;
                }
                bit.user(Usuario);
                UsuarioNormal.Show();
                UsuarioNormal.BringToFront();

               
            }
        }
    }
}
